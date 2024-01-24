// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using BaseApp.Helper;
using BDA.Common.Exchange.Configs.Enums;
using BDA.Common.Exchange.Configs.GlobalConfigs;
using BDA.Common.Exchange.Configs.Upstream.Ttn;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Apps.Attributes;
using Biss.Apps.Base;
using Biss.Apps.Collections;
using Biss.Apps.Enum;
using Biss.Apps.Interfaces;
using Biss.Apps.ViewModel;
using Biss.Common;
using Biss.Dc.Client;
using Biss.Log.Producer;
using Biss.ObjectEx;
using Biss.Serialize;
using Exchange.Enum;
using Exchange.Resources;
using Microsoft.Extensions.Logging;

namespace BaseApp.ViewModel.Infrastructure
{
    /// <summary>
    ///     <para>
    ///         View für dynamische IXchange Konfigurationen
    ///         ToDo Mko - dynamisch machen
    ///         Status für erste Release - TTN Konfiguration
    ///     </para>
    /// Klasse VmConfigs. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewConfigs", true)]
    public class VmConfigs : VmProjectBase
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmConfigs.DesignInstance}"
        /// </summary>
        public static VmConfigs DesignInstance = new VmConfigs();

        /// <summary>
        /// IoT-Geraet
        /// </summary>
        private ExIotDevice _iotDevice = null!;

        /// <summary>
        ///     VmConfigs
        /// </summary>
        public VmConfigs() : base(ResViewConfigs.PageTitle)
        {
            SetViewProperties(true);

            Loaded += (sender, args) => TtnOnPropertyChanged(this, null!);
        }

        #region Properties

        /// <summary>
        ///     Ttn
        /// </summary>
        public GcTtnIotDevice Ttn { get; private set; } = new GcTtnIotDevice();

        /// <summary>
        ///     EntryTtnAppKey
        /// </summary>
        public VmEntry EntryTtnAppKey { get; set; } = null!;

        /// <summary>
        ///     EntryTtnDescription
        /// </summary>
        public VmEntry EntryTtnDescription { get; private set; } = null!;

        /// <summary>
        ///     EntryTtnAppEui
        /// </summary>
        public VmEntry EntryTtnAppEui { get; set; } = null!;

        /// <summary>
        ///     EntryTtnDeviceId
        /// </summary>
        public VmEntry EntryTtnDeviceId { get; set; } = null!;

        /// <summary>
        ///     EntryTtnDevEui
        /// </summary>
        public VmEntry EntryTtnDevEui { get; private set; } = null!;


        /// <summary>
        ///     PickerCopanyTtnConfigs
        /// </summary>
        public VmPicker<ExGlobalConfig> PickerCopanyTtnConfigs { get; private set; } = new VmPicker<ExGlobalConfig>(nameof(PickerCopanyTtnConfigs));


        /// <summary>
        ///     PickerCopanyEnumLorawanVersion
        /// </summary>
        public VmPicker<EnumLorawanVersion> PickerEnumLorawanVersion { get; private set; } = new VmPicker<EnumLorawanVersion>(nameof(PickerEnumLorawanVersion));

        /// <summary>
        ///     PickerEnumLorawanPhysicalVersion
        /// </summary>
        public VmPicker<EnumLorawanPhysicalVersion> PickerEnumLorawanPhysicalVersion { get; private set; } = new VmPicker<EnumLorawanPhysicalVersion>(nameof(PickerEnumLorawanPhysicalVersion));

        /// <summary>
        ///     PickerEnumLorawanFrequencyPlanId
        /// </summary>
        public VmPicker<EnumLorawanFrequencyPlanId> PickerEnumLorawanFrequencyPlanId { get; private set; } = new VmPicker<EnumLorawanFrequencyPlanId>(nameof(PickerEnumLorawanFrequencyPlanId));


        /// <summary>
        ///     Serialisierte Daten
        /// </summary>
        public string AdditionalConfiguration { get; set; } = string.Empty;


        /// <summary>
        ///     If TTN-DEVEUI should be generated or not
        /// </summary>
        public bool GenerateDevEui { get; set; }

        /// <summary>
        ///     If TTN-APPEUI should be generated or not
        /// </summary>
        public bool GenerateAppEui { get; set; }

        /// <summary>
        ///     If TTN-APPKEY should be generated or not
        /// </summary>
        public bool GenerateAppKey { get; set; }

        #endregion

        /// <summary>
        ///     OnDisappearing (4) wenn View unsichtbar / beendet wird
        ///     Nur einmal
        /// </summary>
        public override Task OnDisappearing(IView view)
        {
            Ttn.PropertyChanged -= TtnOnPropertyChanged;
            CheckSaveBehavior = null!;
            return base.OnDisappearing(view);
        }

        /// <summary>
        ///     OnActivated (2) für View geladen noch nicht sichtbar
        ///     Nur einmal
        /// </summary>
        public override async Task OnActivated(object? args = null)
        {
            await base.OnActivated(args).ConfigureAwait(true);
            if (args is ExIotDevice a)
            {
                _iotDevice = a;
            }
            else
            {
                throw new ArgumentNullException($"[{nameof(VmConfigs)}]({nameof(OnActivated)}): {nameof(args)}");
            }

            if (_iotDevice.Upstream != EnumIotDeviceUpstreamTypes.Ttn)
            {
                throw new NotImplementedException();
            }

            foreach (var gc in Dc.DcExGlobalConfig.Where(g => g.Data.CompanyId == _iotDevice.CompanyId && g.Data.ConfigType == EnumGlobalConfigTypes.Ttn).Select(s => s.Data))
            {
                PickerCopanyTtnConfigs.AddKey(gc, gc.Information.Name);
            }

            if (!PickerCopanyTtnConfigs.Any())
            {
                ViewResult = null;
                await MsgBox.Show("Es muss zuerst eine TTN Konfiguration angelegt werden.").ConfigureAwait(true);

                await AddGlobalConfig().ConfigureAwait(true);

#pragma warning disable CS0618 // Type or member is obsolete
                Dc.DcExGlobalConfig.ReadListData();
#pragma warning restore CS0618 // Type or member is obsolete

                foreach (var gc in Dc.DcExGlobalConfig.Where(g => g.Data.CompanyId == _iotDevice.CompanyId && g.Data.ConfigType == EnumGlobalConfigTypes.Ttn).Select(s => s.Data))
                {
                    PickerCopanyTtnConfigs.AddKey(gc, gc.Information.Name);
                }

                if (!PickerCopanyTtnConfigs.Any())
                {
                    //await MsgBox.Show("Es exisitiert noch keine Ttn Konfiguration bei dieser Firma.").ConfigureAwait(true);
                    await Nav.Back().ConfigureAwait(true);
                    return;
                }
            }

            foreach (var value in EnumUtil.GetValues<EnumLorawanVersion>())
            {
                PickerEnumLorawanVersion.AddKey(value, value.ToString());
            }

            foreach (var value in EnumUtil.GetValues<EnumLorawanPhysicalVersion>())
            {
                PickerEnumLorawanPhysicalVersion.AddKey(value, value.ToString());
            }

            foreach (var value in EnumUtil.GetValues<EnumLorawanFrequencyPlanId>())
            {
                PickerEnumLorawanFrequencyPlanId.AddKey(value, value.ToString());
            }

            var ttnConfig = BissDeserialize.FromJson<GcTtnIotDevice?>(_iotDevice.AdditionalConfiguration);
            ttnConfig ??= GcTtnIotDevice.FromCompatibilityModel(BissDeserialize.FromJson<GcTtnIotDeviceCompat?>(_iotDevice.AdditionalConfiguration));

            if (ttnConfig is null || string.IsNullOrEmpty(_iotDevice.AdditionalConfiguration) || string.IsNullOrEmpty(ttnConfig.DeviceId))
            {
                var gc = PickerCopanyTtnConfigs.First().Key;
                PickerCopanyTtnConfigs.SelectKey(gc);
                Ttn = new GcTtnIotDevice
                      {
                          GcTtnCompany = BissDeserialize.FromJson<GcTtn>(gc.AdditionalConfiguration)
                      };
            }
            else
            {
                Ttn = BissDeserialize.FromJson<GcTtnIotDevice>(_iotDevice.AdditionalConfiguration);
                PickerCopanyTtnConfigs.SelectKey(PickerCopanyTtnConfigs.FirstOrDefault().Key);
                var gc = PickerCopanyTtnConfigs.FirstOrDefault(w => w.Key.Id == _iotDevice.GlobalConfigId).Key;
                PickerCopanyTtnConfigs.SelectKey(gc);
            }

            // ReSharper disable once VariableHidesOuterVariable
            PickerCopanyTtnConfigs.SelectedItemChanged += (sender, args) =>
            {
                var gc = args.CurrentItem.Key;
                Ttn.GcTtnCompany = BissDeserialize.FromJson<GcTtn>(gc.AdditionalConfiguration);
            };

            PickerCopanyTtnConfigs.CollectionEvent += PickerCopanyTtnConfigsOnCollectionEvent;

            PickerEnumLorawanVersion.SelectedItem = PickerEnumLorawanVersion.First(f => f.Key == Ttn.LorawanVersion);
            PickerEnumLorawanPhysicalVersion.SelectedItem = PickerEnumLorawanPhysicalVersion.First(f => f.Key == Ttn.LoraPhysicalVersion);
            PickerEnumLorawanFrequencyPlanId.SelectedItem = PickerEnumLorawanFrequencyPlanId.First(f => f.Key == Ttn.LoraFrequency);


            EntryTtnDeviceId = new VmEntry(EnumVmEntryBehavior.StopTyping,
                $"{ResViewConfigs.LblIdOfIot}:",
                "Nur Kleinbuchstaben, Zahlen, und Bindestrich erlaubt",
                Ttn,
                nameof(GcTtnIotDevice.DeviceId),
                ValidateFuncTtnDeviceId,
                showTitle: false
            );

            EntryTtnDevEui = new VmEntry(EnumVmEntryBehavior.StopTyping,
                ResViewConfigs.LblTtnDevEui,
                ResViewConfigs.PlaceholderTtnDevUI,
                Ttn,
                nameof(GcTtnIotDevice.DevEui),
                showTitle: false
            );

            EntryTtnAppEui = new VmEntry(EnumVmEntryBehavior.StopTyping,
                $"{ResViewConfigs.LblTtnAppUI}:",
                ResViewConfigs.PlaceholderTtnAppUI,
                Ttn,
                nameof(GcTtnIotDevice.AppEui),
                showTitle: false
            );

            EntryTtnAppKey = new VmEntry(EnumVmEntryBehavior.StopTyping,
                $"{ResViewConfigs.LblTtnAppKey}:",
                ResViewConfigs.PlaceholderTtnAppKey,
                Ttn,
                nameof(GcTtnIotDevice.AppKey),
                showTitle: false
            );

            EntryTtnDescription = new VmEntry(EnumVmEntryBehavior.StopTyping,
                $"{ResViewConfigs.LblTtnDescription}:",
                ResViewConfigs.PlaceholderTtnDescription,
                Ttn,
                nameof(GcTtnIotDevice.Description),
                showTitle: false
            );

            if (string.IsNullOrEmpty(Ttn.DevEui))
            {
                GenerateDevEui = true;
            }

            if (string.IsNullOrEmpty(Ttn.AppEui))
            {
                GenerateAppEui = true;
            }

            if (string.IsNullOrEmpty(Ttn.AppKey))
            {
                GenerateAppKey = true;
            }
        }

        /// <summary>
        ///     OnLoaded (3) für View geladen
        ///     Jedes Mal wenn View wieder sichtbar
        /// </summary>
        public override Task OnLoaded()
        {
            Ttn.PropertyChanged += TtnOnPropertyChanged;

            CheckSaveBehavior = new CheckSaveBissSerializeBehavior<GcTtnIotDevice>();
            CheckSaveBehavior.SetCompareData(Ttn);

            return base.OnLoaded();
        }

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            View.CmdSaveHeader = new VmCommand("Ok", async () =>
            {
                if (this.GetAllInstancesWithType<VmEntry>().Any(p => !p.DataValid))
                {
                    await MsgBox.Show("Speichern leider nicht möglich!", "Eingabefehler").ConfigureAwait(false);
                    return;
                }

                _iotDevice.AdditionalConfiguration = AdditionalConfiguration;
                _iotDevice.GlobalConfigId = PickerCopanyTtnConfigs.SelectedItem!.Key.Id;
                CheckSaveBehavior = null!;
                ViewResult = true;

                await Nav.Back().ConfigureAwait(true);
            }, CanExecuteSave, glyph: Glyphs.Check);
        }

        /// <summary>
        /// Globale Konfiguration hinzufuegen
        /// </summary>
        /// <returns>Task</returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task AddGlobalConfig()
        {
            var newGc = new ExGlobalConfig
                        {
                            CompanyId = Dc.DcExUser.Data.Premissions.FirstOrDefault()?.CompanyId ?? -1,
                            ConfigType = EnumGlobalConfigTypes.Ttn,
                            ConfigVersion = 1,
                            Information =
                            {
                                CreatedDate = DateTime.UtcNow,
                            }
                        };
            var item = new DcListDataPoint<ExGlobalConfig>(newGc);
            Dc.DcExGlobalConfig.Add(item);
            newGc.Id = item.Index;

            var r = await Nav.ToViewWithResult(typeof(VmEditGlobalConfig), item).ConfigureAwait(true);

            if (r is EnumVmEditResult result)
            {
                if (result != EnumVmEditResult.ModifiedAndStored)
                {
                    // Workaround bis fix in Biss.Collections
                    try
                    {
                        Dc.DcExGlobalConfig.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        Logging.Log.LogError($"{ex}");
                    }
                }
                else
                {
                    PickerCopanyTtnConfigs.AddKey(newGc, newGc.Information.Name);
                }
            }
            else
            {
                throw new ArgumentException("Wrong result!");
            }
        }

        /// <summary>
        /// PickerCopanyTtnConfigsOnCollectionEvent
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        /// <returns>Task</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private async Task PickerCopanyTtnConfigsOnCollectionEvent(object sender, CollectionEventArgs<VmPickerElement<ExGlobalConfig>> e)
        {
            switch (e.TypeOfEvent)
            {
                case EnumCollectionEventType.AddRequest:
                {
                    await AddGlobalConfig().ConfigureAwait(true);
                    break;
                }

                case EnumCollectionEventType.DeleteRequest:
                {
                    if (e.Item == null!)
                    {
                        throw new ArgumentNullException($"[{nameof(VmConfigs)}]({nameof(PickerCopanyTtnConfigsOnCollectionEvent)}): {nameof(e.Item)}");
                    }

                    if (e.Item.Key.IsUsedInIotDevice)
                    {
                        await MsgBox.Show("Diese Konfiguration kann nicht gelöscht werden da diese in Verwendung ist.").ConfigureAwait(true);
                        return;
                    }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    DcListDataPoint<ExGlobalConfig> dp = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    var removed = false;
                    // Workaround bis fix in Biss.Collections
                    try
                    {
                        dp = Dc.DcExGlobalConfig.FirstOrDefault(x => x.Data.Id == e.Item.Key.Id);
                        if (dp is null)
                        {
                            PickerCopanyTtnConfigs.Remove(e.Item);
                            return;
                        }

                        removed = Dc.DcExGlobalConfig.Remove(dp);
                    }
                    catch (Exception ex)
                    {
                        Logging.Log.LogError($"{ex}");
                    }

                    var r = await Dc.DcExGlobalConfig.StoreAll().ConfigureAwait(true);
                    if (!r.DataOk)
                    {
                        await MsgBox.Show("Löchen leider nicht möglich!").ConfigureAwait(true);

                        if (removed)
                        {
                            Dc.DcExGlobalConfig.Add(dp!);
                        }
                    }
                    else
                    {
                        // Workaround bis fix in Biss.Collections
                        try
                        {
                            PickerCopanyTtnConfigs.Remove(e.Item);
                        }
                        catch (Exception ex)
                        {
                            Logging.Log.LogError($"{ex}");
                        }
                    }

                    break;
                }

                case EnumCollectionEventType.EditRequest:
                {
                    if (e.Item == null!)
                    {
                        throw new ArgumentNullException($"[{nameof(VmConfigs)}]({nameof(PickerCopanyTtnConfigsOnCollectionEvent)}): {nameof(e.Item)}");
                    }

                    var dp = Dc.DcExGlobalConfig.FirstOrDefault(x => x.Data.Id == e.Item.Key.Id);
                    if (dp is null)
                    {
                        await MsgBox.Show("Konfiguration nicht mehr vorhanden!").ConfigureAwait(true);
                        return;
                    }

                    var r = await Nav.ToViewWithResult(typeof(VmEditGlobalConfig), dp).ConfigureAwait(true);
                    await View.RefreshAsync().ConfigureAwait(true);
                    if (r is EnumVmEditResult result)
                    {
                        if (result != EnumVmEditResult.ModifiedAndStored)
                        {
                            if (dp.PossibleNewDataOnServer)
                            {
                                dp.Update();
                            }
                        }

                        e.Item.Description = dp.Data.Information.Name;
                    }
                    else
                    {
                        throw new ArgumentException("Wrong result!");
                    }

                    break;
                }

                case EnumCollectionEventType.UpdateRequest:
                case EnumCollectionEventType.InfoRequest:
                case EnumCollectionEventType.MultiDeleteRequest:
                default:
                    break;
            }
        }

        /// <summary>
        /// Ob gespeichert koennen werden soll
        /// </summary>
        /// <returns>Ob gespeichert koennen werden soll</returns>
        private bool CanExecuteSave()
        {
            var r = false;
            if (!IsLoaded)
            {
                if (View.CmdSaveHeader != null)
                {
                    View.CmdSaveHeader.IsVisible = r;
                }

                return r;
            }

            if (string.IsNullOrEmpty(_iotDevice.AdditionalConfiguration))
            {
                r = true;
            }
            else
            {
                if (CheckSaveBehavior != null)
                {
                    r = CheckSaveBehavior.Check();
                }
            }

            if (View.CmdSaveHeader != null)
            {
                View.CmdSaveHeader.IsVisible = r;
            }

            return r;
        }

        /// <summary>
        /// TTn hat sich geandert
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        private void TtnOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AdditionalConfiguration = Ttn.ToJson();
            View.CmdSaveHeader?.CanExecute();
        }

        /// <summary>
        /// Validiere TTN-Device-Id
        /// </summary>
        /// <param name="arg">TTN-DeviceId</param>
        /// <returns>Ob Valide inklusive hint falls nicht valide</returns>
        private (string hint, bool valid) ValidateFuncTtnDeviceId(string arg)
        {
            var r = VmEntryValidators.ValidateFuncStringEmpty(arg);
            if (!r.valid)
            {
                return r;
            }

            if (arg.Any(char.IsUpper))
            {
                return ("Großbuchstaben nicht erlaubt.", false);
            }

            if (arg.Any(char.IsWhiteSpace))
            {
                return ("Leerzeichen nicht erlaubt.", false);
            }

            return ("", true);
        }
    }
}