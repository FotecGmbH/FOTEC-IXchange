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
using BaseApp.Connectivity;
using Biss.Collections;
using BlazorApp.Components;
using Exchange.Extensions;
using Radzen;

namespace BlazorApp.Pages
{
    public partial class ViewMyRatings
    {
        /// <inheritdoc />
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (ViewModel is not null)
            {
                ViewModel.Dc.DcExRatings.SelectedItemChanged -= DcExRatings_SelectedItemChanged;
                ViewModel.Dc.DcExRatings.SelectedItemChanged += DcExRatings_SelectedItemChanged;
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
                ViewModel.Dc.DcExRatings.SelectedItemChanged -= DcExRatings_SelectedItemChanged;
            }

            return base.OnViewDisappearing();
        }

        private void DcExRatings_SelectedItemChanged(object? sender, SelectedItemEventArgs<DcListTypeRating> e)
        {
            OpenEditRating(e.CurrentItem).ConfigureAwait(true);
        }

        private async Task OpenEditRating(DcListTypeRating rating)
        {
            if (rating is null)
            {
                return;
            }

            var parameters = new Dictionary<string, object>
                             {
                                 {nameof(EditRatingComponent.Rating), rating}
                             };

            await InvokeDispatcherAsync(async () =>
            {
                await RadzenDialogService.OpenAsync<EditRatingComponent>(
                    rating.Data?.MeasurementDefinitionAssignment?.Type.GetDisplayName() ?? string.Empty,
                    parameters,
                    new DialogOptions
                    {
                        Width = "25%",
                        CloseDialogOnOverlayClick = true,
                        CloseDialogOnEsc = true
                    }
                ).ConfigureAwait(true);
            }).ConfigureAwait(true);
        }
    }
}