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
    public partial class ViewEditAbo
    {
        private readonly Dictionary<object, RenderFragment> dictionary = new Dictionary<object, RenderFragment>();

        /// <inheritdoc />
        protected override async Task OnInitializedAsync()
        {
            await Task.Delay(100).ConfigureAwait(true); // ansonsten NullReferenceException

            dictionary.Add(ResViewEditAbo.LblName, NameRenderFragment);
            dictionary.Add(ResViewEditAbo.LblDescription, DescriptionRenderFragment);
            dictionary.Add(ResViewEditAbo.LblType, TypeRenderFragment);
            dictionary.Add(ResViewEditAbo.LblSensorConnection, SensorConnectionRenderFragment);
            dictionary.Add(GetSubheadingLabel($"{ResViewEditAbo.LblMessagesFor}:", false), null!);
            dictionary.Add(GetIndentedLabel(ResViewEditAbo.LblExceedValue), ExceedRenderFragment);
            dictionary.Add(GetIndentedLabel(ResViewEditAbo.LblUndercutValue), UndercutRenderFragment);
            dictionary.Add(GetIndentedLabel(ResViewEditAbo.LblFailureMinutes), FailureMinutesRenderFragment);
            dictionary.Add(GetIndentedLabel(ResViewEditAbo.LblMovingAverage), MovingAverageRenderFragment);
            dictionary.Add(ResViewEditAbo.LblCostsValue, CostsRenderFragment);
            dictionary.Add(ResViewEditAbo.LblRatings, RatingRenderFragment);

            await base.OnInitializedAsync().ConfigureAwait(true);
            //return base.OnInitializedAsync();
        }

        private async Task OpenRatingsModal()
        {
            await OpenRatingsModal(ViewModel.Data.MeasurementDefinitionAssignment.MeasurementDefinition).ConfigureAwait(true);

            ViewModel.DcListDataPoint.Update();
            StateHasChanged();
        }
    }
}