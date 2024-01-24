// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Linq;
using System.Threading.Tasks;
using BaseApp.Connectivity;
using BaseApp.Helper;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Apps.Interfaces;
using Biss.Apps.ViewModel;
using Biss.Dc.Core;
using Exchange.Model.ConfigApp;

namespace BaseApp.ViewModel.ModalVM
{
    /// <summary>
    /// <para>Kommentar Modal</para>
    /// Klasse VmRatingModal. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class VmRatingModal : VmProjectBase
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmRatingModal.DesignInstance}"
        /// </summary>
        public static VmRatingModal DesignInstance = new VmRatingModal();

        /// <summary>
        ///     VmRatingModal
        /// </summary>
        public VmRatingModal() : base(string.Empty)
        {
            SetViewProperties();
        }

        #region Properties

        /// <summary>
        ///     Eigene Bewertung.
        /// </summary>
        public DcListTypeRating OwnRatingDataPoint { get; set; } = null!;

        /// <summary>
        ///     Measurement Definition
        /// </summary>
        public ExMeasurementDefinition MeasurementDefinition { get; set; } = null!;

        #endregion

        #region Overrides

        /// <summary>
        ///     OnActivated (2) für View geladen noch nicht sichtbar
        ///     Nur einmal
        /// </summary>
        public override Task OnActivated(object? args = null)
        {
            if (!(args is ExMeasurementDefinition measurementDefinition))
            {
                return Nav.Back();
            }

            MeasurementDefinition = measurementDefinition;

            return base.OnActivated(args);
        }

        /// <summary>
        ///     OnLoaded (3) für View geladen
        ///     Jedes Mal wenn View wieder sichtbar
        /// </summary>
        public override async Task OnLoaded()
        {
            await Dc.DcExRatings.Sync().ConfigureAwait(true);
            Dc.DcExRatings.FilterList(x => ExMesDefAssignHelper.GetAssignment(x.Data.MeasurementDefinitionAssignment, Dc).Id == MeasurementDefinition.Id);
            var ownRating = /*ExMesDefAssignHelper.GetAssignment(MeasurementDefinition,Dc).Data.Ratings*/Dc.DcExRatings.FirstOrDefault(x => x.Data.User.Id == Dc.DcExUser.Data.Id);

            OwnRatingDataPoint = Dc.DcExRatings.FirstOrDefault(x => x.Index == ownRating?.Id) ??
                                 new DcListTypeRating(new ExRating
                                                      {
                                                          User = Dc.DcExUser.Data,
                                                          MeasurementDefinitionAssignment = ExMesDefAssignHelper.GetAssignment(MeasurementDefinition, Dc).Data
                                                      });

            await base.OnLoaded().ConfigureAwait(false);
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdSaveOwnRating = new VmCommand("Bestätigen", async () =>
            {
                if (OwnRatingDataPoint.Data.Id < 1)
                {
                    Dc.DcExRatings.Add(OwnRatingDataPoint);
                }
                else
                {
                    OwnRatingDataPoint.State = EnumDcListElementState.Modified;
                }

                var storeResult = await OwnRatingDataPoint.StoreData(true).ConfigureAwait(true);

                if (storeResult.DataOk)
                {
                    await Nav.Back().ConfigureAwait(true);
                }
                else
                {
                    var answer = await MsgBox.Show($"{storeResult.ServerExceptionText}<br>Trotzdem schließen?", "Error", VmMessageBoxButton.YesNo, VmMessageBoxImage.Error).ConfigureAwait(true);

                    if (answer is VmMessageBoxResult.Yes)
                    {
                        await Nav.Back().ConfigureAwait(true);
                    }
                }
            });
        }

        /// <summary>
        /// Cmd um Rating zu speichern
        /// </summary>
        public VmCommand CmdSaveOwnRating { get; set; } = null!;

        #endregion
    }
}