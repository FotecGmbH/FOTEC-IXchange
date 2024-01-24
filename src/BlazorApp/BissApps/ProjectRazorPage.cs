// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 23.1.2024 10:37
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseApp;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Apps;
using Biss.Apps.Blazor.Pages;
using Biss.Apps.Connectivity.Blazor;
using Biss.Log.Producer;
using BlazorApp.Components.Modals;
using Exchange;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Radzen;
using TG.Blazor.IndexedDB;

namespace BlazorApp.BissApps
{
    /// <summary>
    ///     Basis für RazorPages
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProjectRazorPage<T> : BissRazorPage<T> where T : VmProjectBase
    {
        #region Properties

        /// <summary>
        ///     Detail-View View Model.
        /// </summary>
        public VmBase? DetailViewViewModel { get; set; } = null!;

        #endregion

        /// <summary>
        /// Oeffnet ratings modal
        /// </summary>
        /// <param name="measurementDefinition">Messwertdefinition</param>
        /// <returns>Task</returns>
        public async Task OpenRatingsModal(ExMeasurementDefinition measurementDefinition)
        {
            await OpenRatingsModal(measurementDefinition, RadzenDialogService).ConfigureAwait(true);
        }

        /// <summary>
        /// Oeffnet ratings modal
        /// </summary>
        /// <param name="measurementDefinition">Messwertdefinition</param>
        /// <param name="dialogService">dialogservice</param>
        /// <returns>Task</returns>
        public static async Task OpenRatingsModal(ExMeasurementDefinition measurementDefinition, DialogService dialogService)
        {
            if (measurementDefinition is null)
            {
                throw new ArgumentNullException(nameof(measurementDefinition));
            }

            if (dialogService is null)
            {
                throw new ArgumentNullException(nameof(dialogService));
            }

            var parameters = new Dictionary<string, object>
                             {
                                 {nameof(RatingsModal.MeasurementDefinition), measurementDefinition},
                             };

            await dialogService.OpenAsync<RatingsModal>(string.Empty, parameters, new DialogOptions
                                                                                  {
                                                                                      CloseDialogOnOverlayClick = true,
                                                                                      ShowTitle = false,
                                                                                      Width = "auto",
                                                                                      Height = "75%"
                                                                                  }).ConfigureAwait(true);
        }

        #region Injects

        /// <summary>
        ///     DC Cache - wenn ohne Indexed DB entfern!
        /// </summary>
        [Inject]
        protected IndexedDBManager? IndexedDb { get; set; }

        /// <summary>
        ///     JSInProcessRuntime für Blazor
        /// </summary>
        [Inject]
        protected IJSInProcessRuntime? JsInProcessRuntime { get; set; }

        /// <summary>
        /// Radzen DialogService (auch für Modals!)
        /// </summary>
        [Inject]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected DialogService RadzenDialogService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        #endregion

        #region BissLifecycle

        /// <inheritdoc />
        protected override async Task OnAppStart()
        {
            Logging.Log.LogInfo($"[ProjectRazorPage-({GetType()}]({nameof(OnAppStart)}): Start");

            await this.BissUseConnectivity(AppSettings.Current(), JsRuntime, JsInProcessRuntime, IndexedDb).ConfigureAwait(true);

            await base.OnAppStart().ConfigureAwait(true);

            Logging.Log.LogInfo($"[ProjectRazorPage-({GetType()}]({nameof(OnAppStart)}): Finish");
        }

        /// <inheritdoc />
        protected override Task OnViewCreate()
        {
            Logging.Log.LogTrace($"[ProjectRazorPage-({GetType()}]({nameof(OnViewCreate)}): ");
            return base.OnViewCreate();
        }

        /// <inheritdoc />
        protected override Task OnViewAppearing()
        {
            Logging.Log.LogTrace($"[ProjectRazorPage-({GetType()}]({nameof(OnViewAppearing)}): ");
            return base.OnViewAppearing();
        }

        /// <inheritdoc />
        protected override Task OnViewActivated()
        {
            Logging.Log.LogTrace($"[ProjectRazorPage-({GetType()}]({nameof(OnViewActivated)}): ");
            return base.OnViewActivated();
        }

        /// <inheritdoc />
        protected override Task OnViewLoaded()
        {
            Logging.Log.LogTrace($"[ProjectRazorPage-({GetType()}]({nameof(OnViewLoaded)}): ");
            return base.OnViewLoaded();
        }

        #endregion
    }
}