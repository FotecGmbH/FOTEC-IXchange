// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BaseApp.Connectivity;
using BaseApp.ViewModel.Infrastructure;
using BDA.Common.Exchange.Configs.Enums;
using BDA.Common.Exchange.Enum;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Apps.Attributes;
using Biss.Apps.Collections;
using Biss.Apps.Enum;
using Biss.Apps.Interfaces;
using Biss.Log.Producer;
using Exchange.Resources;
using Microsoft.Extensions.Logging;
using EnumVmEditResult = Exchange.Enum.EnumVmEditResult;

namespace BaseApp.ViewModel
{
    /// <summary>
    /// <para>Viewmodel für ViewIotDevicesOverview</para>
    /// Klasse VmIotDevicesOverview. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewIotDevicesOverview", true)]
    public class VmIotDevicesOverview : VmProjectBase
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmIotDevicesOverview.DesignInstance}"
        /// </summary>
        public static VmIotDevicesOverview DesignInstance = new VmIotDevicesOverview();

        /// <summary>
        /// Ob events angehaengt
        /// </summary>
        private bool _eventsattached;

        /// <summary>
        ///     VmIotDevicesOverview
        /// </summary>
        public VmIotDevicesOverview() : base(ResViewIotDevicesOverview.LblPageTitle)
        {
            SetViewProperties();
        }

        #region Properties

        /// <summary>
        ///     Eigene Devices.
        /// </summary>
        public BxObservableCollection<DcListTypeIotDevice> CollectionOwnDevices { get; } = new BxObservableCollection<DcListTypeIotDevice>();

        /// <summary>
        ///     Abonnierte Devices.
        /// </summary>
        public BxObservableCollection<DcListTypeIotDevice> CollectionSubscribedDevices { get; } = new BxObservableCollection<DcListTypeIotDevice>();

        #endregion

        #region Commands

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
        }

        #endregion

        /// <summary>
        /// An/Abhaengen der events
        /// </summary>
        /// <param name="attach">Ob An oder Abhaengen</param>
        private void AttachDetachVmEvents(bool attach)
        {
            if (attach)
            {
                if (!_eventsattached)
                {
                    Dc.DcExIotDevices.CollectionChanged += DcExIotDevices_CollectionChanged;
                    CollectionOwnDevices.CollectionEvent += IotDevices_CollectionEvent;
                    CollectionSubscribedDevices.CollectionEvent += IotDevices_CollectionEvent;
                    _eventsattached = true;
                }
            }
            else
            {
                if (_eventsattached)
                {
#pragma warning disable CA2213
                    Dc.DcExIotDevices.CollectionChanged -= DcExIotDevices_CollectionChanged;
#pragma warning restore CA2213
                    CollectionSubscribedDevices.CollectionEvent -= IotDevices_CollectionEvent;
                    CollectionOwnDevices.CollectionEvent -= IotDevices_CollectionEvent;
                    _eventsattached = false;
                }
            }
        }

        /// <summary>
        /// Iot Geraete collection aenderung
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        private void DcExIotDevices_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            FillLists();
        }

        /// <summary>
        /// Listen befuellen
        /// </summary>
        private void FillLists()
        {
            CollectionOwnDevices.Clear();
            var owndevices = Dc.DcExIotDevices.Where(x => Dc.DcExUser.Data.HasPermissionTo(x.Data)).ToList();
            foreach (var device in owndevices)
            {
                CollectionOwnDevices.Add(device);
            }

            CollectionSubscribedDevices.Clear();
            var subscribeddevices = Dc.DcExIotDevices.Where(iotDevice => !Dc.DcExUser.Data.HasPermissionTo(iotDevice.Data)
                                                                         && Dc.DcExAbos.Any(abo => iotDevice.Data.MeasurementDefinitions.Any(measurementDefinition => measurementDefinition.Id == abo.Data.MeasurementDefinitionAssignment.MeasurementDefinition.Id)
                                                                                                   && abo.Data.User.Id == Dc.DcExUser.Data.Id));
            foreach (var device in subscribeddevices)
            {
                CollectionSubscribedDevices.Add(device);
            }
        }

        /// <summary>
        /// Callback für das CollectionEvent der Dc.DcExIotDevices (Add, Edit, Delete)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task IotDevices_CollectionEvent(object sender, CollectionEventArgs<DcListTypeIotDevice> e)
        {
            switch (e.TypeOfEvent)
            {
                case EnumCollectionEventType.InfoRequest:
                {
                    // ReSharper disable once UnusedVariable
                    var result = await Nav.ToViewWithResult(typeof(VmIotDeviceDetails), e.Item).ConfigureAwait(true);
                    //await Dc.DcExIotDevices.Sync().ConfigureAwait(true);
                    break;
                }

                case EnumCollectionEventType.AddRequest:
                {
                    var newIot = new DcListTypeIotDevice(new ExIotDevice
                                                         {
                                                             Plattform = EnumIotDevicePlattforms.RaspberryPi,
                                                             TransmissionType = EnumTransmission.Elapsedtime,
                                                             Upstream = EnumIotDeviceUpstreamTypes.Ttn,
                                                             //Id = Dc.DcExGateways.SelectedItem!.Index,
                                                             CompanyId = Dc.DcExUser.Data.Premissions.FirstOrDefault()?.CompanyId,
                                                             Information =
                                                             {
                                                                 CreatedDate = DateTime.UtcNow,
                                                             }
                                                         });

                    try
                    {
                        newIot.Data.GatewayId = Dc.DcExGateways.FirstOrDefault(g => g.Data.CompanyId == Dc.DcExUser.Data.Premissions.FirstOrDefault()?.CompanyId).Id;
                    }
                    catch (Exception exception)
                    {
                        Logging.Log.LogError($"{exception}");
                    }

                    Dc.DcExIotDevices.Add(newIot);

                    var r = await Nav.ToViewWithResult(typeof(VmEditIotDevice), newIot).ConfigureAwait(true);
                    await View.RefreshAsync().ConfigureAwait(true);
                    if (r is EnumVmEditResult result)
                    {
                        if (result != EnumVmEditResult.ModifiedAndStored)
                        {
                            // Workaround bis Fix in der Liste 
                            try
                            {
                                Dc.DcExIotDevices.Remove(newIot);
                            }
                            catch (Exception ex)
                            {
                                Logging.Log.LogError($"[{nameof(VmInfrastructure)}]({nameof(IotDevices_CollectionEvent)}): Workaround - {ex}");
                            }
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Wrong result!");
                    }

                    break;
                }
                case EnumCollectionEventType.DeleteRequest:
                {
                    if (e.Item != null!)
                    {
                        var msg = await MsgBox.Show(ResViewIotDevicesOverview.MsgBoxDeleteIotDevice, icon: VmMessageBoxImage.Warning, button: VmMessageBoxButton.YesNo).ConfigureAwait(true);
                        if (msg == VmMessageBoxResult.No)
                        {
                            return;
                        }

                        // Workaround bis Fix in der Liste 
                        try
                        {
                            Dc.DcExIotDevices.Remove(e.Item);
                        }
                        catch (Exception ex)
                        {
                            Logging.Log.LogError($"{ex}");
                        }

                        var r = await Dc.DcExIotDevices.StoreAll().ConfigureAwait(true);
                        if (!r.DataOk)
                        {
                            await MsgBox.Show(ResViewIotDevicesOverview.MsgBoxDeleteFailed, ResCommon.MsgTitleNotSaved, icon: VmMessageBoxImage.Error).ConfigureAwait(true);

                            Dc.DcExIotDevices.Add(e.Item);
                        }
                    }

                    break;
                }
            }
        }

        #region Overrides

        /// <summary>
        ///     OnAppearing (1) für View geladen noch nicht sichtbar
        ///     Wird Mal wenn View wieder sichtbar ausgeführt
        ///     Unbedingt beim überschreiben auch base. aufrufen!
        /// </summary>
        public override async Task OnAppearing(IView view)
        {
            await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
            //await Dc.DcExIotDevices.Sync().ConfigureAwait(true);
            await Dc.DcExAbos.Sync().ConfigureAwait(true);
            _semaphoreSlim.Release();
            await base.OnAppearing(view).ConfigureAwait(true);
        }

        /// <summary>
        /// Semaphore fuer threadsafe
        /// </summary>
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        /// <summary>
        ///     OnLoaded (3) für View geladen
        ///     Jedes Mal wenn View wieder sichtbar
        /// </summary>
        public override async Task OnLoaded()
        {
            await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
            
            FillLists();
            AttachDetachVmEvents(true);
            _semaphoreSlim.Release();
            await base.OnLoaded().ConfigureAwait(true);
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
    }
}