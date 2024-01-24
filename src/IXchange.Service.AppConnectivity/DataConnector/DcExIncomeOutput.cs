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
using Exchange.Enum;
using Exchange.Model.ConfigApp;
using IXchangeDatabase;
using IXchangeDatabase.Converter;
using IXchangeDatabase.Tables;
using Microsoft.EntityFrameworkCore;


namespace IXchange.Service.AppConnectivity.DataConnector
{
    /// <summary>
    /// <para>Dc für Einnahmen/Ausgaben</para>
    /// Klasse DcExIncomeOutput. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public partial class ServerRemoteCalls
    {
        private async Task<TableIncomeOutput> GetIncomeOutput(Db db, long id)
        {
            return await db.TblIncomeOutputs
                .Include(x => x.TblUser)
                .Include(x => x.TblMeasurementDefinition)
                .ThenInclude(x => x!.Information)
                .FirstAsync(f => f.Id == id)
                .ConfigureAwait(false);
        }

        #region Interface Implementations

        /// <summary>
        /// Device fordert Listen Daten für DcExIncomeOutput
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
        public async Task<List<DcServerListItem<ExIncomeOutput>>> GetDcExIncomeOutput(long deviceId, long userId, long startIndex, long elementsToRead, long secondId, string filter)
        {
            var result = new List<DcServerListItem<ExIncomeOutput>>();

            await using var db = new Db();

            var tblIncomeOutputs = await db.TblIncomeOutputs
                .Where(x => x.TblUserId == userId)
                .Include(x => x.TblUser)
                .Include(x => x.TblMeasurementDefinition)
                .ThenInclude(x => x!.Information)
                .OrderByDescending(x => x.TimeStamp)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var tblIncomeOutput in tblIncomeOutputs)
            {
                var item = new DcServerListItem<ExIncomeOutput>
                           {
                               Data = tblIncomeOutput.ToExIncomeOutput(),
                               SortIndex = tblIncomeOutput.Id,
                               Index = tblIncomeOutput.Id,
#pragma warning disable CS0618 // Type or member is obsolete
                               SecondId = secondId
#pragma warning restore CS0618 // Type or member is obsolete
                           };

                result.Add(item);
            }

            return result.OrderByDescending(x => x.Data.TimeStamp).ToList();
        }

        /// <summary>
        /// Device will Listen Daten für DcExIncomeOutput sichern
        /// </summary>
        /// <param name="deviceId">Id des Gerätes</param>
        /// <param name="userId">Id des Benutzers oder -1 wenn nicht angemeldet</param>
        /// <param name="data">Eingetliche Daten</param>
        /// <param name="secondId">
        /// Optionale 2te Id um schnellen Wechsel zwischen Listen zu ermöglichen bzw. dynamische Listen. Zb.
        /// für Chats
        /// </param>
        /// <returns>Ergebnis (bzw. Infos zum Fehler)</returns>
        public async Task<DcListStoreResult> StoreDcExIncomeOutput(long deviceId, long userId, List<DcStoreListItem<ExIncomeOutput>> data, long secondId)
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

            var syncResultData = new DcListSyncResultData<ExIncomeOutput>();

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await using var db = new Db();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
            // ReSharper disable once UnusedVariable
            var actingUser = await db.TblUsers.FirstAsync(x => x.Id == userId).ConfigureAwait(false);

