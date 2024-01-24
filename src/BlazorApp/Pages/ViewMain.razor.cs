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
using BaseApp.Connectivity;
using Biss.Collections;
using BlazorApp.Components.Modals;
using Radzen;

namespace BlazorApp.Pages
{
    /// <summary>
    /// ViewMain
    /// </summary>
    public partial class ViewMain
    {
        private bool _isFilterSensorMenuOpen;

        /// <inheritdoc />
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (ViewModel is not null)
            {
                ViewModel.Dc.DcExIotDevices.SelectedItemChanged -= DcExIotDevices_SelectedItemChanged!;
                ViewModel.Dc.DcExIotDevices.SelectedItemChanged += DcExIotDevices_SelectedItemChanged!;
            }

            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        ///     Entspricht VM OnDisappearing, wird einmal pro View aufgerufen
        /// </summary>
        /// <returns></returns>
        protected override Task OnViewDisappearing()
        {
            if (ViewModel is not null)
            {
                ViewModel.Dc.DcExIotDevices.SelectedItemChanged -= DcExIotDevices_SelectedItemChanged!;
            }

            return base.OnViewDisappearing();
        }

        private void DcExIotDevices_SelectedItemChanged(object sender, SelectedItemEventArgs<DcListTypeIotDevice> e)
        {
            InvokeDispatcherAsync(async () =>
            {
                var parameters = new Dictionary<string, object>
                                 {
                                     {nameof(IotDeviceDataModal.DcListTypeIotDevice), e.CurrentItem},
                                 };

                await RadzenDialogService.OpenSideAsync<IotDeviceDataModal>(string.Empty, parameters, new SideDialogOptions
                                                                                                      {
                                                                                                          Position = DialogPosition.Right,
                                                                                                          ShowMask = false,
                                                                                                          Width = "25vw",
                                                                                                          Height = "75vh",
                                                                                                          Style = "top: 12.5vh",
                                                                                                          ShowTitle = true,
                                                                                                          ShowClose = true,
                                                                                                          CloseDialogOnOverlayClick = true
                                                                                                      }).ConfigureAwait(true);
            });
        }

        private async Task OpenFilterSensorsMenu()
        {
            if (!_isFilterSensorMenuOpen)
            {
                _isFilterSensorMenuOpen = true;

                await RadzenDialogService.OpenAsync("Sensorarten", FilterSensorsFragment, new DialogOptions
                                                                                          {
                                                                                              Width = "25%",
                                                                                              CloseDialogOnOverlayClick = true,
                                                                                              CloseDialogOnEsc = true,
                                                                                              ShowClose = true,
                                                                                          }).ConfigureAwait(true);

                _isFilterSensorMenuOpen = false;
            }
        }
    }
}