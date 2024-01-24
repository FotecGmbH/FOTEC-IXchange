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
using System.Linq;
using System.Threading.Tasks;
using Biss.Dc.Core;
using Exchange.Model.ConfigApp;
using IXchangeDatabase;
using IXchangeDatabase.Converter;
using IXchangeDatabase.Tables;
using Microsoft.EntityFrameworkCore;


namespace IXchange.Service.AppConnectivity.DataConnector;

/// <summary>
///     <para>Datenaustausch für DcExCompanies</para>
/// Klasse DcExCompanies. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
/// </summary>
public partial class ServerRemoteCalls
{
    #region Interface Implementations

    /// <summary>
    /// Device fordert Listen Daten für ExMeasurementDefinitionAssignment
    /// </summary>
    /// <param name="deviceId">Id des Gerätes</param>
    /// <param name="userId">Id des Benutzers oder -1 wenn nicht angemeldet</param>
    /// <param name="startIndex">Lesen ab Index (-1 für Start)</param>
    /// <param name="elementsToRead">Anzahl der Elemente welche maximal gelesen werden sollen (-1 für alle verfügbaren Daten)</param>
    /// <param name="secondId">
    /// Optionale 2te Id um schnellen Wechsel zwischen Listen zu ermöglichen bzw. dynamische Listen. Zb.
    /// für Chats
    /// </param>
    /// <param name="filter">Optionaler Filter für die Daten</param>
    /// <returns>Daten oder eine Exception auslösen</returns>
    public async Task<List<DcServerListItem<ExMeasurementDefinitionAssignment>>> GetDcExMeasurementDefinitionAssignments(long deviceId, long userId, long startIndex, long elementsToRead, long secondId, string filter)
    {
        var result = new List<DcServerListItem<ExMeasurementDefinitionAssignment>>();

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var db = new Db();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        foreach (var md in db.GetMeasurementDefinitionAssignments())
        {
            try
            {
                var d = new DcServerListItem<ExMeasurementDefinitionAssignment>
                        {
                            Data = md.ToExMeasurementDefinitionAssignment(),
                            SortIndex = md.Id,
                            Index = md.Id,
#pragma warning disable CS0618 // Type or member is obsolete
                            SecondId = secondId
#pragma warning restore CS0618 // Type or member is obsolete
                        };

                result.Add(d);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return result;
    }

    /// <summary>
    /// Device will Listen Daten für ExMeasurementDefinitionAssignment sichern
    /// </summary>
    /// <param name="deviceId">Id des Gerätes</param>
    /// <param name="userId">Id des Benutzers oder -1 wenn nicht angemeldet</param>
    /// <param name="data">Eingetliche Daten</param>
    /// <param name="secondId">
    /// Optionale 2te Id um schnellen Wechsel zwischen Listen zu ermöglichen bzw. dynamische Listen. Zb.
    /// für Chats
    /// </param>
    /// <returns>Ergebnis (bzw. Infos zum Fehler)</returns>
    public async Task<DcListStoreResult> StoreDcExMeasurementDefinitionAssignments(long deviceId, long userId, List<DcStoreListItem<ExMeasurementDefinitionAssignment>> data, long secondId)
    {
        if (data == null!)
        {
            throw new ArgumentNullException(nameof(data));
        }

        var r = new DcListStoreResult
                {
                    SecondId = secondId,
                    StoreResult = new(),
                    ElementsStored = new()
                };

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var db = new Db();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

#pragma warning disable CS0219 // Variable is assigned but its value is never used
        var anyDelete = false;
#pragma warning restore CS0219 // Variable is assigned but its value is never used
        // ReSharper disable once UnusedVariable
        var modifiedIotDevices = new List<long>();
        // ReSharper disable once CollectionNeverQueried.Local
        var modifiedMeasurementDefinitions = new List<long>();

        foreach (var d in data)
        {
            // ReSharper disable once RedundantAssignment
            TableMeasurementDefinitionAssignment c = null!;
            var tmp = new DcListStoreResultIndexAndData();
            switch (d.State)
            {
                case EnumDcListElementState.New:
                    c = new TableMeasurementDefinitionAssignment();

                    tmp.BeforeStoreIndex = d.Index;
                    r.ElementsStored++;
                    break;
                case EnumDcListElementState.Modified:
                    c = db.TblMeasurementDefinitionAssignments.First(f => f.Id == d.Index);
                    r.ElementsStored++;
                    break;
                case EnumDcListElementState.Deleted:
                    c = db.TblMeasurementDefinitionAssignments.Where(f => f.Id == d.Index).Include(i => i.TblRatings).Include(i => i.TblNotifications).Include(i => i.TblAbos).First();
                    break;
                case EnumDcListElementState.None:
                    continue;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (d.State == EnumDcListElementState.Deleted)
            {
                // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
                db.TblRatings.RemoveRange(c.TblRatings);
                // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
                db.TblNotifications.RemoveRange(c.TblNotifications);
                // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
                db.TblAbos.RemoveRange(c.TblAbos);
                db.TblMeasurementDefinitionAssignments.Remove(c);
                // ReSharper disable once RedundantAssignment
                anyDelete = true;
            }
            else
            {
                if (d.Data.MeasurementDefinition != null!)
                {
                    var mesDef = db.TblMeasurementDefinitions.FirstOrDefault(md => md.Id == d.Data.MeasurementDefinition.Id);
                    if (mesDef != null)
                    {
                        c.TblMeasurementDefinition = mesDef;
                    }

                    if (d.State == EnumDcListElementState.New && mesDef != null)
                    {
                        try
                        {
                            if (!db.TblMeasurementDefinitionAssignments.Any(mA => mA.TblMeasurementDefinitionId == mesDef.Id))
                            {
                                db.TblMeasurementDefinitionAssignments.Add(c);
                            }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                }

                d.Data.ToTableMeasurementDefinitionAssignment(c);
            }

            //modifiedIotDevices.Add(c.TblIotDeviceId);

            await db.SaveChangesAsync().ConfigureAwait(true);
            if (d.State == EnumDcListElementState.New)
            {
                tmp.NewIndex = c.Id;
                tmp.NewSortIndex = c.Id;
                r.NewIndex.Add(tmp);
            }

            modifiedMeasurementDefinitions.Add(c.Id);
        }

        //modifiedIotDevices = modifiedIotDevices.Distinct().ToList();
        //long gatewayId = -1;
        //foreach (var iotDevice in modifiedIotDevices)
        //{
        //    var iot = db.TblIotDevices.First(f => f.Id == iotDevice);
        //    iot.DeviceCommon.ConfigversionService++;
        //    gatewayId = iot.TblGatewayId!.Value;
        //}

        await db.SaveChangesAsync().ConfigureAwait(true);

        //_ = Task.Run(async () =>
        //{
        //    await Task.Delay(300).ConfigureAwait(true);

        //    if (anyDelete)
        //    {
        //        await SendReloadList(EnumReloadDcList.ExMeasurementDefinition).ConfigureAwait(false);
        //    }
        //    else
        //    {
        //        await MeasurementDefinitionDataChanged(modifiedMeasurementDefinitions, gatewayId).ConfigureAwait(false);
        //    }

        //    foreach (var iotDevice in modifiedIotDevices)
        //    {
        //        await IotDeviceDataChanged(iotDevice).ConfigureAwait(false);
        //    }

        //    TriggerAgent.ChangedGateway(EnumTriggerSources.ServiceAppConnectivity, gatewayId);
        //});

        return r;
    }

    /// <summary>
    /// Daten Synchronisieren für ExMeasurementDefinitionAssignment
    /// </summary>
    /// <param name="deviceId">Gerät</param>
    /// <param name="userId">User Id oder -1 wenn nicht angemeldet</param>
    /// <param name="current">Aktuelle Datensätze am Gerät</param>
    /// <param name="props">Zusätzliche Optionen</param>
    /// <returns>Neuer, aktualisierte und gelöschte Datensätze</returns>
    public Task<DcListSyncResultData<ExMeasurementDefinitionAssignment>> SyncDcExMeasurementDefinitionAssignments(long deviceId, long userId, DcListSyncData current, DcListSyncProperties props)
    {
        return Task.FromResult(new DcListSyncResultData<ExMeasurementDefinitionAssignment>());
//        await using var db = new Db();

//        var result = new DcListSyncResultData<ExMeasurementDefinitionAssignment>();// List<DcServerListItem<ExMeasurementDefinitionAssignment>>();
//        result.
//#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
//        await using var db = new Db();
//#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

//        foreach (var md in db.GetMeasurementDefinitionAssignments())
//        {
//            try
//            {
//                if (md.Id == 0)
//                {
//                    var ooo = 0;
//                }
//                var d = new DcServerListItem<ExMeasurementDefinitionAssignment>
//                        {
//                            Data = ConverterDbMeasurementDefinitionAssignment.ToExMeasurementDefinitionAssignment(md),
//                            SortIndex = md.Id,
//                            Index = md.Id,
//                            SecondId = secondId
//                        };
//                if (d.Data.Id == 0)
//                {
//                    var gg = 0;
//                }

//                result.Add(d);
//            }
//            catch (Exception e)
//            {
//                var uu = 0;
//            }

//        }

//        return result;

        //var tblMeasurementDefinitions = db.GetMeasurementDefinitionAssignments();
        //throw new NotImplementedException();
        //var r = await _cc.DbCacheMeasurementDefinitions.GetSyncData(db, tblMeasurementDefinitions, current, userId, deviceId).ConfigureAwait(true);
        //r.ServerItemCount = await tblMeasurementDefinitions.CountAsync().ConfigureAwait(false);
        //return r;
    }

    #endregion
}