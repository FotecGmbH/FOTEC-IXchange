// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 23.1.2024 10:38
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Linq;
using System.Threading.Tasks;
using BaseApp.ViewModel.Infrastructure;
using Radzen;

namespace BlazorApp.Pages
{
    /// <summary>
    /// ViewEditIotDevice
    /// </summary>
    public partial class ViewEditIotDevice
    {
        private bool _isBoxIdInfoModalOpen;
        private bool _isConverterTypeInfoModalOpen;
        private bool _isPlatformInfoModalOpen;
        private bool _isSecretInfoModalOpen;
        private bool _isUpstreamInfoModalOpen;

        private async Task ShowBoxIdInfoModal()
        {
            if (!_isBoxIdInfoModalOpen)
            {
                _isBoxIdInfoModalOpen = true;

                await RadzenDialogService.OpenAsync(string.Empty, BoxIdInfoModal, new DialogOptions
                                                                                  {
                                                                                      Width = "25%",
                                                                                      CloseDialogOnOverlayClick = true,
                                                                                      ShowTitle = false,
                                                                                      ShowClose = false
                                                                                  }).ConfigureAwait(true);

                _isBoxIdInfoModalOpen = false;
            }
        }

        private async Task ShowPlatformInfoModal()
        {
            if (!_isPlatformInfoModalOpen)
            {
                _isPlatformInfoModalOpen = true;

                await RadzenDialogService.OpenAsync(string.Empty, PlatformInfoModal, new DialogOptions
                                                                                     {
                                                                                         Width = "25%",
                                                                                         CloseDialogOnOverlayClick = true,
                                                                                         ShowTitle = false,
                                                                                         ShowClose = false
                                                                                     }).ConfigureAwait(true);

                _isPlatformInfoModalOpen = false;
            }
        }

        private async Task ShowUpstreamInfoModal()
        {
            if (!_isUpstreamInfoModalOpen)
            {
                _isUpstreamInfoModalOpen = true;

                await RadzenDialogService.OpenAsync(string.Empty, UpstreamInfoModal, new DialogOptions
                                                                                     {
                                                                                         Width = "25%",
                                                                                         CloseDialogOnOverlayClick = true,
                                                                                         ShowTitle = false,
                                                                                         ShowClose = false
                                                                                     }).ConfigureAwait(true);

                _isUpstreamInfoModalOpen = false;
            }
        }

        private async Task ShowConverterTypeInfoModal()
        {
            if (!_isConverterTypeInfoModalOpen)
            {
                _isConverterTypeInfoModalOpen = true;

                await RadzenDialogService.OpenAsync(string.Empty, ConverterTypeInfoModal, new DialogOptions
                                                                                          {
                                                                                              Width = "25%",
                                                                                              CloseDialogOnOverlayClick = true,
                                                                                              ShowTitle = false,
                                                                                              ShowClose = false
                                                                                          }).ConfigureAwait(true);

                _isConverterTypeInfoModalOpen = false;
            }
        }

        private async Task ShowSecretInfoModal()
        {
            if (!_isSecretInfoModalOpen)
            {
                _isSecretInfoModalOpen = true;

                await RadzenDialogService.OpenAsync(string.Empty, SecretInfoModal, new DialogOptions
                                                                                   {
                                                                                       Width = "25%",
                                                                                       CloseDialogOnOverlayClick = true,
                                                                                       ShowTitle = false,
                                                                                       ShowClose = false
                                                                                   }).ConfigureAwait(true);

                _isSecretInfoModalOpen = false;
            }
        }

        private bool CheckCurrentViewState(ViewState state, ViewElement element)
        {
            switch (state)
            {
                case ViewState.Default:
                    return new[]
                           {
                               ViewElement.MeasurementInterval,
                               ViewElement.TransmissionInterval,
                               ViewElement.Upstream,
                               ViewElement.TransmissionType
                           }.Contains(element);
                case ViewState.Prebuilt:
                    return new[]
                           {
                               ViewElement.Upstream,
                               ViewElement.ConverterType
                           }.Contains(element);
                case ViewState.PrebuiltCustomcode:
                    return new[]
                           {
                               ViewElement.Upstream,
                               ViewElement.CodeArea,
                               ViewElement.ConverterType
                           }.Contains(element);
                case ViewState.OpenSense:
                    return new[]
                           {
                               ViewElement.OpensenseBoxId,
                               ViewElement.OpensenseHistoricalData
                           }.Contains(element);
                default:
                    return true;
            }
        }
    }
}