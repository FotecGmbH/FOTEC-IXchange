﻿// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System.Diagnostics;
using System.Threading.Tasks;
using Biss.Apps.Blazor.Pages;
using Biss.Dc.Server;
using Biss.Log.Producer;
using ConnectivityHost.BaseApp;
using IXchange.Service.AppConnectivity.DataConnector;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace ConnectivityHost.BissApps
{
    /// <summary>
    ///     Basis für RazorPages
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProjectRazorPage<T> : BissRazorPage<T> where T : VmProjectBase
    {
        #region Injects

        /// <summary>
        /// Dc Connections
        /// </summary>
        [Inject]
        public IDcConnections DcConnections { get; set; } = null!;

        /// <summary>
        /// ServerRemoteCalls
        /// </summary>
        [Inject]
        public IServerRemoteCalls ServerRemoteCalls { get; set; } = null!;

        #endregion

        #region BissLifecycle

        /// <summary>
        ///     Appstart - Wird einmal für die App aufgerufen
        /// </summary>
        /// <returns></returns>
        protected override async Task OnAppStart()
        {
            Logging.Log.LogTrace($"[ProjectRazorPage-({GetType()}]({nameof(OnAppStart)}): Start");

            var sw = new Stopwatch();
            sw.Start();

            await base.OnAppStart().ConfigureAwait(true);

            Logging.Log.LogTrace($"[ProjectRazorPage-({GetType()}]({nameof(OnAppStart)}): Finish - {sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        ///     View Create entspricht Constructor für VM, einmal pro View
        /// </summary>
        /// <returns></returns>
        protected override Task OnViewCreate()
        {
            Logging.Log.LogTrace($"[ProjectRazorPage-({GetType()}]({nameof(OnViewCreate)}): ");
            return base.OnViewCreate();
        }

        /// <summary>
        ///     Entspricht VM OnAppearing, wird einmal pro View aufgerufen
        /// </summary>
        /// <returns></returns>
        protected override Task OnViewAppearing()
        {
            Logging.Log.LogTrace($"[ProjectRazorPage-({GetType()}]({nameof(OnViewAppearing)}): ");
            return base.OnViewAppearing();
        }

        /// <summary>
        ///     Entspricht VM OnActivated, wird einmal pro View aufgerufen
        /// </summary>
        /// <returns></returns>
        protected override Task OnViewActivated()
        {
            Logging.Log.LogTrace($"[ProjectRazorPage-({GetType()}]({nameof(OnViewActivated)}): ");
            return base.OnViewActivated();
        }

        /// <summary>
        ///     Entspricht VM OnLoaded, wird einmal pro View aufgerufen
        /// </summary>
        /// <returns></returns>
        protected override Task OnViewLoaded()
        {
            Logging.Log.LogTrace($"[ProjectRazorPage-({GetType()}]({nameof(OnViewLoaded)}): ");
            return base.OnViewLoaded();
        }

        #endregion
    }
}