// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Threading;
using System.Threading.Tasks;
using Biss.Dc.Server;
using Biss.Dc.Transport.Server.SignalR;
using Biss.Log.Producer;
using IXchange.Service.AppConnectivity.DataConnector;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace ConnectivityHost.Services
{
    /// <summary>
    ///     <para>Hintergrundservice</para>
    /// Klasse BackgroundService. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class BackgroundService<T> : IHostedService, IDisposable
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly IDcConnections _clientConnection;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ServerRemoteCalls _dc;

        // ReSharper disable once NotAccessedField.Local
        private readonly IHubContext<DcCoreHub<T>> _hubContext;
        // ReSharper disable once UnusedMember.Local
        private readonly DateTime _startDateTime = DateTime.UtcNow;
#pragma warning disable CS0169 // Field is never used
        private int _counter10Min;
#pragma warning restore CS0169 // Field is never used
        private bool _disposedValue;

        /// <summary>
        ///     Timer
        /// </summary>
        private Timer _timer = null!;

        /// <summary>
        ///     Konstruktor, Services werden injected
        /// </summary>
        /// <param name="clientConnection"></param>
        /// <param name="hubcontext"></param>
        /// <param name="serviceScopeFactory"></param>
        public BackgroundService(IDcConnections clientConnection, IHubContext<DcCoreHub<T>> hubcontext, IServiceScopeFactory serviceScopeFactory)
        {
            _clientConnection = clientConnection;
            _hubContext = hubcontext;

            if (serviceScopeFactory == null!)
            {
                throw new ArgumentException($"[{nameof(BackgroundService)}]({nameof(BackgroundService)}): {nameof(serviceScopeFactory)} is NULL!");
            }

            var scope = serviceScopeFactory.CreateScope();
            var s = scope.ServiceProvider.GetService<IServerRemoteCalls>();
            if (scope == null! || s == null!)
            {
                throw new ArgumentException($"[{nameof(BackgroundService)}]({nameof(BackgroundService)}): {nameof(scope)}  is NULL!");
            }

            _dc = (ServerRemoteCalls) s;
            _dc.SetClientConnection(_clientConnection);
        }

        /// <summary>
        ///     Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _timer.Dispose();
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        ///     Führt arbeiten im angegebenen Interval durch
        ///     (Für eventhandler kann async void verwendet werden)
        /// </summary>
        /// <param name="state"></param>
        private void DoWork(object state)
        {
        }

        #region Interface Implementations

        /// <summary>
        ///     Dispose
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Start
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken stoppingToken)
        {
            Logging.Log.LogInfo($"[{nameof(BackgroundService)}]({nameof(StartAsync)}): Timed Hosted Service running.");
            _timer = new Timer(DoWork!, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Stop
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken stoppingToken)
        {
            Logging.Log.LogInfo($"[{nameof(BackgroundService)}]({nameof(StopAsync)}): Timed Hosted Service is stopping.");
            _timer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        #endregion
    }
}