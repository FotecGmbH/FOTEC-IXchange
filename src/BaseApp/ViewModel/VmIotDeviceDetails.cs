// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using BaseApp.Connectivity;
using BaseApp.ViewModel.Infrastructure;
using BDA.Common.Exchange.Configs.Downstreams.Virtual;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Apps.Attributes;
using Biss.Apps.Collections;
using Biss.Apps.Enum;
using Biss.Apps.Interfaces;
using Biss.Apps.ViewModel;
using Biss.Log.Producer;
using Exchange.Enum;
using Exchange.Resources;
using Microsoft.Extensions.Logging;


namespace BaseApp.ViewModel
{
    /// <summary>
    /// <para>DESCRIPTION</para>
    /// Klasse VmIotDeviceDetails. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewIotDeviceDetails", true)]
    public class VmIotDeviceDetails : VmEditDcListPoint<ExIotDevice>
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmIotDeviceDetails.DesignInstance}"
        /// </summary>
        public static VmIotDeviceDetails DesignInstance = new VmIotDeviceDetails();

        /// <summary>
        /// Ob events angehaengt sind
        /// </summary>
        private bool _eventsattached;

        /// <summary>
        ///     VmIotDeviceDetails
        /// </summary>
        public VmIotDeviceDetails() : base(ResViewIotDeviceDetails.LblPageTitle)
        {
            SetViewProperties();
        }

        #region Properties

        /// <summary>
        ///     Measurement Definitions.
        /// </summary>
        public ICollection<ExMeasurementDefinition> MeasurementDefinitions { get; set; } = new List<ExMeasurementDefinition>();

        /// <summary>
        /// Ob eigener Sensor
        /// </summary>
        public bool IsOwnIotDevice { get; set; }

        /// <summary>
        /// Advanced Mode ein/aus
        /// </summary>
        public bool AdvancedMode { get; set; }

        #endregion

        /// <summary>
        ///     ViewModel Events
        /// </summary>
        /// <param name="attach"></param>
        private void AttachDetachVmEvents(bool attach)
        {
            if (attach)
            {
                if (!_eventsattached)
                {
                    Dc.DcExIotDevices.CollectionEvent += DcExIotDevices_CollectionEvent;
                    Dc.DcExMeasurementDefinition.CollectionEvent += DcExMeasurementDefinition_CollectionEvent_Details;
                    Dc.DcExMeasurementDefinition.CollectionChanged += DcExMeasurementDefinition_CollectionChanged;
                    _eventsattached = true;
                }
            }
            else
            {
                if (_eventsattached)
                {
                    Dc.DcExIotDevices.CollectionEvent -= DcExIotDevices_CollectionEvent;
                    Dc.DcExMeasurementDefinition.CollectionEvent -= DcExMeasurementDefinition_CollectionEvent_Details;
                    Dc.DcExMeasurementDefinition.CollectionChanged -= DcExMeasurementDefinition_CollectionChanged;
                    _eventsattached = false;
                }
            }
        }

        /// <summary>
        /// Messwertdefinitionen collection hat sich geaendert
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">even argumente</param>
        private void DcExMeasurementDefinition_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            FillMeasurements();
        }

        /// <summary>
        /// Messwerte befuellen
        /// </summary>
        private void FillMeasurements()
        {
            var measurements = Dc.DcExMeasurementDefinition.Select(x => x.Data).ToList();
            var measurementlist = new List<ExMeasurementDefinition>(measurements);
            MeasurementDefinitions = measurementlist;
        }

        /// <summary>
        /// Messwertdefinitionen collection details
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">event argumente</param>
        /// <returns>Task</returns>
        private async Task DcExMeasurementDefinition_CollectionEvent_Details(object sender, CollectionEventArgs<DcListTypeMeasurementDefinition> e)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (e is null)
            {
                return;
            }

            switch (e.TypeOfEvent)
            {
                case EnumCollectionEventType.AddRequest:
                {
                    var newMd = new GcDownstreamVirtualFloat().ToExMeasurementDefinition();
                    newMd.IotDeviceId = DcListDataPoint.Index;
                    newMd.MeasurementInterval = 100;
                    var item = new DcListTypeMeasurementDefinition(newMd);

                    Dc.DcExMeasurementDefinition.Add(item);

                    await EditMeasurementDefinition(item, true).ConfigureAwait(true);

                    break;
                }
                case EnumCollectionEventType.EditRequest:
                {
                    await EditMeasurementDefinition(e.Item, false).ConfigureAwait(true);
                    break;
                }
                case EnumCollectionEventType.DeleteRequest:
                {
                    var answer = await MsgBox.Show("Wirklich löschen", ResCommon.CmdDelete, icon: VmMessageBoxImage.Warning, button: VmMessageBoxButton.YesNo).ConfigureAwait(true);

                    if (answer != VmMessageBoxResult.Yes)
                    {
                        break;
                    }

                    if (Dc.DcExMeasurementDefinition.Remove(e.Item))
                    {
                        await Dc.DcExMeasurementDefinition.StoreAll().ConfigureAwait(true);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Bearbeiten einer messwertdefinition
        /// </summary>
        /// <param name="item">item</param>
        /// <param name="isNew">ob neu</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task EditMeasurementDefinition(DcListTypeMeasurementDefinition item, bool isNew)
        {
            AttachDetachVmEvents(false);
            var r = await Nav.ToViewWithResult(typeof(VmEditMeasurementDefinition), item).ConfigureAwait(true);
            await View.RefreshAsync().ConfigureAwait(true);
            AttachDetachVmEvents(true);
            if (r is EnumVmEditResult result)
            {
                if (isNew && result != EnumVmEditResult.ModifiedAndStored)
                {
                    // Workaround bis Fix in der Liste 
                    try
                    {
                        Dc.DcExMeasurementDefinition.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        Logging.Log.LogError($"{ex}");
                    }
                }
            }
            else
            {
                throw new ArgumentException("Wrong result!");
            }

            AttachDetachVmEvents(true);
        }

        /// <summary>
        /// Measurements filtern
        /// </summary>
        private void FilterMeasurements()
        {
            Dc.DcExMeasurementDefinition.FilterList(x => x.Data.IotDeviceId == DcListDataPoint.Index);
        }

        #region Overrides

        /// <summary>
        ///     OnAppearing (1) für View geladen noch nicht sichtbar
        ///     Wird Mal wenn View wieder sichtbar ausgeführt
        ///     Unbedingt beim überschreiben auch base. aufrufen!
        /// </summary>
        public override Task OnAppearing(IView view)
        {
            AttachDetachVmEvents(true);
            return base.OnAppearing(view);
        }

        /// <summary>
        ///     OnActivated (2) für View geladen noch nicht sichtbar
        ///     Nur einmal
        /// </summary>
        public override async Task OnActivated(object? args = null)
        {
            await base.OnActivated(args).ConfigureAwait(true);
            FilterMeasurements();
            IsOwnIotDevice = Dc.DcExUser.Data.HasPermissionTo(Data);
            //await Dc.DcExMeasurementDefinition.Sync().ConfigureAwait(true);
            //await Dc.DcExMeasurementDefinition.WaitDataFromServerAsync(reload: true).ConfigureAwait(true);
            //await Dc.DcExMeasurementDefinitionAssignments.WaitDataFromServerAsync(reload: true).ConfigureAwait(true);
        }

        /// <summary>
        ///     OnLoaded (3) für View geladen
        ///     Jedes Mal wenn View wieder sichtbar
        /// </summary>
        public override Task OnLoaded()
        {
            FillMeasurements();
            return base.OnLoaded();
        }

        /// <summary>
        ///     OnDisappearing (4) wenn View unsichtbar / beendet wird
        ///     Nur einmal
        /// </summary>
        public override Task OnDisappearing(IView view)
        {
            AttachDetachVmEvents(false);
            return base.OnDisappearing(view);
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            CmdShowDeviceOnMap = new VmCommand(ResViewIotDeviceDetails.LblShowOnMap, async () =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                Dc.DcExIotDevices.CmdSelectItem.Execute(DcListDataPoint);
                _ = Nav.ToViewWithResult(typeof(VmMain), cachePage: true);
                GetVmBaseStatic.CmdAllMenuCommands.SelectedItem = GetVmBaseStatic.GCmdHome;
            }, glyph: Glyphs.Maps_search);
        }

        /// <summary>
        ///     Karte öffnen und Pos auswählen Command
        /// </summary>
        public VmCommand CmdShowDeviceOnMap { get; set; } = null!;

        /// <summary>
        /// Iot devices collection geaendert
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task DcExIotDevices_CollectionEvent(object sender, CollectionEventArgs<DcListTypeIotDevice> e)
        {
            switch (e.TypeOfEvent)
            {
                case EnumCollectionEventType.EditRequest:
                {
                    var r = await Nav.ToViewWithResult(typeof(VmEditIotDevice), e.Item).ConfigureAwait(true);
                    await View.RefreshAsync().ConfigureAwait(true);
                    if (r is EnumVmEditResult result)
                    {
                        if (result != EnumVmEditResult.ModifiedAndStored)
                        {
                            if (e.Item.PossibleNewDataOnServer)
                            {
                                e.Item.Update();
                            }
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Wrong result!");
                    }

                    AttachDetachVmEvents(true);
                    break;
                }
            }
        }

        #endregion
    }
}