// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BaseApp.Connectivity;
using BaseApp.ViewModel.Infrastructure;
using BaseApp.ViewModel.ModalVM;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Apps.Attributes;
using Biss.Apps.Collections;
using Biss.Apps.Enum;
using Biss.Apps.Interfaces;
using Biss.Apps.Map.Base;
using Biss.Apps.Map.Component;
using Biss.Apps.Map.Helper;
using Biss.Apps.Map.Interface;
using Biss.Apps.Map.Model;
using Biss.Apps.ViewModel;
using Biss.Collections;
using Biss.Common;
using Biss.Log.Producer;
using Exchange;
using Exchange.Enum;
using Exchange.Extensions;
using Exchange.Model;
using Exchange.Model.ConfigApp;
using Exchange.Resources;
using Microsoft.Extensions.Logging;

namespace BaseApp.ViewModel
{
    /// <summary>
    ///     <para>Hauptansicht - Projekte und Messwerte</para>
    /// Klasse ViewModelUserAccount. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewMain")]
    public class VmMain : VmProjectBase
    {
        /// <summary>
        /// Ob events angehaengt
        /// </summary>
        private bool _eventsattached;

        /// <summary>
        /// Selektierte Iot Device id
        /// </summary>
#pragma warning disable CS0169 // Field is never used
        private long? _selectedIotDeviceId;
#pragma warning restore CS0169 // Field is never used

        /// <summary>
        /// Suchfeld
        /// </summary>
        private string _sensorSearch = string.Empty;

        /// <summary>
        ///     ViewModel Template
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public VmMain() : base(ResViewMain.LblTitle, subTitle: ResViewMain.LblSubTitle)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            SetViewProperties();
        }

