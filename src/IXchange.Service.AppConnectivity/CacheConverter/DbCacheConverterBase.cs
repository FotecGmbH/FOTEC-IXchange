// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Biss.Dc.Core;
using Biss.Interfaces;
using Biss.Log.Producer;
using Biss.Reflection;
using Biss.Serialize;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


// ReSharper disable once CheckNamespace 
// ToDo: Eventuell eigenes Nuget für Db?!
namespace ConnectivityHost.CacheConverter;

/// <summary>
/// Modi für das Bearbeiten eines einzelnen Elements
/// </summary>
public enum EnumDbCacheElementOperations
{
    /// <summary>
    /// Neuer Datensatz
    /// </summary>
    New,

    /// <summary>
    /// Daten wurden modifiziert
    /// </summary>
    Modified,

    /// <summary>
    /// Element soll gelöscht werden
    /// </summary>
    Deleted
}

/// <summary>
///     Modes für den DbCache
/// </summary>
public enum EnumDbCacheConverterModes
{
#pragma warning disable CS1591
    On,
    OnLoadAllOnStart,
    Off
#pragma warning restore CS1591
}

/// <summary>
///     <para>Daten aus Db für ViewModel konvertieren und zwischenspeichern (optional)</para>
/// Klasse DbCacheConverterBase. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
/// </summary>
/// <typeparam name="TDb">Datenbanktyp</typeparam>
/// <typeparam name="TDbTable">Tabellentyp der Datenbank</typeparam>
/// <typeparam name="TModel">Typ des Modells für das ViewModel</typeparam>
public abstract class DbCacheConverterBase<TDb, TDbTable, TModel>
    where TDb : DbContext
    where TDbTable : class, IDcChangeTracking
    where TModel : class, IBissModel
{
    private readonly Dictionary<long, DcServerListItem<TModel>> _cache = new Dictionary<long, DcServerListItem<TModel>>();
    private readonly SemaphoreSlim _dbReadLock = new SemaphoreSlim(1, 1);
    private readonly EnumDbCacheConverterModes _mode;
    private readonly PropertyInfo _rootDataSetProperty;


    /// <summary>
    ///     Daten aus Db für ViewModel konvertieren und zwischenspeichern
    /// </summary>
    /// <param name="rootDataSetPropertyName">Name der Tabelle im Db Kontext</param>
    /// <param name="mode">Verhalten</param>
    /// <exception cref="ArgumentNullException"></exception>
    protected DbCacheConverterBase(string rootDataSetPropertyName, EnumDbCacheConverterModes mode)
    {
        _mode = mode;
        var pi = typeof(TDb).GetProperty(rootDataSetPropertyName);
        if (pi == null!)
        {
            throw new ArgumentNullException($"[DbCacheConverterBase](Constructor): {nameof(pi)}");
        }

        _rootDataSetProperty = pi;
        if (_mode == EnumDbCacheConverterModes.OnLoadAllOnStart)
        {
            _ = Task.Run(async () => { await LoadAllToCache().ConfigureAwait(false); });
        }
    }

    /// <summary>
    ///     Alle Daten laden
    /// </summary>
    /// <param name="reload"></param>
    /// <param name="userId">User Id (in Verbindung mit ModifyDataForUserOrDevice Postprozessing)</param>
    /// <param name="deviceId">Device Id (in Verbindung mit ModifyDataForUserOrDevice Postprozessing)</param>
    /// <returns></returns>
    public async Task<List<DcServerListItem<TModel>>> LoadAllToCache(bool reload = false, long userId = -1, long deviceId = -1)
    {
        if (reload)
        {
            _cache.Clear();
        }

#pragma warning disable CA2007
        await using var db = Activator.CreateInstance<TDb>();
#pragma warning restore CA2007
        var ds = (DbSet<TDbTable>) _rootDataSetProperty.GetValue(db)!;
        var r = await GetModelData(db, ds, userId, deviceId).ConfigureAwait(true);
        return r;
    }

    /// <summary>
    ///     Daten mit folgenden Ids lesen
    /// </summary>
    /// <param name="id">Einzelner Datensatz</param>
    /// <param name="userId">User Id (in Verbindung mit ModifyDataForUserOrDevice Postprozessing)</param>
    /// <param name="deviceId">Device Id (in Verbindung mit ModifyDataForUserOrDevice Postprozessing)</param>
    /// <returns></returns>
    public async Task<DcServerListItem<TModel>?> GetModelData(long id, long userId = -1, long deviceId = -1)
    {
        return (await GetModelData(new List<long> {id}, userId, deviceId).ConfigureAwait(false)).FirstOrDefault();
    }

    /// <summary>
    ///     Daten mit folgenden Ids lesen
    /// </summary>
    /// <param name="db">Db Kontext</param>
    /// <param name="id">Einzelner Datensatz</param>
    /// <param name="userId">User Id (in Verbindung mit ModifyDataForUserOrDevice Postprozessing)</param>
    /// <param name="deviceId">Device Id (in Verbindung mit ModifyDataForUserOrDevice Postprozessing)</param>
    /// <returns></returns>
    public async Task<DcServerListItem<TModel>?> GetModelData(TDb db, long id, long userId = -1, long deviceId = -1)
    {
        return (await GetModelData(db, new List<long> {id}, userId, deviceId).ConfigureAwait(false)).FirstOrDefault();
    }


    /// <summary>
    ///     Daten mit folgenden Ids lesen
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="userId">User Id (in Verbindung mit ModifyDataForUserOrDevice Postprozessing)</param>
    /// <param name="deviceId">Device Id (in Verbindung mit ModifyDataForUserOrDevice Postprozessing)</param>
    /// <returns></returns>
    public async Task<List<DcServerListItem<TModel>>> GetModelData(List<long> ids, long userId = -1, long deviceId = -1)
    {
#pragma warning disable CA2007
        await using var db = Activator.CreateInstance<TDb>();
#pragma warning restore CA2007
        return await GetModelData(db, ids, userId, deviceId).ConfigureAwait(false);
    }

    /// <summary>
    ///     Daten mit folgenden Ids lesen
    /// </summary>
    /// <param name="db">Db Kontext</param>
    /// <param name="ids"></param>
    /// <param name="userId">User Id (in Verbindung mit ModifyDataForUserOrDevice Postprozessing)</param>
    /// <param name="deviceId">Device Id (in Verbindung mit ModifyDataForUserOrDevice Postprozessing)</param>
    /// <returns></returns>
    public async Task<List<DcServerListItem<TModel>>> GetModelData(TDb db, List<long> ids, long userId = -1, long deviceId = -1)
    {
        var ds = (DbSet<TDbTable>) _rootDataSetProperty.GetValue(db)!;
        var rows = ds.Where(w => ids.Contains(w.Id));
        return await GetModelData(db, rows, userId, deviceId).ConfigureAwait(false);
    }


    /// <summary>
    ///     Daten eines Queryable lesen
    /// </summary>
    /// <param name="db">Aktuelle Db Instanz des Queries</param>
    /// <param name="entries">Query</param>
    /// <param name="userId">User Id (in Verbindung mit ModifyDataForUserOrDevice Postprozessing)</param>
    /// <param name="deviceId">Device Id (in Verbindung mit ModifyDataForUserOrDevice Postprozessing)</param>
    /// <param name="safeMode">
    /// Abgleich mit Db ob gelöscht und ob modifiziert. Lngsamer aber sicherer. Wenn aber richtig
    /// implementiert nicht notwendig!
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public async Task<List<DcServerListItem<TModel>>> GetModelData(TDb db, IQueryable<TDbTable> entries, long userId = -1, long deviceId = -1, bool safeMode = false)
    {
        if (db == null!)
        {
            throw new ArgumentNullException($"[DbCacheConverterBase]({nameof(GetModelData)}): {nameof(db)}");
        }

        if (entries == null!)
        {
            throw new ArgumentNullException($"[DbCacheConverterBase]({nameof(GetModelData)}): {nameof(entries)}");
        }

        Logging.Log.LogTrace($"[{GetType().Name}]({nameof(GetModelData)}): {_cache.Count} Elements in Cache.");

        await _dbReadLock.WaitAsync().ConfigureAwait(true);
        var sw = new Stopwatch();
        sw.Start();

        if (safeMode && _mode != EnumDbCacheConverterModes.Off && _cache.Count > 0)
        {
            var sw2 = new Stopwatch();
            sw2.Start();

            CheckDeleted(db);
            CheckModified(db);

            sw2.Stop();
            Logging.Log.LogTrace($"[{GetType().Name}]({nameof(GetModelData)}): SaveMode enabled. Takes {sw2.ElapsedMilliseconds} ms");
            Logging.Log.LogTrace($"[{GetType().Name}]({nameof(GetModelData)}): {_cache.Count} Elements in Cache after safeMode.");
        }

        var allIds = entries.Select(s => s.Id).ToList();
        var notLoaded = allIds.Except(_cache.Keys).ToList();
        if (notLoaded.Any())
        {
            var tmpQuery = ModifyQueryableIncludes(entries);
            var newData = await GetDataFromDbAndConvert(db, tmpQuery.Where(i => notLoaded.Contains(i.Id))).ConfigureAwait(false);

            if (_mode == EnumDbCacheConverterModes.Off)
            {
                sw.Stop();
                Logging.Log.LogInfo($"[{GetType().Name}]({nameof(GetModelData)}): Read takes {sw.Elapsed}");
                _dbReadLock.Release();
                return newData;
            }

            if (newData.Any())
            {
                foreach (var e in newData)
                {
                    _cache.Add(e.Index, e);
                }
            }
        }

        var r = _cache.Where(k => allIds.Contains(k.Key)).ToList();
        var checkAllLoaded = r.Select(s => s.Key).Except(allIds).ToList();
        if (checkAllLoaded.Any())
        {
            Logging.Log.LogWarning($"[DbCacheConverterBase]({nameof(GetModelData)}): Not loaded Ids: {JsonConvert.SerializeObject(checkAllLoaded)}");
        }

        sw.Stop();

        var data = r.Select(s => s.Value).ToList();
        var m = typeof(DbCacheConverterBase<TDb, TDbTable, TModel>).GetMethod(nameof(ModifyDataForUserOrDevice), BindingFlags.Instance | BindingFlags.NonPublic);
        if (m!.IsOverride(GetType()))
        {
            if (_mode == EnumDbCacheConverterModes.Off)
            {
                data = await ModifyDataForUserOrDevice(db, userId, deviceId, data).ConfigureAwait(true);
            }
            else
            {
                var copy = BissDeserialize.FromJson<List<DcServerListItem<TModel>>>(data.ToJson());
                data = await ModifyDataForUserOrDevice(db, userId, deviceId, copy).ConfigureAwait(true);
            }
        }

        _dbReadLock.Release();
        return data;
    }

    /// <summary>
    ///     Syncdaten holen.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="data4User"></param>
    /// <param name="currentElements"></param>
    /// <param name="userId"></param>
    /// <param name="deviceId"></param>
    /// <param name="safeMode"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<DcListSyncResultData<TModel>> GetSyncData(TDb db, IQueryable<TDbTable> data4User, DcListSyncData currentElements, long userId, long deviceId, bool safeMode = false)
    {
        if (currentElements == null!)
        {
            throw new ArgumentNullException($"[{GetType().Name}]({nameof(GetSyncData)}): {nameof(currentElements)}");
        }


        Logging.Log.LogTrace($"[{GetType().Name}]({nameof(GetSyncData)}): DeviceId {deviceId} UserId {userId}");
        Logging.Log.LogTrace($"[{GetType().Name}]({nameof(GetSyncData)}): Current: {currentElements.GetDebugInfos()}");
        var sw = new Stopwatch();
        sw.Start();

        var r = new DcListSyncResultData<TModel>();

        if (currentElements == null!)
        {
            throw new ArgumentNullException($"[DbCacheConverterBase]({nameof(GetSyncData)}): {nameof(currentElements)}");
        }

        //Alle Daten welche der User benötigt aus dem Cache laden (bzw. aus der Db falls nich nicht im Cache)
        var step1 = await GetModelData(db, data4User, userId, deviceId, safeMode).ConfigureAwait(false);


        //welche sollen beim User gelöscht werden?
        var step2 = currentElements.CurrentListEntries
            .Select(s => s.Id)
            .Except(step1.Select(s => s.Index))
            .ToList();
        r.ItemsToRemoveOnClient = step2;


        foreach (var element in step1.ToArray())
        {
            if (currentElements.CurrentListEntries.Any(a => a == element.ToDcSyncElement()))
            {
                step1.Remove(element);
            }
        }

        r.NewOrModifiedItems = step1;


        sw.Stop();
        Logging.Log.LogTrace($"[{GetType().Name}]({nameof(GetSyncData)}): Modified Ids {JsonConvert.SerializeObject(r.NewOrModifiedItems.Select(s => s.Index).ToList())}");
        Logging.Log.LogTrace($"[{GetType().Name}]({nameof(GetSyncData)}): Deleted Ids {JsonConvert.SerializeObject(r.ItemsToRemoveOnClient)}");
        Logging.Log.LogInfo($"[{GetType().Name}]({nameof(GetSyncData)}): Sync takes {sw.ElapsedMilliseconds} ms");

        return r;
    }

    /// <summary>
    ///     Cache löschen
    /// </summary>
    public void RemoveAllFromCache()
    {
        _cache.Clear();
    }

    /// <summary>
    ///     Einzelne Id aus Cache löschen
    /// </summary>
    /// <param name="id"></param>
    public void RemoveFromCache(long id)
    {
        RemoveFromCache(new List<long> {id});
    }

    /// <summary>
    ///     Bestimmte Ids aus Cache löschen
    /// </summary>
    /// <param name="ids"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void RemoveFromCache(List<long> ids)
    {
        if (ids == null!)
        {
            throw new ArgumentNullException($"[DbCacheConverterBase]({nameof(RemoveFromCache)}): {nameof(ids)}");
        }

        foreach (var id in ids)
        {
            _cache.Remove(id);
        }
    }

    /// <summary>
    ///     Daten abspeichern.
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="userId"></param>
    /// <param name="data"></param>
    /// <param name="deleteFileAction"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public virtual async Task<DcListStoreResult> StoreData(long deviceId, long userId, List<DcStoreListItem<TModel>> data, Func<TDb, long, Task> deleteFileAction = null!)
    {
        if (data == null!)
        {
            throw new ArgumentNullException(nameof(data));
        }

        var dataInfo = data.Select(s => new {s.Index, s.State}).ToList();
        Logging.Log.LogInfo($"[{GetType().Name}]({nameof(StoreData)}): Items: {JsonConvert.SerializeObject(dataInfo)}");


#pragma warning disable CA2007
        await using var db = Activator.CreateInstance<TDb>();
#pragma warning restore CA2007
        var ds = (DbSet<TDbTable>) _rootDataSetProperty.GetValue(db)!;

        // ReSharper disable once CollectionNeverQueried.Local
        var modifiedOrNewIds = new List<long>();

        var result = new DcListStoreResult();
        foreach (var item in data)
        {
            switch (item.State)
            {
                case EnumDcListElementState.New:
                    var tmpNewItem = Activator.CreateInstance<TDbTable>();
                    var newItem = await StoreElement(db, deviceId, userId, item.Data, tmpNewItem, 0, EnumDbCacheElementOperations.New, deleteFileAction).ConfigureAwait(false);
                    if (newItem == null!)
                    {
                        throw new ArgumentNullException($"[{GetType().Name}]({nameof(StoreData)}): {nameof(newItem)}");
                    }

                    if (!ds.Contains(newItem))
                    {
                        await ds.AddAsync(newItem).ConfigureAwait(true);
                    }

                    await db.SaveChangesAsync().ConfigureAwait(true);
                    modifiedOrNewIds.Add(newItem.Id);
                    result.ElementsStored++;
                    var tmp = new DcListStoreResultIndexAndData
                              {
                                  BeforeStoreIndex = item.Index,
                                  NewIndex = newItem.Id,
                                  NewSortIndex = newItem.Id,
                                  DataVersion = new byte[8] // Absichtlich nicht - newItem.DataVersion!
                              };
                    result.NewIndex.Add(tmp);

                    break;
                case EnumDcListElementState.Modified:
                    var tmpModItem = await ds.FirstOrDefaultAsync(f => f.Id == item.Index).ConfigureAwait(true);
                    if (tmpModItem == null!)
                    {
                        throw new ArgumentNullException($"[{GetType().Name}]({nameof(StoreData)}): {nameof(tmpModItem)}");
                    }

                    var modifiedItem = await StoreElement(db, deviceId, userId, item.Data, tmpModItem, item.Index, EnumDbCacheElementOperations.Modified, deleteFileAction).ConfigureAwait(false);
                    if (modifiedItem == null!)
                    {
                        throw new ArgumentNullException($"[{GetType().Name}]({nameof(StoreData)}): {nameof(modifiedItem)}");
                    }

                    await db.SaveChangesAsync().ConfigureAwait(true);
                    modifiedOrNewIds.Add(modifiedItem.Id);
                    result.ElementsStored++;
                    break;
                case EnumDcListElementState.Deleted:
                    var tmpDelItem = await ds.FirstOrDefaultAsync(f => f.Id == item.Index).ConfigureAwait(true);
                    if (tmpDelItem == null!)
                    {
                        throw new ArgumentNullException($"[{GetType().Name}]({nameof(StoreData)}): {nameof(tmpDelItem)}");
                    }

                    var itemTodelete = await StoreElement(db, deviceId, userId, item.Data, tmpDelItem, item.Index, EnumDbCacheElementOperations.Deleted, deleteFileAction).ConfigureAwait(false);
                    if (itemTodelete == null!)
                    {
                        throw new ArgumentNullException($"[{GetType().Name}]({nameof(StoreData)}): {nameof(itemTodelete)}");
                    }

                    if (ds.Contains(itemTodelete))
                    {
                        ds.Remove(itemTodelete);
                    }

                    await db.SaveChangesAsync().ConfigureAwait(true);
                    break;
                case EnumDcListElementState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RemoveFromCache(item.Index);
        }


        return result;
    }

    /// <summary>
    /// User (oder Device) abhängiges Post-Prozessing der Daten.
    /// Wenn überschrieben werden Kopien der Daten erzugt um die zwischengespeicherten Daten nicht zu verändern (wenn Cache
    /// aktiv)
    /// </summary>
    /// <param name="db">Aktuelle db Kontext wenn benötigt</param>
    /// <param name="userId">User Id (-1 default)</param>
    /// <param name="deviceId">Device Id (-1 defauult)</param>
    /// <param name="data">Rohdaten ohe User/Device Bezug (bereits eine Kopie)</param>
    /// <returns></returns>
    protected virtual Task<List<DcServerListItem<TModel>>> ModifyDataForUserOrDevice(TDb db, long userId, long deviceId, List<DcServerListItem<TModel>> data)
    {
        throw new InvalidOperationException("Not allowed to call this base method!");
    }

    /// <summary>
    ///     Falls zusätzliche Includes benötigt werden
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    protected virtual IQueryable<TDbTable> ModifyQueryableIncludes(IQueryable<TDbTable> data)
    {
        return data.AsNoTracking();
    }

    /// <summary>
    ///     Liste der Daten aus Db konvertieren zm Model (für ViewModel)
    /// </summary>
    /// <param name="db">Aktueller Db Kontext falls benötigt</param>
    /// <param name="data">Daten aus Db zum Konvertieren</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    protected virtual async Task<List<DcServerListItem<TModel>>> GetDataFromDbAndConvert(TDb db, IQueryable<TDbTable> data)
    {
        if (data == null!)
        {
            throw new ArgumentNullException($"[DbCacheConverterBase]({nameof(GetDataFromDbAndConvert)}): {nameof(data)}");
        }

        var r = new List<DcServerListItem<TModel>>();
        foreach (var dbData in data)
        {
            var e = await ConvertToModel(db, dbData);
            r.Add(e);
        }

        return r;
    }

    /// <summary>
    /// Datenversion eines Datensatzes eines DB Eintrags berechnen
    /// </summary>
    /// <param name="db">Aktueller Db Kontext bei Bedarf</param>
    /// <param name="dbData">aktueller Datensat</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    protected virtual byte[] GetDataVersion(TDb db, TDbTable dbData)
    {
        if (dbData == null!)
        {
            throw new ArgumentNullException($"[DbCacheConverterBase]({nameof(GetDataVersion)}): {nameof(dbData)}");
        }

        return dbData.DataVersion;
    }

    /// <summary>
    ///     Einzelnen Db Datensatz konvertieren
    /// </summary>
    /// <param name="db">Aktueller Db Kontext falls benötigt</param>
    /// <param name="dbData">Einzelner Db-Datensatz</param>
    /// <returns></returns>
    protected abstract Task<DcServerListItem<TModel>> ConvertToModel(TDb db, TDbTable dbData);

    /// <summary>
    /// Ein einzelnen Element in der Datenbank sichern. Add/Remove/SaveChanges usw. passiert automatisch und muss in dieser
    /// Methode nicht aufgerufen werden.
    /// </summary>
    /// <param name="db">Aktueller Datenbankkontext für das Bearbeiten</param>
    /// <param name="deviceId">Geräte Id des Bearbeiters</param>
    /// <param name="userId">User Id - welcher bearbeitet</param>
    /// <param name="newOrModifiedData">Neue bzw. modifizierte Daten</param>
    /// <param name="dbData">
    /// Db Datensatz - one "Includes" - wenn "mehr" gebraucht wird dann mit der dbId und db Kontext
    /// arbeiten. Immer den richtigen Db-Datensatz dann zurückliefern für das Add/Remove/SaveChanges usw.
    /// </param>
    /// <param name="dbId">Id des Datensatzes (bei "New" immer 0)</param>
    /// <param name="operation">New/Edit/Delete</param>
    /// <param name="deleteFileAction">
    /// Falls "Dateien aus DB und aus einer Storage gelöscht werden müssen dann hier die Methode
    /// angeben
    /// </param>
    /// <returns>Db-Datensatz</returns>
    protected abstract Task<TDbTable> StoreElement(TDb db, long deviceId, long userId, TModel newOrModifiedData, TDbTable dbData, long dbId, EnumDbCacheElementOperations operation, Func<TDb, long, Task> deleteFileAction);


    private void CheckModified(TDb db)
    {
        var cachedData = _cache.Select(s => s.Value.ToDcSyncElement()).ToList();
        if (cachedData.Count <= 0)
        {
            return;
        }


        var m = typeof(DbCacheConverterBase<TDb, TDbTable, TModel>).GetMethod(nameof(GetDataVersion), BindingFlags.Instance | BindingFlags.NonPublic);
        if (m!.IsOverride(GetType()))
        {
            try
            {
                var ds = (DbSet<TDbTable>) _rootDataSetProperty.GetValue(db)!;
                var tmp = cachedData.Select(c => c.Id).ToList();
                var step1 = ds.Where(s => tmp.Contains(s.Id));
                var step2 = ModifyQueryableIncludes(step1);

                foreach (var element in cachedData)
                {
                    var check = step2.FirstOrDefault(f => f.Id == element.Id);
                    var t1 = GetDataVersion(db, check!);
                    var t2 = new DcSyncElement(check!.Id, t1);
                    if (t2 != element)
                    {
                        Debugger.Break();
                        _cache.Remove(element.Id);
                    }
                }
            }
            catch (Exception e)
            {
                Logging.Log.LogError($"{e}");
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
        }
        else
        {
            var ds = (DbSet<TDbTable>) _rootDataSetProperty.GetValue(db)!;
            var tmp = cachedData.Select(c => c.Id).ToList();
            var step1 = ds.Where(s => tmp.Contains(s.Id)).Select(s => new {s.Id, s.DataVersion}).ToList();
            foreach (var element in cachedData)
            {
                var check = step1.FirstOrDefault(f => f.Id == element.Id);
                var t2 = new DcSyncElement(check!.Id, check.DataVersion);
                if (t2 != element)
                {
                    Debugger.Break();
                    _cache.Remove(element.Id);
                }
            }
        }
    }

    private void CheckDeleted(TDb db)
    {
        if (_cache.Count == 0)
        {
            return;
        }

        var ds = (DbSet<TDbTable>) _rootDataSetProperty.GetValue(db)!;

        //Gelöschte User (Abgleich welche in der DB nicht mehr existieren)
        var itemsToRemoveOnClient = _cache.Keys
            .Except(ds.Select(s => s.Id))
            .ToList();

        foreach (var l in itemsToRemoveOnClient)
        {
            _cache.Remove(l);
        }
    }
}

/// <summary>
///     Daten aus Db für ViewModel konvertieren und zwischenspeichern (optional)
///     Generische abtraierung falls "nur" eine Tabelle für das Modell genügt und die Properties gleich benannt sind
/// </summary>
/// <typeparam name="TDb">Datenbanktyp</typeparam>
/// <typeparam name="TDbTable">Tabellentyp der Datenbank</typeparam>
/// <typeparam name="TModel">Typ des Modells für das ViewModel</typeparam>
public class DbCacheConverterGeneric<TDb, TDbTable, TModel> : DbCacheConverterBase<TDb, TDbTable, TModel>
    where TDb : DbContext
    where TDbTable : class, IDcChangeTracking
    where TModel : class, IBissModel
{
    /// <summary>
    ///     Daten aus Db für ViewModel konvertieren und zwischenspeichern
    ///     Generische abtraierung falls "nur" eine Tabelle für das Modell genügt und die Properties gleich benannt sind
    /// </summary>
    /// <param name="rootDataSetPropertyName">Name der Tabelle im Db Kontext</param>
    /// <param name="mode">Verhalten</param>
    /// <exception cref="ArgumentNullException"></exception>
    public DbCacheConverterGeneric(string rootDataSetPropertyName, EnumDbCacheConverterModes mode = EnumDbCacheConverterModes.On) : base(rootDataSetPropertyName, mode)
    {
    }

    /// <summary>
    /// Convert to model
    /// </summary>
    /// <param name="db">db</param>
    /// <param name="dbData">db data</param>
    /// <returns>model</returns>
    protected override Task<DcServerListItem<TModel>> ConvertToModel(TDb db, TDbTable dbData)
    {
        var r = Activator.CreateInstance<TModel>();
        var modelProps = typeof(TModel).GetProperties();
        var dbProps = typeof(TDbTable).GetProperties();

        foreach (var prop in modelProps)
        {
            if (prop.CanWrite)
            {
                var dbProp = dbProps.FirstOrDefault(f => string.Equals(f.Name, prop.Name, StringComparison.InvariantCultureIgnoreCase));
                if (dbProp != null!)
                {
                    try
                    {
                        prop.SetValue(r, dbProp.GetValue(dbData));
                    }
                    catch (Exception e)
                    {
                        Logging.Log.LogWarning($"[{GetType().Name}]({nameof(StoreElement)}): Can not set property {dbProp.Name}: {e}");
                    }
                }
            }
        }

        var result = new DcServerListItem<TModel>
                     {
                         Data = r,
                         DataVersion = GetDataVersion(db, dbData),
                         Index = dbData.Id,
                         SortIndex = dbData.Id
                     };

        return Task.FromResult(result);
    }

    /// <summary>
    /// Ein einzelnen Element in der Datenbank sichern. Add/Remove/SaveChanges usw. passiert automatisch und muss in dieser
    /// Methode nicht aufgerufen werden.
    /// </summary>
    /// <param name="db">Aktueller Datenbankkontext für das Bearbeiten</param>
    /// <param name="deviceId">Geräte Id des Bearbeiters</param>
    /// <param name="userId">User Id - welcher bearbeitet</param>
    /// <param name="newOrModifiedData">Neue bzw. modifizierte Daten</param>
    /// <param name="dbData">
    /// Db Datensatz - one "Includes" - wenn "mehr" gebraucht wird dann mit der dbId und db Kontext
    /// arbeiten. Immer den richtigen Db-Datensatz dann zurückliefern für das Add/Remove/SaveChanges usw.
    /// </param>
    /// <param name="dbId">Id des Datensatzes (bei "New" immer 0)</param>
    /// <param name="operation">New/Edit/Delete</param>
    /// <param name="deleteFileAction">
    /// Falls "Dateien aus DB und aus einer Storage gelöscht werden müssen dann hier die Methode
    /// angeben
    /// </param>
    /// <returns>Db-Datensatz</returns>
    // ReSharper disable once OptionalParameterHierarchyMismatch
    protected override Task<TDbTable> StoreElement(TDb db, long deviceId, long userId, TModel newOrModifiedData, TDbTable dbData, long dbId, EnumDbCacheElementOperations operation, Func<TDb, long, Task> deleteFileAction = null!)
    {
        var modelProps = typeof(TModel).GetProperties();
        var dbProps = typeof(TDbTable).GetProperties();

        foreach (var prop in dbProps)
        {
            if (prop.CanWrite)
            {
                var modelProp = modelProps.FirstOrDefault(f => string.Equals(f.Name, prop.Name, StringComparison.InvariantCultureIgnoreCase));
                if (modelProp != null!)
                {
                    try
                    {
                        prop.SetValue(dbData, modelProp.GetValue(newOrModifiedData));
                    }
                    catch (Exception e)
                    {
                        Logging.Log.LogWarning($"[{GetType().Name}]({nameof(StoreElement)}): Can not set property {modelProp.Name}: {e}");
                    }
                }
            }
        }


        return Task.FromResult(dbData);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="db"></param>
    /// <param name="userId"></param>
    /// <param name="deviceId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    protected override Task<List<DcServerListItem<TModel>>> ModifyDataForUserOrDevice(TDb db, long userId, long deviceId, List<DcServerListItem<TModel>> data)
    {
        return Task.FromResult(data);
    }
}