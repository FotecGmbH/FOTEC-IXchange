// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Threading.Tasks;
using BaseApp.Helper;
using BaseApp.ViewModel.ModalVM;
using Biss.Apps.Attributes;
using Biss.Apps.Interfaces;
using Biss.Apps.ViewModel;
using Exchange.Model.ConfigApp;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    /// <para>Viewmodel für ViewEditAbo</para>
    /// Klasse VmEditAbo. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewEditAbo", true)]
    public class VmEditAbo : VmEditDcListPoint<ExAbo>
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmEditAbo.DesignInstance}"
        /// </summary>
        public static VmEditAbo DesignInstance = new VmEditAbo();

        /// <summary>
        ///     VmEditAbo
        /// </summary>
        public VmEditAbo() : base(ResViewEditAbo.LblPageTitle)
        {
            //SetViewProperties(true);
            View.ShowMenu = true;
        }

        #region Entries

        /// <summary>
        /// Entry für Wert Überschreitung
        /// </summary>
        public VmEntry EntryExceedNotifyValue { get; set; } = new VmEntry();

        /// <summary>
        /// Entry für Wert Unterschreitung
        /// </summary>
        public VmEntry EntryUndercutNotifyValue { get; set; } = new VmEntry();

        /// <summary>
        /// Entry für Wert Ausfallzeit in Minuten
        /// </summary>
        public VmEntry EntryFailureForMinutesNotifyValue { get; set; } = new VmEntry();

        /// <summary>
        /// Entry für Wert Abweichung Mittelwert
        /// </summary>
        public VmEntry EntryMovingAverageNotifyValue { get; set; } = new VmEntry();

        /// <summary>
        /// Initialisieren der Entries
        /// </summary>
        private void InitEntries()
        {
            EntryExceedNotifyValue = new VmEntry(
                rootObject: Data,
                rootObjectPropertyName: nameof(Data.ExceedNotifyValue),
                showMaxChar: false,
                showTitle: false,
                validateFunc: VmEntryValidators.ValidateFuncDouble);

            EntryUndercutNotifyValue = new VmEntry(
                rootObject: Data,
                rootObjectPropertyName: nameof(Data.UndercutNotifyValue),
                showMaxChar: false,
                showTitle: false,
                validateFunc: VmEntryValidators.ValidateFuncDouble);

            EntryFailureForMinutesNotifyValue = new VmEntry(
                rootObject: Data,
                rootObjectPropertyName: nameof(Data.FailureForMinutesNotifyValue),
                showMaxChar: false,
                showTitle: false,
                validateFunc: VmEntryValidators.ValidateFuncDouble);

            EntryMovingAverageNotifyValue = new VmEntry(
                rootObject: Data,
                rootObjectPropertyName: nameof(Data.MovingAverageNotifyValue),
                showMaxChar: false,
                showTitle: false,
                validateFunc: VmEntryValidators.ValidateFuncDouble);
        }

        #endregion

        #region Overrides

        /// <summary>
        ///     OnAppearing (1) für View geladen noch nicht sichtbar
        ///     Wird Mal wenn View wieder sichtbar ausgeführt
        ///     Unbedingt beim überschreiben auch base. aufrufen!
        /// </summary>
        public override async Task OnAppearing(IView view)
        {
            await Dc.DcExAbos.Sync().ConfigureAwait(true);
            await base.OnAppearing(view).ConfigureAwait(true);
        }

        /// <summary>
        ///     OnActivated (2) für View geladen noch nicht sichtbar
        ///     Nur einmal
        /// </summary>
        public override async Task OnActivated(object? args = null)
        {
            await Dc.DcExAbos.Sync().ConfigureAwait(true);
            await base.OnActivated(args).ConfigureAwait(true);
        }

        /// <summary>
        ///     OnLoaded (3) für View geladen
        ///     Jedes Mal wenn View wieder sichtbar
        /// </summary>
        public override Task OnLoaded()
        {
            InitEntries();
            CmdOpenModal.DisplayName = Data.MeasurementDefinitionAssignment.NumberOfRatingsAsString;
            return base.OnLoaded();
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdOpenModal = new VmCommand(string.Empty, async () => { _ = await View.DetailView.ShowVmDetailWithResult(typeof(VmRatingModal), Data.MeasurementDefinitionAssignment.MeasurementDefinition).ConfigureAwait(true); });
            base.InitializeCommands();
        }

        /// <summary>
        /// Modal oeffnen (nur XF).
        /// </summary>
        public VmCommand CmdOpenModal { get; set; } = null!;

        #endregion
    }
}