        #region Properties

        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmEntry.DesignInstance}"
        /// </summary>
        public static VmMain DesignInstance => new VmMain();

        /// <summary>
        ///     Wrapper BissMap
        /// </summary>
        public BissMap Map => this.BcBissMap()!.BissMap;

        /// <summary>
        /// Suche von Sensoren in Karte
        /// </summary>
        public string SensorSearch
        {
            get => _sensorSearch;
            set
            {
                _sensorSearch = value;
                UpdateMapItems();
            }
        }

        /// <summary>
        /// Gefilterte Sensortypen
        /// </summary>
        public List<EnumMeasurementType> TypesFilter { get; set; } = new List<EnumMeasurementType>();

        #endregion

        /// <summary>
        /// Filter aenderung
        /// </summary>
        /// <param name="value">Wert</param>
        /// <param name="measurementType">Type</param>
        /// <returns>task</returns>
        public Task ChangeFilter(bool value, EnumMeasurementType measurementType)
        {
            if (value)
            {
                if (!TypesFilter.Contains(measurementType))
                {
                    TypesFilter.Add(measurementType);
                }
            }
            else
            {
                TypesFilter.Remove(measurementType);
            }

            UpdateMapItems();
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Aktuelle Position für Map
        /// </summary>
        /// <returns></returns>
        public async Task GoToCurrentUserLocation()
        {
            try
            {
                var loc = await this.BcBissMap()!.GetUserLocation().ConfigureAwait(true);

                if (loc != null)
                {
                    Map.SetCenterAndZoom(loc, BmDistance.FromKilometers(5));
                }
            }
            catch (Exception e)
            {
                Logging.Log.LogError($"[{nameof(VmMain)}]({nameof(GoToCurrentUserLocation)}): {e}");
            }
        }

        /// <summary>
        /// Karten-Items aktuallisieren
        /// </summary>
        public void UpdateMapItems()
        {
            IEnumerable<DcListTypeIotDevice> currentItems = Dc.DcExIotDevices;

            IEnumerable<DcListTypeMeasurementDefinitionAssignment> currItemsMeasDefAssignments = Dc.DcExMeasurementDefinitionAssignments;

            //filter auf typen
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            if (TypesFilter?.Any() ?? false)
            {
                currItemsMeasDefAssignments = currItemsMeasDefAssignments.Where(mA => TypesFilter.Contains(mA.Data.Type));
                // ReSharper disable once VariableHidesOuterVariable
                currentItems = Dc.DcExIotDevices.Where(x => x.Data.MeasurementDefinitions.Select(x => x.IotDeviceId).Intersect(currItemsMeasDefAssignments.Select(mA => mA.Data.MeasurementDefinition.IotDeviceId))?.Any() ?? false);
                //currentItems = Dc.DcExIotDevices.Where(x => x.Data.MeasurementDefinitions.Select(x => x.Type).Intersect(TypesFilter)?.Any() ?? false);
            }

            //filter auf texteingabe
            if (!string.IsNullOrWhiteSpace(_sensorSearch))
            {
                //entweder name von iotDevice oder Name,Type von einem der Messwerte
                currentItems = currentItems.Where(x => (x.Data.Information?.Name?.Contains(_sensorSearch, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                                       (x.Data.MeasurementDefinitions?.Any(y => (y.Information?.Name?.Contains(_sensorSearch, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                                                                                currItemsMeasDefAssignments.Any(mA => mA.Data.MeasurementDefinition.Id == y.Id && mA.Data.Type.GetDisplayName().Contains(_sensorSearch, StringComparison.OrdinalIgnoreCase)))
                                                        ?? false)); /*||
                                currItemsMeasDefAssignments.Any(mA=>mA.Data.MeasurementDefinition.Id == y.Id && mA.Data.Type.GetDisplayName().Contains(_sensorSearch, StringComparison.OrdinalIgnoreCase))))));*/
            }


            Map.MapItems.Clear();

            foreach (var dcListTypeIotDevice in currentItems)
            {
                var location = dcListTypeIotDevice.Data.Location;

                var selected = Dc.DcExIotDevices.SelectedItem?.Index == dcListTypeIotDevice.Index;

                Map.MapItems.Add(new BmPoint(dcListTypeIotDevice.Data.Information.UiNameShort)
                                 {
                                     Position = location.ToBissPosition(),
                                     IsVisible = true,
                                     Index = dcListTypeIotDevice.Index,
                                     Label = dcListTypeIotDevice.Data.Information.UiNameShort,
                                     Image = new BmPinImage(selected ? $"{AppSettings.Current().DcSignalHost}/content/PinSelected.png" : $"{AppSettings.Current().DcSignalHost}/content/Pin.png"),
                                     IsSelected = selected
                                 });
            }
        }

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdSubscribe = new VmCommand(ResViewEditAbo.TxtSubscribe, async arg =>
            {
                IEnumerable<ExMeasurementDefinition> measurementDefinitions = new List<ExMeasurementDefinition>();

                if (arg is IEnumerable<ExMeasurementDefinition> measurementDefinitionsEnumerable)
                {
                    measurementDefinitions = measurementDefinitionsEnumerable;
                }

                if (arg is ExMeasurementDefinition measurementDefinition)
                {
                    measurementDefinitions = new List<ExMeasurementDefinition> {measurementDefinition};
                }

                var changed = false;
                foreach (var exMeasurementDefinition in measurementDefinitions)
                {
                    var mesDefAssignment = Dc.DcExMeasurementDefinitionAssignments.FirstOrDefault(mA => mA.Data.MeasurementDefinition.Id == exMeasurementDefinition.Id);
                    if (mesDefAssignment == null)
                    {
                        Dc.DcExMeasurementDefinitionAssignments.Add(new DcListTypeMeasurementDefinitionAssignment(new ExMeasurementDefinitionAssignment
                                                                                                                  {
                                                                                                                      AccessForResearchInstitutesGranted = false,
                                                                                                                      MeasurementDefinition = exMeasurementDefinition,
                                                                                                                      Ratings = new ObservableCollection<ExRating>(),
                                                                                                                      Type = EnumMeasurementType.Other
                                                                                                                  }));
                        await Dc.DcExMeasurementDefinitionAssignments.StoreAll().ConfigureAwait(true);
                        mesDefAssignment = Dc.DcExMeasurementDefinitionAssignments.FirstOrDefault(mA => mA.Data.MeasurementDefinition.Id == exMeasurementDefinition.Id);
                    }


                    var datapoint = Dc.DcExAbos.FirstOrDefault(x => x.Data.User.Id == Dc.DcExUser.Data.Id && x.Data.MeasurementDefinitionAssignment.Id == mesDefAssignment.Id);

                    if (exMeasurementDefinition.IsSelected)
                    {
                        //abonnieren wenn noch nicht abonniert
                        if (datapoint is null)
                        {
                            datapoint = new DcListTypeAbo(new ExAbo
                                                          {
                                                              MeasurementDefinitionAssignment = mesDefAssignment.Data,
                                                              User = Dc.DcExUser.Data,
                                                          });

                            Dc.DcExAbos.Add(datapoint);
                            changed = true;
                        }
                    }
                    else
                    {
                        //deabonnieren falls abonniert
                        if (datapoint != null)
                        {
                            Dc.DcExAbos.Remove(datapoint);
                            changed = true;
                        }
                    }
                }

                if (changed)
                {
                    View.BusySet(ResCommon.MsgPleaseWait, 0);
                    await Dc.DcExAbos.StoreAll().ConfigureAwait(true);
                    await Dc.DcExAbos.Sync().ConfigureAwait(true);
                    View.BusyClear(true);
                }
                else
                {
                    await MsgBox.Show(ResViewMain.MsgBoxNoChangesMade, icon: VmMessageBoxImage.Information).ConfigureAwait(true);
                }
            });

            CmdOpenFilter = new VmCommand(string.Empty, async () =>
            {
                var filters = new List<ExMapFilter>();
                var values = (EnumMeasurementType[]) Enum.GetValues(typeof(EnumMeasurementType));
                foreach (var value in values)
                {
                    filters.Add(new ExMapFilter
                                {
                                    MeasurementType = value,
                                    Shown = TypesFilter.Contains(value)
                                });
                }

                var r = await Nav.ToViewWithResult(typeof(VmMapFilter), filters).ConfigureAwait(true);
                if (r is IEnumerable<ExMapFilter> filter)
                {
                    foreach (var exMapFilter in filter)
                    {
                        await ChangeFilter(exMapFilter.Shown, exMapFilter.MeasurementType).ConfigureAwait(true);
                    }
                }
            }, glyph: Glyphs.Filter_1);

            base.InitializeCommands();
        }

        /// <summary>
        /// An/Abhaengen der events
        /// </summary>
        /// <param name="attach">Ob An oder Abhaengen</param>
        private void AttachDetachVmEvents(bool attach)
        {
            if (attach)
            {
                //einmal alle events weg -> dann wieder dranhängen -> Onloaded wird derzeit 2x aufgerufen
                //AttachDetachVmEvents(false);
                if (!_eventsattached)
                {
                    if (DeviceInfo.Plattform != EnumPlattform.Web)
                    {
                        Dc.DcExIotDevices.SelectedItemChanged += DcExIotDevices_SelectedItemChanged;
                    }

                    Map.MapItemClicked += MapOnMapItemClicked;
                    Map.CoordinatesClicked += MapOnCoordinatesClicked;
                    Map.MapItems.CollectionEvent += MapItemsOnCollectionEvent;
                    Dc.DcExMeasurementDefinition.CollectionEvent += DcExMeasurementDefinitionOnCollectionEvent;
                    _eventsattached = true;
                }
            }
            else
            {
                if (_eventsattached)
                {
                    if (DeviceInfo.Plattform != EnumPlattform.Web)
                    {
                        Dc.DcExIotDevices.SelectedItemChanged -= DcExIotDevices_SelectedItemChanged;
                    }

                    Map.MapItemClicked -= MapOnMapItemClicked;
                    Map.CoordinatesClicked -= MapOnCoordinatesClicked;
                    Map.MapItems.CollectionEvent -= MapItemsOnCollectionEvent;
                    Dc.DcExMeasurementDefinition.CollectionEvent -= DcExMeasurementDefinitionOnCollectionEvent;
                    _eventsattached = false;
                }
            }
        }

        /// <summary>
        /// Iot Geraete selektiertes item aenderung
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        private async void DcExIotDevices_SelectedItemChanged(object sender, SelectedItemEventArgs<DcListTypeIotDevice> e)
        {
            if (e.CurrentItem == null!)
            {
                return;
            }

            _ = await View.DetailView.ShowVmDetailWithResult(typeof(VmIotDeviceInfo), e.CurrentItem).ConfigureAwait(true);
        }

        /// <summary>
        /// Messwert definition collection aenderung
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        private async Task DcExMeasurementDefinitionOnCollectionEvent(object sender, CollectionEventArgs<DcListTypeMeasurementDefinition> e)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (e is null)
            {
                return;
            }

            switch (e.TypeOfEvent)
            {
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
        /// Messwertdefinition bearbeiten
        /// </summary>
        /// <param name="item">item</param>
        /// <param name="isNew">ob neu</param>
        /// <returns>task</returns>
        // ReSharper disable once UnusedParameter.Local
        private async Task EditMeasurementDefinition(DcListTypeMeasurementDefinition item, bool isNew)
        {
            AttachDetachVmEvents(false);

            _ = await Nav.ToViewWithResult(typeof(VmEditMeasurementDefinition), item).ConfigureAwait(false);

            AttachDetachVmEvents(true);

            await OnDisappearing(null!).ConfigureAwait(true); //workaround vmmain kommt 2 mal
            try
            {
                GCmdHome.Execute(null!);
                UpdateMapItems();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// Klick auf map item
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        private void MapOnMapItemClicked(object? sender, MapItemClickEventArgs e)
        {
            var device = Dc.DcExIotDevices.FirstOrDefault(x => x.Index == e.MapItem.Index);

            if (device is null)
            {
                return;
            }

            Dc.DcExIotDevices.CmdSelectItem.Execute(device);
            UpdateMapItems();
        }

        /// <summary>
        /// Klick auf koordinaten
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        private void MapOnCoordinatesClicked(object sender, CoordinatesClickEventArgs e)
        {
            //MsgBox.Show($"Coordinates Lat: {e.Position.Latitude}, Lon: {e.Position.Longitude}");
        }

        /// <summary>
        /// Karten item collection aenderung
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        private Task MapItemsOnCollectionEvent(object sender, CollectionEventArgs<IBmMapItem> e)
        {
            switch (e.TypeOfEvent)
            {
                case EnumCollectionEventType.InfoRequest:
                    break;
                case EnumCollectionEventType.AddRequest:
                case EnumCollectionEventType.DeleteRequest:
                case EnumCollectionEventType.UpdateRequest:
                case EnumCollectionEventType.EditRequest:
                case EnumCollectionEventType.MultiDeleteRequest:
                default:
                    break;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Initialisieren nach daten laden
        /// </summary>
        private void InitAfterLoadedData()
        {
        }

        #region Overrides

        /// <summary>
        ///     OnDisappearing (4) wenn View unsichtbar / beendet wird
        ///     Nur einmal
        /// </summary>
        public override Task OnDisappearing(IView view)
        {
            AttachDetachVmEvents(false);
            View.BusyClear();
            return base.OnDisappearing(view);
        }

        /// <summary>View ist komplett geladen und sichtbar</summary>
        /// <returns></returns>
        public override async Task OnLoaded()
        {
            if (!Dc.AutoConnect)
            {
                DcStartAutoConnect();
            }

            if (!ProjectDataLoadedAfterDcConnected)
            {
                ProjectDataLoaded += (sender, args) =>
                {
                    InitAfterLoadedData();
                    View.BusyClear();
                };
            }
            else
            {
                InitAfterLoadedData();
                View.BusyClear();
            }

            if (Dc.DcExUser.Data.Id != -1)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                await Dc.DcExIotDevices.WaitDataFromServerAsync().ConfigureAwait(true);

                await Dc.DcExMeasurementDefinitionAssignments.WaitDataFromServerAsync(reload: true).ConfigureAwait(true);
#pragma warning restore CS0618 // Type or member is obsolete
                var ttt = Dc.DcExMeasurementDefinitionAssignments;
                // ReSharper disable once UnusedVariable
                var ids = ttt.Select(f => f.Data.Id).ToList();

                Dc.DcExIotDevices.FilterClear();
            }

            AttachDetachVmEvents(true);

            await base.OnLoaded().ConfigureAwait(true);

            UpdateMapItems();

            BissPosition? centerPos = null;
            //if (SelectedIotDeviceId > 0)
            if (Dc.DcExIotDevices.SelectedItem != null)
            {
                var centerMapItem = Map.MapItems.FirstOrDefault(x => x.Index == Dc.DcExIotDevices.SelectedItem.Index);

                if (centerMapItem != null)
                {
                    centerPos = centerMapItem.Position;
                    centerMapItem.OnClicked();
                }
            }

            if (centerPos is null)
            {
                //Todo 1: löschen wenn...
                centerPos = Map.MapItems.LastOrDefault()?.Position;

                //Todo 2: ...das hier funktioniert
                //centerPos = await this.BcBissMap()!.GetUserLocation().ConfigureAwait(true);
            }

            if (centerPos != null)
            {
                Map.SetCenterAndZoom(centerPos, BmDistance.FromKilometers(5));
            }
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Cmd für Abonnieren
        /// </summary>
        public VmCommand CmdSubscribe { get; set; }

        /// <summary>
        ///     Filter View öffnen.
        /// </summary>
        public VmCommand CmdOpenFilter { get; set; } = null!;

        #endregion
    }
}