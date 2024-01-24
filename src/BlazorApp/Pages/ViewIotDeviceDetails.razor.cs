// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 23.1.2024 10:38
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exchange.Resources;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages
{
    /// <summary>
    /// ViewIotDeviceDetails
    /// </summary>
    public partial class ViewIotDeviceDetails
    {
        private readonly Dictionary<string, RenderFragment> dictionary = new Dictionary<string, RenderFragment>();

        private readonly Dictionary<string, RenderFragment> dictionaryAdvancedMode = new Dictionary<string, RenderFragment>();

        /// <inheritdoc />
        protected override Task OnInitializedAsync()
        {
            dictionary.Add(ResViewIotDeviceDetails.LblName, NameRenderFragment);
            dictionary.Add(ResViewIotDeviceDetails.LblDescription, DescriptionRenderFragment);
            dictionary.Add(ResViewIotDeviceDetails.LblLocalisation, LocalisationRenderFragment);
            dictionary.Add(ResViewIotDeviceDetails.LblCoordinates, CoordinatesRenderFragment);
            dictionary.Add(ResViewIotDeviceDetails.LblAdvancedMode, AdvancedModeRenderFragment);

            dictionaryAdvancedMode.Add(ResViewIotDeviceDetails.LblGateway, GatewayRenderFragment);
            dictionaryAdvancedMode.Add(ResViewIotDeviceDetails.LblUpstream, UpstreamRenderFragment);
            dictionaryAdvancedMode.Add(ResViewIotDeviceDetails.LblPlatform, PlatformRenderFragment);
            dictionaryAdvancedMode.Add(ResViewIotDeviceDetails.LblTransmission, TransmissionRenderFragment);
            dictionaryAdvancedMode.Add(ResViewIotDeviceDetails.LblTransmissionInterval, TransmissionIntervalRenderFragment);
            dictionaryAdvancedMode.Add(ResViewIotDeviceDetails.LblMeasurementInterval, MeasurementIntervalRenderFragment);
            dictionaryAdvancedMode.Add(ResViewIotDeviceDetails.LblAdditionalSettings, AdditionalSettingsRenderFragment);
            return base.OnInitializedAsync();
        }
    }
}