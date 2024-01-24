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
    /// <para>Dc für Notifications</para>
    /// Klasse DcExNotifications. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public partial class ServerRemoteCalls
    {
        #region Interface Implementations

        /// <summary>
        /// Device fordert Listen Daten für DcExNotifications
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
        public async Task<List<DcServerListItem<ExNotification>>> GetDcExNotifications(long deviceId, long userId, long startIndex, long elementsToRead, long secondId, string filter)
        {
            var result = new List<DcServerListItem<ExNotification>>();

#pragma warning disable CA2007
            await using var db = new Db();
#pragma warning restore CA2007

            var tblNotifications = await db.TblNotifications
                .Include(x => x.TblUser)
                .Include(x => x.TblMeasurementDefinitionAssignment)
                .ThenInclude(x => x.TblMeasurementDefinition)
                .ThenInclude(x => x.Information)
                .Include(x => x.TblMeasurementDefinitionAssignment)
                .ThenInclude(x => x.TblMeasurementDefinition)
                .ThenInclude(x => x.TblIoTDevice)
                .ThenInclude(x => x.TblGateway)
                .ThenInclude(x => x!.TblCompany)
                .Where(x => x.TblUserId == userId)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var tblNotification in tblNotifications)
            {
                var item = new DcServerListItem<ExNotification>
                           {
                               Data = tblNotification.ToExNotification(),
                               SortIndex = tblNotification.Id,
                               Index = tblNotification.Id,
#pragma warning disable CS0618 // Type or member is obsolete
                               SecondId = secondId
#pragma warning restore CS0618 // Type or member is obsolete
                           };

                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Device will Listen Daten für DcExNotifications sichern
        /// </summary>
        /// <param name="deviceId">Id des Gerätes</param>
        /// <param name="userId">Id des Benutzers oder -1 wenn nicht angemeldet</param>
        /// <param name="data">Eingetliche Daten</param>
        /// <param name="secondId">
        /// Optionale 2te Id um schnellen Wechsel zwischen Listen zu ermöglichen bzw. dynamische Listen. Zb.
        /// für Chats
        /// </param>
        /// <returns>Ergebnis (bzw. Infos zum Fehler)</returns>
        public async Task<DcListStoreResult> StoreDcExNotifications(long deviceId, long userId, List<DcStoreListItem<ExNotification>> data, long secondId)
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

            var syncResultData = new DcListSyncResultData<ExNotification>();

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await using var db = new Db();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
            var actingUser = await db.TblUsers.FirstAsync(x => x.Id == userId).ConfigureAwait(false);

            foreach (var d in data)
            {
                // ReSharper disable once RedundantAssignment
                TableNotification tblNotification = null!;

                var tmp = new DcListStoreResultIndexAndData();
                switch (d.State)
                {
                    case EnumDcListElementState.New:
                        tblNotification = new TableNotification
                                          {
                                              TblUser = actingUser,
                                              TblMeasurementDefinitionAssignmentId = d.Data.MeasurementDefinitionAssignment.Id,
                                          };
                        tmp.BeforeStoreIndex = d.Index;
                        result.ElementsStored++;
                        tblNotification.UpdateTableNotification(d.Data);
                        await db.TblNotifications.AddAsync(tblNotification).ConfigureAwait(false);
                        await db.SaveChangesAsync().ConfigureAwait(false);
                        tmp.NewIndex = tmp.NewSortIndex = tblNotification.Id;
                        result.NewIndex.Add(tmp);
                        break;
                    case EnumDcListElementState.Modified:
                        tblNotification = await db.TblNotifications
                            .Include(x => x.TblUser)
                            .Include(x => x.TblMeasurementDefinitionAssignment)
                            .ThenInclude(m => m.TblMeasurementDefinition)
                            .ThenInclude(x => x.Information)
                            .Include(x => x.TblMeasurementDefinitionAssignment)
                            .ThenInclude(m => m.TblMeasurementDefinition)
                            .ThenInclude(x => x.TblIoTDevice)
                            .ThenInclude(x => x.TblGateway)
                            .FirstAsync(f => f.Id == d.Index)
                            .ConfigureAwait(false);
                        result.ElementsStored++;
                        tblNotification.UpdateTableNotification(d.Data);
                        break;
                    case EnumDcListElementState.Deleted:
                        tblNotification = db.TblNotifications.First(f => f.Id == d.Index);
                        db.TblNotifications.Remove(tblNotification);
                        syncResultData.ItemsToRemoveOnClient.Add(tblNotification.Id);
                        break;
                    case EnumDcListElementState.None:
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (d.State is EnumDcListElementState.New or EnumDcListElementState.Modified)
                {
                    syncResultData.NewOrModifiedItems.Add(new DcServerListItem<ExNotification>
                                                          {
                                                              Data = tblNotification.ToExNotification(),
                                                              SortIndex = tblNotification.Id,
                                                              Index = tblNotification.Id,
#pragma warning disable CS0618 // Type or member is obsolete
                                                              SecondId = secondId
#pragma warning restore CS0618 // Type or member is obsolete
                                                          });
                }
            }

            await db.SaveChangesAsync().ConfigureAwait(false);

            await SyncDcExNotifications(syncResultData).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Daten Synchronisieren für DcExNotifications
        /// </summary>
        /// <param name="deviceId">Gerät</param>
        /// <param name="userId">User Id oder -1 wenn nicht angemeldet</param>
        /// <param name="current">Aktuelle Datensätze am Gerät</param>
        /// <param name="props">Zusätzliche Optionen</param>
        /// <returns>Neuer, aktualisierte und gelöschte Datensätze</returns>
        public Task<DcListSyncResultData<ExNotification>> SyncDcExNotifications(long deviceId, long userId, DcListSyncData current, DcListSyncProperties props)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}