            foreach (var d in data)
            {
                // ReSharper disable once RedundantAssignment
                TableIncomeOutput tblIncomeOutput = null!;

                var tmp = new DcListStoreResultIndexAndData();
                switch (d.State)
                {
                    case EnumDcListElementState.New:
                        var previousTotal = 0;
                        var previous = db.TblIncomeOutputs.Where(io => io.TblUserId == d.Data.UserId);
                        if (previous.Any())
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        {
                            previousTotal = previous.MaxBy(a => a.TimeStamp).CurrentTotalIxies;
                        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                        tblIncomeOutput = new TableIncomeOutput
                                          {
                                              TblMeasurementDefinitonId = d.Data.MeasurementDefinitionId,
                                              Option = d.Data.Option,
                                              TblUserId = d.Data.UserId,
                                              Ixies = d.Data.Ixies,
                                              CurrentTotalIxies = previousTotal + d.Data.Ixies
                                          };
                        tmp.BeforeStoreIndex = d.Index;
                        result.ElementsStored++;
                        tblIncomeOutput.UpdateTableIncomeOutput(d.Data);
                        await db.TblIncomeOutputs.AddAsync(tblIncomeOutput).ConfigureAwait(false);
                        await db.SaveChangesAsync().ConfigureAwait(false);
                        tmp.NewIndex = tmp.NewSortIndex = tblIncomeOutput.Id;
                        result.NewIndex.Add(tmp);
                        break;
                    case EnumDcListElementState.Modified:
                        throw new NotImplementedException(); // wie blockchain, aenderungen sind nicht erwuenscht
                    case EnumDcListElementState.Deleted:
                        throw new NotImplementedException(); // wie blockchain, aenderungen sind nicht erwuenscht
                    case EnumDcListElementState.None:
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (d.State is EnumDcListElementState.New && d.Data.Option == EnumIncomeOutputOption.Abo)
                {
                    var currM = db.TblMeasurementDefinitions
                        .Include(m => m.TblIoTDevice)
                        .ThenInclude(x => x.TblGateway)
                        .ThenInclude(x => x!.TblCompany)
                        .ThenInclude(x => x.TblPermissions)
                        .FirstOrDefault(x => x.Id == d.Data.MeasurementDefinitionId);

                    long userIdForMeasurementDefinition = -1;

                    try
                    {
                        userIdForMeasurementDefinition = currM!.TblIoTDevice.TblGateway!.TblCompany.TblPermissions.FirstOrDefault()!.TblUserId;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    if (userIdForMeasurementDefinition != -1)
                    {
                        tblIncomeOutput = new TableIncomeOutput
                                          {
                                              TblMeasurementDefinitonId = d.Data.MeasurementDefinitionId,
                                              Option = d.Data.Option,
                                              TblUserId = userIdForMeasurementDefinition,
                                              Ixies = -d.Data.Ixies - 1, // besitzer bekommt ixies, aber 1 weniger
                                              TimeStamp = DateTime.UtcNow
                                          };
                        tmp.BeforeStoreIndex = d.Index;
                        result.ElementsStored++;
                        await db.TblIncomeOutputs.AddAsync(tblIncomeOutput).ConfigureAwait(false);
                        await db.SaveChangesAsync().ConfigureAwait(false);
                    }
                }

                if (d.State is EnumDcListElementState.New)
                {
                    tblIncomeOutput = await GetIncomeOutput(db, tblIncomeOutput.Id).ConfigureAwait(false);

                    syncResultData.NewOrModifiedItems.Add(new DcServerListItem<ExIncomeOutput>
                                                          {
                                                              Data = tblIncomeOutput.ToExIncomeOutput(),
                                                              SortIndex = tblIncomeOutput.Id,
                                                              Index = tblIncomeOutput.Id,
#pragma warning disable CS0618 // Type or member is obsolete
                                                              SecondId = secondId
#pragma warning restore CS0618 // Type or member is obsolete
                                                          });
                }
            }

            await SyncDcExIncomeOutput(syncResultData).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Daten Synchronisieren für DcExIncomeOutput
        /// </summary>
        /// <param name="deviceId">Gerät</param>
        /// <param name="userId">User Id oder -1 wenn nicht angemeldet</param>
        /// <param name="current">Aktuelle Datensätze am Gerät</param>
        /// <param name="props">Zusätzliche Optionen</param>
        /// <returns>Neuer, aktualisierte und gelöschte Datensätze</returns>
        public async Task<DcListSyncResultData<ExIncomeOutput>> SyncDcExIncomeOutput(long deviceId, long userId, DcListSyncData current, DcListSyncProperties props)
        {
            await using var db = new Db();

            var tblIncomeOutputs = db.TblIncomeOutputs
                .Where(x => x.TblUserId == userId)
                .Include(x => x.TblUser)
                .Include(x => x.TblMeasurementDefinition)
                .ThenInclude(x => x!.Information)
                .OrderByDescending(x => x.TimeStamp)
                .AsNoTracking();

            var r = await _cc.DbCacheIncomeOutputs.GetSyncData(db, tblIncomeOutputs, current, userId, deviceId).ConfigureAwait(true);
            r.ServerItemCount = await db.TblIncomeOutputs.AsNoTracking().CountAsync().ConfigureAwait(false);
            return r;
        }

        #endregion
    }
}