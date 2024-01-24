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
    /// <para>Bewertungen</para>
    /// Klasse DcExRatings. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public partial class ServerRemoteCalls
    {
        #region Interface Implementations

        /// <summary>
        /// Device fordert Listen Daten für DcExRatings
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
        public async Task<List<DcServerListItem<ExRating>>> GetDcExRatings(long deviceId, long userId, long startIndex, long elementsToRead, long secondId, string filter)
        {
            var result = new List<DcServerListItem<ExRating>>();

            await using var db = new Db();

            var tblRatings = await db.TblRatings
                .Include(x => x.TblUser)
                .Include(x => x.TblMeasurementDefinitionAssignment)
                .ThenInclude(x => x.TblMeasurementDefinition)
                //.ThenInclude(x => x.Information)
                //.Include(x => x.TblMeasurementDefinitionAssignment)
                //.ThenInclude(x => x.TblIoTDevice)
                //.ThenInclude(x => x.TblGateway)
                //.Where(x => x.TblUserId == userId)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var tblRating in tblRatings)
            {
                var item = new DcServerListItem<ExRating>
                           {
                               Data = tblRating.ToExRating(),
                               SortIndex = tblRating.Id,
                               Index = tblRating.Id,
#pragma warning disable CS0618 // Type or member is obsolete
                               SecondId = secondId
#pragma warning restore CS0618 // Type or member is obsolete
                           };

                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Device will Listen Daten für DcExRatings sichern
        /// </summary>
        /// <param name="deviceId">Id des Gerätes</param>
        /// <param name="userId">Id des Benutzers oder -1 wenn nicht angemeldet</param>
        /// <param name="data">Eingetliche Daten</param>
        /// <param name="secondId">
        /// Optionale 2te Id um schnellen Wechsel zwischen Listen zu ermöglichen bzw. dynamische Listen. Zb.
        /// für Chats
        /// </param>
        /// <returns>Ergebnis (bzw. Infos zum Fehler)</returns>
        public async Task<DcListStoreResult> StoreDcExRatings(long deviceId, long userId, List<DcStoreListItem<ExRating>> data, long secondId)
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

            var syncResultData = new DcListSyncResultData<ExRating>();
            var syncResultDataMeasurementDefinition = new DcListSyncResultData<ExMeasurementDefinitionAssignment>();
            var syncResultDataAbo = new DcListSyncResultData<ExAbo>();

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await using var db = new Db();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
            var actingUser = await db.TblUsers.FirstAsync(x => x.Id == userId).ConfigureAwait(false);

            foreach (var d in data)
            {
                var tblMeasurementDefAssignment = await db.TblMeasurementDefinitionAssignments
                    .Include(x => x.TblMeasurementDefinition)
                    .ThenInclude(x => x.Information)
                    .Include(x => x.TblMeasurementDefinition)
                    .ThenInclude(x => x.TblIoTDevice)
                    .ThenInclude(x => x.TblGateway)
                    .Include(x => x.TblAbos)
                    .ThenInclude(a => a.TblUser)
                    .Include(x => x.TblRatings)
                    .ThenInclude(r => r.TblUser)
                    .Include(x => x.TblNotifications)
                    .FirstAsync(x => x.Id == d.Data.MeasurementDefinitionAssignment.Id)
                    .ConfigureAwait(false);

                // ReSharper disable once RedundantAssignment
                TableRating tblRating = null!;

                var tmp = new DcListStoreResultIndexAndData();
                switch (d.State)
                {
                    case EnumDcListElementState.New:

                        tblRating = new TableRating
                                    {
                                        TblUser = actingUser,
                                    };
                        tmp.BeforeStoreIndex = d.Index;
                        result.ElementsStored++;
                        tblRating.UpdateTableRating(d.Data);
                        tblMeasurementDefAssignment.TblRatings.Add(tblRating);
                        await db.SaveChangesAsync().ConfigureAwait(false);
                        tmp.NewIndex = tmp.NewSortIndex = tblRating.Id;
                        result.NewIndex.Add(tmp);
                        break;
                    case EnumDcListElementState.Modified:
                        tblRating = tblMeasurementDefAssignment.TblRatings.First(x => x.Id == d.Index);
                        result.ElementsStored++;
                        tblRating.UpdateTableRating(d.Data);
                        break;
                    case EnumDcListElementState.Deleted:
                        tblRating = db.TblRatings.First(f => f.Id == d.Index);
                        db.TblRatings.Remove(tblRating);
                        syncResultData.ItemsToRemoveOnClient.Add(tblRating.Id);

                        break;
                    case EnumDcListElementState.None:
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (d.State is EnumDcListElementState.New or EnumDcListElementState.Modified)
                {
                    syncResultData.NewOrModifiedItems.Add(new DcServerListItem<ExRating>
                                                          {
                                                              Data = tblRating.ToExRating(),
                                                              SortIndex = tblRating.Id,
                                                              Index = tblRating.Id,
#pragma warning disable CS0618 // Type or member is obsolete
                                                              SecondId = secondId
#pragma warning restore CS0618 // Type or member is obsolete
                                                          });
                }

                syncResultDataMeasurementDefinition.NewOrModifiedItems.Add(new DcServerListItem<ExMeasurementDefinitionAssignment>
                                                                           {
                                                                               Data = tblMeasurementDefAssignment.ToExMeasurementDefinitionAssignment(),
                                                                               SortIndex = tblMeasurementDefAssignment.Id,
                                                                               Index = tblMeasurementDefAssignment.Id,
                                                                           });

                foreach (var tblAbo in tblMeasurementDefAssignment.TblAbos)
                {
                    syncResultDataAbo.NewOrModifiedItems.Add(new DcServerListItem<ExAbo>
                                                             {
                                                                 Data = tblAbo.ToExAbo(),
                                                                 SortIndex = tblAbo.Id,
                                                                 Index = tblAbo.Id,
                                                             });
                }
            }

            await db.SaveChangesAsync().ConfigureAwait(false);

            //DcExRatings syncen
            // ReSharper disable once UnusedVariable
            var r1 = await SyncDcExRatings(syncResultData).ConfigureAwait(false);
            //DcExMeasurementDefinition syncen
            // ReSharper disable once UnusedVariable
            var r2 = await SyncDcExMeasurementDefinitionAssignment(syncResultDataMeasurementDefinition).ConfigureAwait(false);
            //DcExAbos syncen
            // ReSharper disable once UnusedVariable
            var r3 = await SyncDcExAbos(syncResultDataAbo).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Daten Synchronisieren für DcExRatings
        /// </summary>
        /// <param name="deviceId">device</param>
        /// <param name="userId">User Id oder -1 wenn nicht angemeldet</param>
        /// <param name="current">Aktuelle Datensätze am Gerät</param>
        /// <param name="props">Zusätzliche Optionen</param>
        /// <returns>Neuer, aktualisierte und gelöschte Datensätze</returns>
        public async Task<DcListSyncResultData<ExRating>> SyncDcExRatings(long deviceId, long userId, DcListSyncData current, DcListSyncProperties props)
        {
            await using var db = new Db();

            var ratings = db.TblRatings
                .Include(x => x.TblUser)
                .Include(x => x.TblMeasurementDefinitionAssignment)
                .ThenInclude(x => x.TblMeasurementDefinition)
                .ThenInclude(x => x.Information)
                .Include(x => x.TblMeasurementDefinitionAssignment)
                .ThenInclude(x => x.TblMeasurementDefinition)
                .ThenInclude(x => x.TblIoTDevice)
                .ThenInclude(x => x.TblGateway)
                .AsNoTracking();
            var r = await _cc.DbCacheRatings.GetSyncData(db, ratings, current, userId, deviceId).ConfigureAwait(true);
            r.ServerItemCount = await db.TblRatings.AsNoTracking().CountAsync().ConfigureAwait(false);
            return r;
        }

        #endregion
    }
}