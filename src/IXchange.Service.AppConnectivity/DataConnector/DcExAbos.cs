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


namespace IXchange.Service.AppConnectivity.DataConnector
{
    /// <summary>
    /// <para>Dc für Abos</para>
    /// Klasse DcExAbos. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public partial class ServerRemoteCalls
    {
        private async Task<List<TableAbo>> GetAbosOfUser(Db db, long userId)
        {
            return await db.TblAbos
                .AsNoTracking()
                .Include(x => x.TblUser)
                .Include(x => x.TblMeasurementDefinitionAssignment)
                //.ThenInclude(x => x.Information)
                .Include(x => x.TblMeasurementDefinitionAssignment)
                .ThenInclude(x => x.TblMeasurementDefinition)
                //.ThenInclude(x => x.TblIoTDevice)
                //.ThenInclude(x => x.TblGateway)
                //.ThenInclude(x=>x.TblCompany)
                //.ThenInclude(x=>x.TblPermissions)
                .Include(x => x.TblMeasurementDefinitionAssignment)
                .ThenInclude(x => x.TblRatings)
                .ThenInclude(x => x.TblUser)
                .Where(x => x.TblUserId == userId)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        private async Task<TableAbo> GetAbo(Db db, long id)
        {
            return await db.TblAbos
                .Include(x => x.TblUser)
                .Include(x => x.TblMeasurementDefinitionAssignment)
                .ThenInclude(x => x.TblMeasurementDefinition)
                .ThenInclude(x => x.TblIoTDevice)
                .ThenInclude(x => x.TblGateway)
                .ThenInclude(x => x!.TblCompany)
                .Include(x => x.TblMeasurementDefinitionAssignment)
                .ThenInclude(x => x.TblMeasurementDefinition)
                .ThenInclude(x => x.Information)
                //.ThenInclude(x => x.Information)
                //.Include(x => x.TblMeasurementDefinitionAssignment)
                //.ThenInclude(x => x.TblIoTDevice)
                //.ThenInclude(x => x.TblGateway)
                .FirstAsync(f => f.Id == id)
                .ConfigureAwait(false);
        }

        #region Interface Implementations

        /// <summary>
        /// Device fordert Listen Daten für DcExAbos
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
        public async Task<List<DcServerListItem<ExAbo>>> GetDcExAbos(long deviceId, long userId, long startIndex, long elementsToRead, long secondId, string filter)
        {
            var result = new List<DcServerListItem<ExAbo>>();

            await using var db = new Db();

            var tblAbos = await GetAbosOfUser(db, userId).ConfigureAwait(false);

            foreach (var tblAbo in tblAbos)
            {
                var item = new DcServerListItem<ExAbo>
                           {
                               Data = tblAbo.ToExAbo(),
                               SortIndex = tblAbo.Id,
                               Index = tblAbo.Id,
#pragma warning disable CS0618 // Type or member is obsolete
                               SecondId = secondId
#pragma warning restore CS0618 // Type or member is obsolete
                           };

                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Device will Listen Daten für DcExAbos sichern
        /// </summary>
        /// <param name="deviceId">Id des Gerätes</param>
        /// <param name="userId">Id des Benutzers oder -1 wenn nicht angemeldet</param>
        /// <param name="data">Eingetliche Daten</param>
        /// <param name="secondId">
        /// Optionale 2te Id um schnellen Wechsel zwischen Listen zu ermöglichen bzw. dynamische Listen. Zb.
        /// für Chats
        /// </param>
        /// <returns>Ergebnis (bzw. Infos zum Fehler)</returns>
        public async Task<DcListStoreResult> StoreDcExAbos(long deviceId, long userId, List<DcStoreListItem<ExAbo>> data, long secondId)
        {
            if (data == null!)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var result = new DcListStoreResult
                         {
                             SecondId = secondId,
                             StoreResult = new(),
                             ElementsStored = new()
                         };

            var syncResultData = new DcListSyncResultData<ExAbo>();

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await using var db = new Db();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
            var actingUser = await db.TblUsers.FirstAsync(x => x.Id == userId).ConfigureAwait(false);

            foreach (var d in data)
            {
                // ReSharper disable once RedundantAssignment
                TableAbo tblAbo = null!;

                var tmp = new DcListStoreResultIndexAndData();
                switch (d.State)
                {
                    case EnumDcListElementState.New:
                        tblAbo = new TableAbo
                                 {
                                     TblUser = actingUser,
                                     TblMeasurementDefinitionAssignmentId = d.Data.MeasurementDefinitionAssignment.Id,
                                 };
                        tmp.BeforeStoreIndex = d.Index;
                        result.ElementsStored++;
                        tblAbo.UpdateTableAbo(d.Data);
                        await db.TblAbos.AddAsync(tblAbo).ConfigureAwait(false);
                        await db.SaveChangesAsync().ConfigureAwait(false);

                        tmp.NewIndex = tmp.NewSortIndex = tblAbo.Id;
                        result.NewIndex.Add(tmp);
                        break;
                    case EnumDcListElementState.Modified:
                        tblAbo = await GetAbo(db, d.Index).ConfigureAwait(false);
                        result.ElementsStored++;
                        tblAbo.UpdateTableAbo(d.Data);
                        await db.SaveChangesAsync().ConfigureAwait(false);
                        break;
                    case EnumDcListElementState.Deleted:
                        tblAbo = db.TblAbos.First(f => f.Id == d.Index);
                        db.TblAbos.Remove(tblAbo);
                        await db.SaveChangesAsync().ConfigureAwait(false);
                        syncResultData.ItemsToRemoveOnClient.Add(tblAbo.Id);
                        break;
                    case EnumDcListElementState.None:
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (d.State is EnumDcListElementState.New or EnumDcListElementState.Modified)
                {
                    if (d.State is EnumDcListElementState.New)
                    {
                        tblAbo = await GetAbo(db, tblAbo.Id).ConfigureAwait(false);
                    }

                    syncResultData.NewOrModifiedItems.Add(new DcServerListItem<ExAbo>
                                                          {
                                                              Data = tblAbo.ToExAbo(),
                                                              SortIndex = tblAbo.Id,
                                                              Index = tblAbo.Id,
#pragma warning disable CS0618 // Type or member is obsolete
                                                              SecondId = secondId
#pragma warning restore CS0618 // Type or member is obsolete
                                                          });
                }
            }

            await SyncDcExAbos(syncResultData).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Daten Synchronisieren für DcExAbos
        /// </summary>
        /// <param name="deviceId">Gerät</param>
        /// <param name="userId">User Id oder -1 wenn nicht angemeldet</param>
        /// <param name="current">Aktuelle Datensätze am Gerät</param>
        /// <param name="props">Zusätzliche Optionen</param>
        /// <returns>Neuer, aktualisierte und gelöschte Datensätze</returns>
        public async Task<DcListSyncResultData<ExAbo>> SyncDcExAbos(long deviceId, long userId, DcListSyncData current, DcListSyncProperties props)
        {
            await using var db = new Db();

            var abos = db.TblAbos
                .Include(x => x.TblUser)
                .Include(x => x.TblMeasurementDefinitionAssignment)
                .ThenInclude(m => m.TblMeasurementDefinition)
                .ThenInclude(x => x.Information)
                .Include(x => x.TblMeasurementDefinitionAssignment)
                .ThenInclude(m => m.TblMeasurementDefinition)
                .ThenInclude(x => x.TblIoTDevice)
                .ThenInclude(x => x.TblGateway)
                .ThenInclude(x => x!.TblCompany)
                .Include(x => x.TblMeasurementDefinitionAssignment)
                .ThenInclude(x => x.TblRatings)
                .ThenInclude(x => x.TblUser)
                .Where(x => x.TblUserId == userId)
                .AsNoTracking();
            var r = await _cc.DbCacheAbos.GetSyncData(db, abos, current, userId, deviceId).ConfigureAwait(true);
            r.ServerItemCount = await db.TblAbos.AsNoTracking().CountAsync().ConfigureAwait(false);
            return r;
        }

        #endregion
    }
}