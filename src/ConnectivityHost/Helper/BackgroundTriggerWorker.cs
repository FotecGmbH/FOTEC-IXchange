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
using BDA.Common.Exchange.Enum;
using Biss.Dc.Server;
using Biss.Log.Producer;
using Exchange.Enum;
using IXchange.Service.AppConnectivity.DataConnector;
using IXchange.Service.Com.Rest.Controllers;
using IXchangeDatabase;
using IXchangeDatabase.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EnumReloadDcList = Exchange.Enum.EnumReloadDcList;

namespace ConnectivityHost.Helper
{
    /// <summary>
    /// <para>Worker fuer Triggers</para>
    /// Klasse BackgroundTriggerWorker. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class BackgroundTriggerWorker
    {
        /// <summary>
        /// worker
        /// </summary>
        private static Task? _worker;

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
        private readonly long _waitTime = 90000;

        /// <summary>
        /// CancelationToken fuer Task
        /// </summary>
        private readonly CancellationToken _cancellationToken;

        /// <summary>
        /// BackgroundTriggerWorker
        /// </summary>
        /// <param name="clientConnection">Connection fuer DC</param>
        /// <param name="db">Datenbank</param>
        /// <param name="cts">CancelationToken</param>
        /// <param name="serviceScopeFactory">Factory fuer DC</param>
        /// <exception cref="ArgumentException"></exception>
        public BackgroundTriggerWorker(IDcConnections clientConnection, Db db, CancellationTokenSource cts, IServiceScopeFactory serviceScopeFactory)
        {
            _db = db;
            _cancellationToken = cts.Token;


            if (serviceScopeFactory == null!)
            {
                throw new ArgumentException($"[{nameof(BackgroundTriggerWorker)}]({nameof(BackgroundTriggerWorker)}): {nameof(serviceScopeFactory)} is NULL!");
            }

            var scope = serviceScopeFactory.CreateScope();
            var s = scope.ServiceProvider.GetService<IServerRemoteCalls>();
            if (scope == null! || s == null!)
            {
                throw new ArgumentException($"[{nameof(BackgroundTriggerWorker)}]({nameof(BackgroundTriggerWorker)}): {nameof(scope)}  is NULL!");
            }

            _dc = (ServerRemoteCalls) s;
            _dc.SetClientConnection(clientConnection);
        }

        /// <summary>
        /// Startet backgroundworker, fuer IXies, Abo und Bereitstellung von Daten
        /// </summary>
        public void StartBackgroundWorker()
        {
            if (_worker == null || _worker.Status != TaskStatus.Running && _worker.Status != TaskStatus.WaitingForActivation)
            {
                _worker = Worker();
            }
        }

        /// <summary>
        /// Backgroundworker, fuer Trigger
        /// </summary>
        /// <returns>Task</returns>
        private async Task Worker()
        {
            Logging.Log.LogInfo($"[{nameof(BackgroundTriggerWorker)}]({nameof(Worker)}): BackgroundWorker started");


            var func = async () =>
            {
                var userIds = new List<long>();

                userIds.AddRange(await AboSpecific().ConfigureAwait(true));
                userIds.AddRange(await MeasurementDefinitionSpecific().ConfigureAwait(true));

                userIds = userIds.Distinct().ToList();

                if (userIds.Any())
                {
                    await _dc.SendReloadList(EnumReloadDcList.ExNotifications).ConfigureAwait(false);
                }
            };

            await BackgroundHelper.Worker(_cancellationToken, _waitTime, func).ConfigureAwait(false);

            Logging.Log.LogInfo($"[{nameof(MeasurementResultController)}]({nameof(Worker)}): BackgroundWorker stopped");
        }

        /// <summary>
        /// Trigger die 1mal pro Messwertdefinition sind
        /// z.b. wurde die Messwertdefinition bewertet?
        /// Diese sind auch nur fuer Besitzer moeglich
        /// </summary>
        /// <returns></returns>
        private async Task<List<long>> MeasurementDefinitionSpecific()
        {
            using var currDb = new Db();

            var userIds = new List<long>();

            var measurementDefinitionAssignments = currDb.TblMeasurementDefinitionAssignments
                //.Include(m=>m.TblMeasurementDefinition)
                //.ThenInclude(m => m.TblMeasurements)
                .Include(m => m.TblRatings)
                .Include(m => m.TblMeasurementDefinition)
                .ThenInclude(m => m.TblIoTDevice)
                .ThenInclude(d => d.TblGateway)
                .ThenInclude(g => g!.TblCompany)
                .ThenInclude(c => c.TblPermissions)
                .AsNoTracking();


            var timeStamp = DateTime.UtcNow - TimeSpan.FromMinutes(_waitTime);
            var timeStamp2 = DateTime.UtcNow - TimeSpan.FromHours(6); // In den letzten 6 Stunden

            foreach (var measurementDefinitionAssignment in measurementDefinitionAssignments)
            {
                var save = false;

                if (!measurementDefinitionAssignment.TblMeasurementDefinition.TblIoTDevice.TblGateway!.TblCompany.TblPermissions.Any())
                {
                    continue;
                }

                var userId = measurementDefinitionAssignment.TblMeasurementDefinition.TblIoTDevice.TblGateway.TblCompany.TblPermissions.FirstOrDefault()!.TblUserId;

                var currNotifications = currDb.TblNotifications.Where(n => n.TblUserId == userId && n.TimeStamp > timeStamp2);

                if (measurementDefinitionAssignment.NotificationOnNewRating)
                {
                    if (await NotificationOnNewRating(currDb, currNotifications, measurementDefinitionAssignment, timeStamp, userId))
                    {
                        save = true;
                    }
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
        /// Benachrichtigung wenn eine neue Bewertung fuer eine Messwertdefinition
        /// eingetroffen ist
        /// </summary>
        /// <param name="currDb">Datenbank</param>
        /// <param name="currNotifications">die relevanten Notifizierungen</param>
        /// <param name="measurementDefinitionAssignment">Messwertdefinition</param>
        /// <param name="timeStamp">Zeitpunk wann das letzte mal gecheckt wurde</param>
        /// <param name="userId">Id des Users</param>
        /// <returns>Wurde eine Notifizierung hinzugefügt?</returns>
        private async Task<bool> NotificationOnNewRating(Db currDb, IQueryable<TableNotification> currNotifications, TableMeasurementDefinitionAssignment measurementDefinitionAssignment, DateTime timeStamp, long userId)
        {
            var measurementDefinition = measurementDefinitionAssignment.TblMeasurementDefinition;

            var lastRatings = measurementDefinitionAssignment.TblRatings.Where(r => r.TimeStamp > timeStamp);
            var lastNots = currNotifications.Where(n => n.TimeStamp > timeStamp && n.NotificationType == EnumNotificationType.Rating && n.TblMeasurementDefinitionAssignmentId == measurementDefinition.Id);

            if (lastRatings.Any() && !lastNots.Any())
            {
                await currDb.TblNotifications.AddAsync(new TableNotification
                                                       {
                                                           NotificationType = EnumNotificationType.Rating,
                                                           TblMeasurementDefinitionAssignmentId = measurementDefinition.Id,
                                                           TimeStamp = DateTime.UtcNow,
                                                           TblUserId = userId,
                                                           Description = "Sensor wurde bewertet"
                                                       }).ConfigureAwait(true);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Benachrichtigungen im Zusammenhang mit abos
        /// z.b. Unterschreitung eines wertes
        /// Kann jeder benutzer fuer jeden seiner abonierten Sensoren einstellen
        /// </summary>
        /// <returns></returns>
        private async Task<List<long>> AboSpecific()
        {
            using var currDb = new Db();

            var userIds = new List<long>();

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
                .Include(x => x.TblMeasurementDefinitionAssignment)
                .ThenInclude(x => x.TblMeasurementDefinition)
                .ThenInclude(x => x.TblMeasurements)
                .AsNoTracking();

            await foreach (var user in currDb.TblUsers)
            {
                var timeStamp = DateTime.UtcNow - TimeSpan.FromMinutes(_waitTime); // In der letzten minute
                var timeStamp2 = DateTime.UtcNow - TimeSpan.FromHours(6); // In den letzten 6 Stunden

                var currAbos = abos.Where(a => a.TblUserId == user.Id);
                //notifications des Users die juenger als 6 Stunden sind, z.b. unterschreitung moechte ich nicht jede minute bekommen sondern eben alle 6 stunden, falls es drunter bleibt
                var currNotifications = currDb.TblNotifications.Where(n => n.TblUserId == user.Id && n.TimeStamp > timeStamp2);

                var save = false;

                foreach (var abo in currAbos)
                {
                    if (abo.ExceedNotify)
                    {
                        if (await NotificationOnExceed(currDb, abo, currNotifications, timeStamp, user.Id).ConfigureAwait(true))
                        {
                            save = true;
                        }
                    }

                    if (abo.UndercutNotify)
                    {
                        if (await NotificationOnUndercut(currDb, abo, currNotifications, timeStamp, user.Id).ConfigureAwait(true))
                        {
                            save = true;
                        }
                    }

                    if (abo.FailureForMinutesNotify)
                    {
                        if (await NotificationOnFailure(currDb, abo, currNotifications, timeStamp, user.Id).ConfigureAwait(true))
                        {
                            save = true;
                        }
                    }

                    if (abo.MovingAverageNotify)
                    {
                        if (await NotificationOnMovingAverage(currDb, abo, currNotifications, timeStamp2, user.Id).ConfigureAwait(true))
                        {
                            save = true;
                        }
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
        /// Notifizierung bei ueberschreitung eines Wertes
        /// </summary>
        /// <param name="currDb">Datenbank</param>
        /// <param name="currNotifications">die relevanten Notifizierungen</param>
        /// <param name="abo">Betroffenes Abo</param>
        /// <param name="timeStamp">Zeitpunk wann das letzte mal gecheckt wurde</param>
        /// <param name="userId">Id des Users</param>
        /// <returns>Wurde eine Notifizierung hinzugefügt?</returns>
        private async Task<bool> NotificationOnExceed(Db currDb, TableAbo abo, IQueryable<TableNotification> currNotifications, DateTime timeStamp, long userId)
        {
            var measurementResults = abo.TblMeasurementDefinitionAssignment.TblMeasurementDefinition.TblMeasurements.Where(m => m.TimeStamp > timeStamp)
                .Where(mr => mr.ValueType == EnumValueTypes.Number && mr.Value.Number > abo.ExceedNotifyValue);

            // es gibt messwerte die den wert ueberschritten haben und es gibt keine benachrichtigung darueber in den letzten 6 Stunden
            // ReSharper disable once PossibleMultipleEnumeration
            if (measurementResults.Any() && !currNotifications.Any(n => n.TblMeasurementDefinitionAssignmentId == abo.TblMeasurementDefinitionAssignmentId && n.NotificationType == EnumNotificationType.Exceed))
            {
                await currDb.TblNotifications.AddAsync(new TableNotification
                                                       {
                                                           NotificationType = EnumNotificationType.Exceed,
                                                           TblMeasurementDefinitionAssignmentId = abo.TblMeasurementDefinitionAssignmentId,
                                                           TimeStamp = DateTime.UtcNow,
                                                           TblUserId = userId,
                                                           // ReSharper disable once PossibleMultipleEnumeration
                                                           Description = "Wert: " + abo.ExceedNotifyValue + " wurde mit dem Wert: " + measurementResults.Max(m => m.Value.Number) + " überschritten"
                                                       }).ConfigureAwait(true);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Notifizierung bei unterschreitung eines Wertes
        /// </summary>
        /// <param name="currDb">Datenbank</param>
        /// <param name="currNotifications">die relevanten Notifizierungen</param>
        /// <param name="abo">Betroffenes Abo</param>
        /// <param name="timeStamp">Zeitpunk wann das letzte mal gecheckt wurde</param>
        /// <param name="userId">Id des Users</param>
        /// <returns>Wurde eine Notifizierung hinzugefügt?</returns>
        private async Task<bool> NotificationOnUndercut(Db currDb, TableAbo abo, IQueryable<TableNotification> currNotifications, DateTime timeStamp, long userId)
        {
            var measurementResults = abo.TblMeasurementDefinitionAssignment.TblMeasurementDefinition.TblMeasurements.Where(m => m.TimeStamp > timeStamp)
                .Where(mr => mr.ValueType == EnumValueTypes.Number && mr.Value.Number < abo.UndercutNotifyValue);

            // es gibt messwerte die den wert unterschritten haben und es gibt keine benachrichtigung darueber in den letzten 6 Stunden
            // ReSharper disable once PossibleMultipleEnumeration
            if (measurementResults.Any() && !currNotifications.Any(n => n.TblMeasurementDefinitionAssignmentId == abo.TblMeasurementDefinitionAssignmentId && n.NotificationType == EnumNotificationType.Undercut))
            {
                await currDb.TblNotifications.AddAsync(new TableNotification
                                                       {
                                                           NotificationType = EnumNotificationType.Undercut,
                                                           TblMeasurementDefinitionAssignmentId = abo.TblMeasurementDefinitionAssignmentId,
                                                           TimeStamp = DateTime.UtcNow,
                                                           TblUserId = userId,
                                                           // ReSharper disable once PossibleMultipleEnumeration
                                                           Description = "Wert: " + abo.UndercutNotifyValue + " wurde mit dem Wert: " + measurementResults.Min(m => m.Value.Number) + " unterschritten"
                                                       }).ConfigureAwait(true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Notifizierung bei Ausfall eines Sensors seit X minuten
        /// </summary>
        /// <param name="currDb">Datenbank</param>
        /// <param name="currNotifications">die relevanten Notifizierungen</param>
        /// <param name="abo">Betroffenes Abo</param>
        /// <param name="timeStamp">Zeitpunk wann das letzte mal gecheckt wurde</param>
        /// <param name="userId">Id des Users</param>
        /// <returns>Wurde eine Notifizierung hinzugefügt?</returns>
        // ReSharper disable once UnusedParameter.Local
        private async Task<bool> NotificationOnFailure(Db currDb, TableAbo abo, IQueryable<TableNotification> currNotifications, DateTime timeStamp, long userId)
        {
            var currTimeStamp = DateTime.UtcNow - TimeSpan.FromMinutes(abo.FailureForMinutesNotifyValue);

            var measurementResults = abo.TblMeasurementDefinitionAssignment.TblMeasurementDefinition.TblMeasurements.Where(m => m.TimeStamp > currTimeStamp);


            if (!measurementResults.Any() && abo.TblMeasurementDefinitionAssignment.TblMeasurementDefinition.TblMeasurements.Any() && !currNotifications.Any(n => n.TblMeasurementDefinitionAssignmentId == abo.TblMeasurementDefinitionAssignmentId && n.NotificationType == EnumNotificationType.Failure))
            {
                // ReSharper disable once RedundantAssignment
                var msg = ", nämlich noch nie Werte gesendet";
                //if (abo.TblMeasurementDefinitionAssignment.TblMeasurements.Any())
                msg = ", nämlich seit " + abo.TblMeasurementDefinitionAssignment.TblMeasurementDefinition.TblMeasurements.Max(m => m.TimeStamp).ToString("s") + " nichts mehr gesendet";

                await currDb.TblNotifications.AddAsync(new TableNotification
                                                       {
                                                           NotificationType = EnumNotificationType.Failure,
                                                           TblMeasurementDefinitionAssignmentId = abo.TblMeasurementDefinitionAssignmentId,
                                                           TimeStamp = DateTime.UtcNow,
                                                           TblUserId = userId,
                                                           Description = "Sensor hat seit über: " + abo.FailureForMinutesNotifyValue + " Minuten" + msg
                                                       }).ConfigureAwait(true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Notifizierung bei veraenderung des Moving Average innerhalb der letzten X Stunden, Stundenweise
        /// </summary>
        /// <param name="currDb">Datenbank</param>
        /// <param name="currNotifications">die relevanten Notifizierungen</param>
        /// <param name="abo">Betroffenes Abo</param>
        /// <param name="timeStamp2">Zeitpunk der letzten X stunden, die gecheckt werden sollen</param>
        /// <param name="userId">Id des Users</param>
        /// <returns>Wurde eine Notifizierung hinzugefügt?</returns>
        private async Task<bool> NotificationOnMovingAverage(Db currDb, TableAbo abo, IQueryable<TableNotification> currNotifications, DateTime timeStamp2, long userId)
        {
            // fuer den moving average nimm die letzten 6 Stunden.....erstelle hierzu 6 Groupings mit den werten der jeweiligen stunde
            var hourGroups = abo.TblMeasurementDefinitionAssignment.TblMeasurementDefinition.TblMeasurements.Where(m => m.TimeStamp > timeStamp2)
                .Where(mr => mr.ValueType == EnumValueTypes.Number).GroupBy(m => m.TimeStamp.Hour);

            var values = new List<double?>();

            foreach (var hourGroup in hourGroups)
            {
                if (hourGroup.Any())
                {
                    values.Add(hourGroup.Average(h => h.Value.Number));
                }
            }

            values = values.Distinct().ToList();

            // die durchschnitte der letzten 6 stunden sind mehr auseinander als der angegebene wert und es gibt keine benachrichtigung darueber in den letzten 6 Stunden
            if (values.Any() && values.Max() - values.Min() > abo.MovingAverageNotifyValue && !currNotifications.Any(n => n.TblMeasurementDefinitionAssignmentId == abo.TblMeasurementDefinitionAssignmentId && n.NotificationType == EnumNotificationType.MovingAverage))
            {
                await currDb.TblNotifications.AddAsync(new TableNotification
                                                       {
                                                           NotificationType = EnumNotificationType.MovingAverage,
                                                           TblMeasurementDefinitionAssignmentId = abo.TblMeasurementDefinitionAssignmentId,
                                                           TimeStamp = DateTime.UtcNow,
                                                           TblUserId = userId,
                                                           Description = "Mittelwert der letzten 6 Stunden hat sich um mehr als: " + abo.MovingAverageNotifyValue + " verändert"
                                                       }).ConfigureAwait(true);
                return true;
            }

            return false;
        }
    }
}