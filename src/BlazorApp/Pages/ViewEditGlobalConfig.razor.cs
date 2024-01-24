// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 23.1.2024 10:38
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System.Collections.Generic;
using System.Threading.Tasks;
using Exchange.Resources;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages
{
    /// <summary>
    /// ViewEditGlobalConfig
    /// </summary>
    public partial class ViewEditGlobalConfig
    {
        private readonly Dictionary<string, RenderFragment> dictionary = new Dictionary<string, RenderFragment>();

        /// <inheritdoc />
        protected override Task OnInitializedAsync()
        {
            dictionary.Add(ResViewEditGlobalConfig.LblTtnName, TtnNameRenderFragment);
            dictionary.Add(ResViewEditGlobalConfig.LblTtnDescription, TtnDescriptionRenderFragment);
            dictionary.Add(ResViewEditGlobalConfig.LblTtnZone, TtnZoneRenderFragment);
            dictionary.Add(ResViewEditGlobalConfig.LblTtnApiKey, TtnApiKeyRenderFragment);
            dictionary.Add(ResViewEditGlobalConfig.LblTtnApplicationId, TtnAppIdRenderFragment);
            dictionary.Add(ResViewEditGlobalConfig.LblTtnUserId, TtnUserIdRenderFragment);

            return base.OnInitializedAsync();
        }
    }
}