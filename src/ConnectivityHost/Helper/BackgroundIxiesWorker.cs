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
using System.Threading;
using System.Threading.Tasks;
using Biss.Dc.Server;
using Biss.Log.Producer;
using Exchange.Enum;
using IXchange.Service.AppConnectivity.DataConnector;
using IXchange.Service.Com.Rest.Controllers;
using IXchangeDatabase;
using IXchangeDatabase.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace ConnectivityHost.Helper
{
    /// <summary>
    /// <para>WorkerSlow fuer IXies, Abo und Bereitstellung von Daten</para>
    /// Klasse BackgroundIxiesWorker. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class BackgroundIxiesWorker
    {
        /// <summary>
        /// worker fuer alle sachen die selten passieren(alle 2m30s)
        /// </summary>
        private static Task? _workerSlow;

        /// <summary>
        /// worker fuer alle sachen die oft passieren(alle 10s)
        /// </summary>
        private static Task? _workerFast;

        /// <summary>
        /// Datenbank Context
        /// </summary>
        // ReSharper disable once NotAccessedField.Local
        private readonly Db _db;

        /// <summary>
        /// DC Referenz
        /// </summary>
        private readonly ServerRemoteCalls _dc;

        /// <summary>
        /// Wartezeit fuer den worker zwischen den durchlaeufen
        /// </summary>
        private readonly long _waitTimeFast = 10000;

        /// <summary>
        /// Wartezeit fuer den worker zwischen den durchlaeufen
        /// </summary>
        private readonly long _waitTimeSlow = 150000;

        /// <summary>
        /// CancelationToken fuer worker
        /// </summary>
        private readonly CancellationToken _cancellationToken;

        /// <summary>
        /// BackgroundIxiesWorker
        /// </summary>
        /// <param name="clientConnection">Connection fuer DC</param>
        /// <param name="db">Datenbank</param>
        /// <param name="cts">CancelationToken</param>
        /// <param name="serviceScopeFactory">Factory fuer DC</param>
        /// <exception cref="ArgumentException"></exception>
        public BackgroundIxiesWorker(IDcConnections clientConnection, Db db, CancellationTokenSource cts, IServiceScopeFactory serviceScopeFactory)
        {
            if (db == null!)
            {
                throw new ArgumentException($"[{nameof(BackgroundIxiesWorker)}]({nameof(BackgroundIxiesWorker)}): {nameof(db)} is NULL!");
            }

            if (cts == null!)
            {
                throw new ArgumentException($"[{nameof(BackgroundIxiesWorker)}]({nameof(BackgroundIxiesWorker)}): {nameof(cts)} is NULL!");
            }

            _db = db;
            _cancellationToken = cts.Token;

            if (serviceScopeFactory == null!)
            {
                throw new ArgumentException($"[{nameof(BackgroundIxiesWorker)}]({nameof(BackgroundIxiesWorker)}): {nameof(serviceScopeFactory)} is NULL!");
            }

            var scope = serviceScopeFactory.CreateScope();
            var s = scope.ServiceProvider.GetService<IServerRemoteCalls>();
            if (scope == null! || s == null!)
            {
                throw new ArgumentException($"[{nameof(BackgroundIxiesWorker)}]({nameof(BackgroundIxiesWorker)}): {nameof(scope)}  is NULL!");
            }

            _dc = (ServerRemoteCalls) s;
            _dc.SetClientConnection(clientConnection);
        }


        /// <summary>
        /// Startet backgroundworker, fuer IXies, Abo und Bereitstellung von Daten
        /// Startet backgroundworker, fuer IXies, Abo und Bereitstellung von Daten
        /// </summary>
        public void StartBackgroundWorker()
        {
            if (_workerSlow == null || _workerSlow.Status != TaskStatus.Running && _workerSlow.Status != TaskStatus.WaitingForActivation)
            {
                _workerSlow = WorkerSlow();
            }

            if (_workerFast == null || _workerFast.Status != TaskStatus.Running && _workerFast.Status != TaskStatus.WaitingForActivation)
            {
                _workerFast = WorkerFast();
            }
        }

        /// <summary>
        /// Backgroundworker, fuer alle Ixie Berechnungen die oft passieren(alle 10 Sekunden)
        /// </summary>
        /// <returns>Task</returns>
        private async Task WorkerFast()
        {
            Logging.Log.LogInfo($"[{nameof(BackgroundIxiesWorker)}]({nameof(WorkerSlow)}): BackgroundWorker started");


            var func = async () =>
            {
                var userIds = new List<long>();

                userIds.AddRange(await IxiesFromRating().ConfigureAwait(true));
                userIds.AddRange(await IxiesFromAccountCreation().ConfigureAwait(true));
                userIds.AddRange(await IxiesFromSensorCreation().ConfigureAwait(true));

                userIds = userIds.Distinct().ToList();

                if (userIds.Any())
                {
                    await _dc.SendReloadList(EnumReloadDcList.ExIncomeOutputs).ConfigureAwait(false);
                }
            };

            await BackgroundHelper.Worker(_cancellationToken, _waitTimeFast, func).ConfigureAwait(false);

            Logging.Log.LogInfo($"[{nameof(MeasurementResultController)}]({nameof(WorkerSlow)}): BackgroundWorker stopped");
        }

        /// <summary>
        /// Erhalte Ixies fuer einen erstellten Sensor
        /// Wenn ein Sensor(Measurementdefinition) erstellt wird und mindestens 1 Measurementresult hat
        /// </summary>
        /// <returns>Betroffene UserIds</returns>
        private async Task<IEnumerable<long>> IxiesFromSensorCreation()
        {
            await using var currDb = new Db();

            var measurementDefinitions = currDb.TblMeasurementDefinitions
                .Include(m => m.TblIoTDevice)
                .ThenInclude(d => d.TblGateway)
                .ThenInclude(g => g!.TblCompany)
                .ThenInclude(c => c.TblPermissions)
                .Include(m => m.TblMeasurements)
                .AsNoTracking();

            var userIds = new List<long>();

            foreach (var measurementDefinition in measurementDefinitions)
            {
                var save = false;

                if (!measurementDefinition.TblMeasurements.Any()) // Messwertdefinition hat nicht einmal einen Wert bis jetzt gesendet
                {
                    continue;
                }


                if (!measurementDefinition.TblIoTDevice.TblGateway!.TblCompany.TblPermissions.Any())
                {
                    continue;
                }

                var userId = measurementDefinition.TblIoTDevice.TblGateway.TblCompany.TblPermissions.FirstOrDefault()!.TblUserId;


                if (!currDb.TblIncomeOutputs // gab es bereits einen ixie eintrag fuer diesen messwert?
                        .Any(i => i.Option == EnumIncomeOutputOption.CreateIoTDevice && i.TblMeasurementDefinitonId == measurementDefinition.Id && i.TblUserId == userId)) // und hat dieser zumindest 1 wert gesendet?
                {
                    save = true;
                    var previousUserIxies = PreviousTotalCalculator(currDb.TblIncomeOutputs.Where(io => io.TblUserId == userId));
                    await currDb.TblIncomeOutputs.AddAsync(new TableIncomeOutput
                                                           {
                                                               Option = EnumIncomeOutputOption.CreateIoTDevice,
                                                               TimeStamp = DateTime.UtcNow,
                                                               TblMeasurementDefinitonId = measurementDefinition.Id,
                                                               Ixies = 20,
                                                               TblUserId = userId,
                                                               CurrentTotalIxies = previousUserIxies + 20
                                                           }).ConfigureAwait(true);
                }

                if (save)
                {
                    await currDb.SaveChangesAsync(_cancellationToken).ConfigureAwait(true);
                    userIds.Add(userId);
                }
            }

            return userIds;
        }

        /// <summary>
        /// Erhalte Ixies fuer Erstellung eines Accounts
        /// </summary>
        /// <returns>betroffene UserIds</returns>
        private async Task<IEnumerable<long>> IxiesFromAccountCreation()
        {
            await using var currDb = new Db();

            var userIds = new List<long>();

            await foreach (var user in currDb.TblUsers)
            {
                var save = false; // gab es bereits einen ixie eintrag fuer diesen account?
                if (!currDb.TblIncomeOutputs.Any(i => i.Option == EnumIncomeOutputOption.CreateAccount && i.TblUserId == user.Id))
                {
                    save = true;
                    var previousUserIxies = PreviousTotalCalculator(currDb.TblIncomeOutputs.Where(io => io.TblUserId == user.Id));
                    await currDb.TblIncomeOutputs.AddAsync(new TableIncomeOutput
                                                           {
                                                               Option = EnumIncomeOutputOption.CreateAccount,
                                                               TimeStamp = DateTime.UtcNow,
                                                               TblMeasurementDefinitonId = null,
                                                               Ixies = 100,
                                                               TblUserId = user.Id,
                                                               CurrentTotalIxies = previousUserIxies + 100
                                                           }).ConfigureAwait(true);
                }

                if (save)
                {
                    await currDb.SaveChangesAsync(_cancellationToken).ConfigureAwait(true);
                    userIds.Add(user.Id);
                }
            }

            return userIds;
        }

        /// <summary>
        /// Erhalte Ixies fuer eine Bewertung
        /// </summary>
        /// <returns>betroffene UserIds</returns>
        private async Task<IEnumerable<long>> IxiesFromRating()
        {
            await using var currDb = new Db();

            var userIds = new List<long>();

            await foreach (var user in currDb.TblUsers)
            {
                var save = false;

                var currRatingsMesDefAsIds = currDb.TblRatings.Include(r => r.TblMeasurementDefinitionAssignment).Where(r => r.TblUserId == user.Id).Select(r => r.TblMeasurementDefinitionAssignmentId).ToList();


                foreach (var ratingMesDefAsId in currRatingsMesDefAsIds)
                {
                    var currMesDef = currDb.TblMeasurementDefinitionAssignments.Include(mA => mA.TblMeasurementDefinition).FirstOrDefault(mA => mA.Id == ratingMesDefAsId)!.TblMeasurementDefinition;

                    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                    if (currMesDef != null && !currDb.TblIncomeOutputs.Any(i => i.Option == EnumIncomeOutputOption.Rating && i.TblUserId == user.Id && i.TblMeasurementDefinitonId == currMesDef.Id))
                    {
                        save = true;
                        var previousUserIxies = PreviousTotalCalculator(currDb.TblIncomeOutputs.Where(io => io.TblUserId == user.Id));
                        var newIcomeOutput = new TableIncomeOutput
                                             {
                                                 Option = EnumIncomeOutputOption.Rating,
                                                 TimeStamp = DateTime.UtcNow,
                                                 TblMeasurementDefinitonId = currMesDef.Id,
                                                 Ixies = 2,
                                                 TblUserId = user.Id,
                                                 CurrentTotalIxies = previousUserIxies + 2
                                             };
                        await currDb.TblIncomeOutputs.AddAsync(newIcomeOutput).ConfigureAwait(true);
                    }
                }

                if (save)
                {
                    await currDb.SaveChangesAsync(_cancellationToken).ConfigureAwait(true);
                    userIds.Add(user.Id);
                }
            }

            return userIds;
        }

        /// <summary>
        /// Backgroundworker, fuer Erhalten von Ixies fuer alles was nicht oft passiert(z.b. 1mal täglich Abo Kosten)
        /// </summary>
        /// <returns>Task</returns>
        private async Task WorkerSlow()
        {
            Logging.Log.LogInfo($"[{nameof(BackgroundIxiesWorker)}]({nameof(WorkerSlow)}): BackgroundWorker started");

            var func = async () =>
            {
                var userIds = new List<long>();

                userIds.AddRange(await IxiesFromDeliveredData().ConfigureAwait(true));
                userIds.AddRange(await IxiesFromAbos().ConfigureAwait(true));

                userIds = userIds.Distinct().ToList();

                if (userIds.Any())
                {
                    await _dc.SendReloadList(EnumReloadDcList.ExAbos).ConfigureAwait(false);
                    await _dc.SendReloadList(EnumReloadDcList.ExIncomeOutputs).ConfigureAwait(false);
                }
            };

            await BackgroundHelper.Worker(_cancellationToken, _waitTimeSlow, func).ConfigureAwait(false);

            Logging.Log.LogInfo($"[{nameof(MeasurementResultController)}]({nameof(WorkerSlow)}): BackgroundWorker stopped");
        }

        /// <summary>
        /// Erhalte Ixies fuer das liefern von Daten
        /// 1mal taeglich, falls im letzten Tag Daten eingetroffen sind
        /// </summary>
        /// <returns>Betroffene userids</returns>
        private async Task<List<long>> IxiesFromDeliveredData()
        {
            await using var currDb = new Db();

            var measurementDefinitionAssignments = currDb.TblMeasurementDefinitionAssignments
                .Include(m => m.TblMeasurementDefinition)
                .ThenInclude(m => m.TblMeasurements)
                .Include(m => m.TblRatings)
                .Include(m => m.TblMeasurementDefinition)
                .ThenInclude(m => m.TblIoTDevice)
                .ThenInclude(d => d.TblGateway)
                .ThenInclude(g => g!.TblCompany)
                .ThenInclude(c => c.TblPermissions)
                .AsNoTracking();

            //var measurementDefinitions = currDb.TblMeasurementDefinitions
            //    .Include(m => m.TblMeasurements)
            //    //.Include(m => m.TblRatings)
            //    .Include(m => m.TblIoTDevice)
            //    .ThenInclude(d => d.TblGateway)
            //    .ThenInclude(g => g.TblCompany)
            //    .ThenInclude(c => c.TblPermissions)
            //    .AsNoTracking();


            var timeStamp = DateTime.UtcNow - TimeSpan.FromDays(1);
            // alle income outputs mit option "transfer", welche im letzten Tag passiert sind, davon liefer mir die MeasurementDefinitionIds
            var incomeOutputMeasurementIds = currDb.TblIncomeOutputs.Where(i => i.Option == EnumIncomeOutputOption.Transfer && i.TimeStamp > timeStamp)
                .Select(io => io.TblMeasurementDefinitonId).ToList(); //ohne ToList funktioniert Contains in einem Linq nicht

            var userIds = new List<long>();

            try
            {
                measurementDefinitionAssignments = measurementDefinitionAssignments
                    .Where(m => m.TblMeasurementDefinition.TblMeasurements.Any(mr => mr.TimeStamp > timeStamp)) // Die Definitionen, welche im letzten Tag mindestens einen Wert gesendet haben
                    .Where(md => !incomeOutputMeasurementIds.Contains(md.Id)) // Die, welche in dem letzten Tag keine income/output produziert haben
                    .AsQueryable();

                foreach (var measurementDefinitionAssignment in measurementDefinitionAssignments)
                {
                    var ixies = IxieCalculation(measurementDefinitionAssignment.TblRatings, 1, 2, 3);

                    long userId = -1;

                    try
                    {
                        userId = measurementDefinitionAssignment.TblMeasurementDefinition.TblIoTDevice.TblGateway!.TblCompany.TblPermissions.FirstOrDefault()!.TblUserId;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    if (userId != -1)
                    {
                        userIds.Add(userId);
                        var previousUserIxies = PreviousTotalCalculator(currDb.TblIncomeOutputs.Where(io => io.TblUserId == userId));
                        await currDb.TblIncomeOutputs.AddAsync(new TableIncomeOutput
                                                               {
                                                                   Option = EnumIncomeOutputOption.Transfer,
                                                                   TimeStamp = DateTime.UtcNow,
                                                                   TblMeasurementDefinitonId = measurementDefinitionAssignment.TblMeasurementDefinition.Id,
                                                                   Ixies = ixies,
                                                                   TblUserId = userId,
                                                                   CurrentTotalIxies = previousUserIxies + ixies
                                                               }).ConfigureAwait(true);
                    }
                }

                await currDb.SaveChangesAsync(_cancellationToken).ConfigureAwait(true);
            }
            catch (Exception)
            {
                // ignored
            }

            return userIds;
        }

        /// <summary>
        /// Eingaenge und Ausgaenge aufgrund von Abos
        /// 1mal taeglich
        /// </summary>
        /// <returns>Betroffene UserIds</returns>
        private async Task<List<long>> IxiesFromAbos()
        {
            await using var currDb = new Db();

            var abos = currDb.TblAbos
                .Include(x => x.TblUser)
                .Include(x => x.TblMeasurementDefinitionAssignment)
                .ThenInclude(x => x.TblMeasurementDefinition)
                .ThenInclude(x => x.Information)
                .Include(x => x.TblMeasurementDefinitionAssignment)
                .ThenInclude(x => x.TblMeasurementDefinition)
                .ThenInclude(x => x.TblIoTDevice)
                .ThenInclude(x => x.TblGateway)
                .ThenInclude(g => g!.TblCompany)
                .ThenInclude(c => c.TblPermissions)
                .Include(x => x.TblMeasurementDefinitionAssignment)
                .ThenInclude(x => x.TblRatings)
                .ThenInclude(x => x.TblUser)
                .AsNoTracking();

            var userIds = new List<long>();
            var timeStamp = DateTime.UtcNow - TimeSpan.FromDays(1);
            // alle income outputs mit option "abo", welche im letzten Tag passiert sind
            var incomeOutputs = currDb.TblIncomeOutputs.Where(i => i.Option == EnumIncomeOutputOption.Abo && i.TimeStamp > timeStamp);

            foreach (var abo in abos)
            {
                var finishedInputOutpu = incomeOutputs.FirstOrDefault(io => io.TblUserId == abo.TblUserId && io.TblMeasurementDefinitonId == abo.TblMeasurementDefinitionAssignment.TblMeasurementDefinition.Id);

                if (finishedInputOutpu == null) // es gibt noch kein input output für dieses abo, fuer den letzten tag
                {
                    long userId = -1;

                    try
                    {
                        if (abo.TblMeasurementDefinitionAssignment.TblMeasurementDefinition.TblIoTDevice.TblGateway!.TblCompany.TblPermissions.Any())
                        {
                            userId = abo.TblMeasurementDefinitionAssignment.TblMeasurementDefinition.TblIoTDevice.TblGateway.TblCompany.TblPermissions.FirstOrDefault()!.TblUserId;
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    if (userId != -1 && abo.TblUserId != userId)
                    {
                        userIds.Add(userId);
                        userIds.Add(abo.TblUserId);

                        var ixies = IxieCalculation(abo.TblMeasurementDefinitionAssignment.TblRatings, 2, 3, 4);
                        var previousAboUserIxies = PreviousTotalCalculator(currDb.TblIncomeOutputs.Where(io => io.TblUserId == abo.TblUserId));
                        var previousUserIxies = PreviousTotalCalculator(currDb.TblIncomeOutputs.Where(io => io.TblUserId == userId));

                        await currDb.TblIncomeOutputs.AddAsync(new TableIncomeOutput() // Verbraucher werden Ixies abgezogen
                                                               {
                                                                   Option = EnumIncomeOutputOption.Abo,
                                                                   TimeStamp = DateTime.UtcNow,
                                                                   TblMeasurementDefinitonId = abo.TblMeasurementDefinitionAssignment.TblMeasurementDefinition.Id,
                                                                   Ixies = -ixies,
                                                                   TblUserId = abo.TblUserId,
                                                                   CurrentTotalIxies = previousAboUserIxies - ixies
                                                               }).ConfigureAwait(true);

                        await currDb.TblIncomeOutputs.AddAsync(new TableIncomeOutput() // Betreieber bekommt Ixies
                                                               {
                                                                   Option = EnumIncomeOutputOption.Abo,
                                                                   TimeStamp = DateTime.UtcNow,
                                                                   TblMeasurementDefinitonId = abo.TblMeasurementDefinitionAssignment.TblMeasurementDefinition.Id,
                                                                   Ixies = ixies - 1,
                                                                   TblUserId = userId,
                                                                   CurrentTotalIxies = previousUserIxies + ixies - 1
                                                               }).ConfigureAwait(true);
                    }

                    await currDb.SaveChangesAsync(_cancellationToken).ConfigureAwait(true);
                }
            }

            return userIds;
        }

        /// <summary>
        /// Berechnet Ixies anhand von Liste an Bewertungen
        /// </summary>
        /// <param name="ratings">Bewertungen</param>
        /// <param name="low">bei schlechten Bewertungen</param>
        /// <param name="middle">Durchschnitt</param>
        /// <param name="high">bei guten Bewertungen</param>
        /// <returns>Kosten in Ixies</returns>
        private int IxieCalculation(ICollection<TableRating> ratings, int low, int middle, int high)
        {
            // ReSharper disable once RedundantAssignment
            var av = 0.0;
            if (ratings.Any())
            {
                av = ratings.Average(r => r.Rating);
            }
            else
            {
                av = 3;
            }

            // ReSharper disable once RedundantAssignment
            var ixies = 0;
            if (av <= 2)
            {
                ixies = low;
            }
            else if (av <= 4)
            {
                ixies = middle;
            }
            else
            {
                ixies = high;
            }

            return ixies;
        }

        /// <summary>
        /// Berechnet den ist Kontostand der vergangenen Ein- und Ausgaenge
        /// </summary>
        /// <param name="previous">vergangene Ein- und Ausgaenge</param>
        /// <returns>Kontostand der vergangenen Ein- und Ausgaenge</returns>
        private int PreviousTotalCalculator(IQueryable<TableIncomeOutput> previous)
        {
            var previousTotal = 0;
            if (previous.Any())
            {
                previousTotal = previous.OrderByDescending(t => t.TimeStamp).FirstOrDefault()!.CurrentTotalIxies;
            }

            return previousTotal;
        }
    }
}