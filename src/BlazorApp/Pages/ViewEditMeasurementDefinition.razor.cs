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
    /// ViewEditMeasurementDefinition
    /// </summary>
    public partial class ViewEditMeasurementDefinition
    {
        private readonly Dictionary<string, RenderFragment> dictionary = new Dictionary<string, RenderFragment>();

        private readonly Dictionary<string, RenderFragment> dictionaryAdvancedMode = new Dictionary<string, RenderFragment>();

        /// <inheritdoc />
        protected override Task OnInitializedAsync()
        {
            dictionary.Add(ResViewEditMeasurementDefinition.LblName, NameRenderFragment);
            dictionary.Add(ResViewEditMeasurementDefinition.LblDescription, DescriptionRenderFragment);
            dictionary.Add(ResViewEditMeasurementDefinition.LblType, TypeRenderFragment);
            dictionary.Add(ResViewEditMeasurementDefinition.LblSensorConnection, DownStreamTypeRenderFragment);
            dictionary.Add(ResViewEditMeasurementDefinition.LblAvailableTypes, PredefinedMeasurementsRenderFragment);
            dictionary.Add(ResViewEditMeasurementDefinition.LblResearchInstituteAccessGranted, ResearchInstituteAccessGrantedRenderFragment);
            //dictionary.Add(ResViewEditAbo.LblMessages, MessagesRenderFragment);
            dictionary.Add(ResViewEditAbo.LblExceedValue, ExceedRenderFragment);
            dictionary.Add(ResViewEditAbo.LblUndercutValue, UndercutRenderFragment);
            dictionary.Add(ResViewEditAbo.LblFailureMinutes, FailureMinutesRenderFragment);
            dictionary.Add(ResViewEditAbo.LblMovingAverage, MovingAverageRenderFragment);
            dictionary.Add(ResViewEditMeasurementDefinition.LblMeasurementRated, MeasurementRatedSubscriptionRenderFragment);
            //dictionary.Add(ResViewEditMeasurementDefinition.LblMeasurementSubscribed, MeasurementSubscribedNotificationRenderFragment);
            //dictionary.Add(ResViewEditMeasurementDefinition.LblMeasurementUnsubscribed, MeasurementUnsubscribedNotificationRenderFragment);


            dictionaryAdvancedMode.Add(ResViewEditMeasurementDefinition.LblAdditionalSettings, AdditionalSettingsRenderFragment);

            return base.OnInitializedAsync();
        }
    }
}