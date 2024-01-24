// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 23.1.2024 10:38
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Threading.Tasks;
using Exchange.Resources;
using Radzen;

namespace BlazorApp.Pages
{
    public partial class ViewIncomeOutput
    {
        private bool _isHowToCollectIxiesModalOpen;
        private bool _isHowToSpendIxiesModalOpen;

        private async Task ShowHowToCollectIxiesModal()
        {
            if (!_isHowToCollectIxiesModalOpen)
            {
                _isHowToCollectIxiesModalOpen = true;

                await RadzenDialogService.OpenAsync(ResViewIncomeOutput.TxtHowToCollectIxies, HowToCollectIxiesModal, new DialogOptions
                                                                                                                      {
                                                                                                                          Width = "25%",
                                                                                                                          CloseDialogOnOverlayClick = true,
                                                                                                                      }).ConfigureAwait(true);

                _isHowToCollectIxiesModalOpen = false;
            }
        }

        private async Task ShowHowToSpendIxiesModal()
        {
            if (!_isHowToSpendIxiesModalOpen)
            {
                _isHowToSpendIxiesModalOpen = true;

                await RadzenDialogService.OpenAsync(ResViewIncomeOutput.TxtHowToSpendIxies, HowToSpendIxiesModal, new DialogOptions
                                                                                                                  {
                                                                                                                      Width = "25%",
                                                                                                                      CloseDialogOnOverlayClick = true,
                                                                                                                  }).ConfigureAwait(true);

                _isHowToSpendIxiesModalOpen = false;
            }
        }
    }
}