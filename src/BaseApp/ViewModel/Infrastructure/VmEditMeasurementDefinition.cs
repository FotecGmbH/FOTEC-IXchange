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
using BaseApp.Connectivity;
using BaseApp.Helper;
using BDA.Common.Exchange.Configs.Attributes.Value;
using BDA.Common.Exchange.Configs.Downstreams;
using BDA.Common.Exchange.Configs.Downstreams.Custom;
using BDA.Common.Exchange.Configs.Downstreams.DotNet;
using BDA.Common.Exchange.Configs.Downstreams.Esp32;
using BDA.Common.Exchange.Configs.Downstreams.OpenSense;
using BDA.Common.Exchange.Configs.Downstreams.Prebuilt;
using BDA.Common.Exchange.Configs.Downstreams.Virtual;
using BDA.Common.Exchange.Configs.Enums;
using BDA.Common.Exchange.Configs.Helper;
using BDA.Common.Exchange.Configs.Plattform;
using BDA.Common.Exchange.Enum;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.AppConfiguration;
using Biss.Apps.Attributes;
using Biss.Apps.Enum;
using Biss.Apps.ViewModel;
using Biss.Collections;
using Biss.Common;
using Biss.Dc.Client;
using Biss.Dc.Core;
using Biss.Log.Producer;
using Biss.ObjectEx;
using Exchange;
using Exchange.Enum;
using Exchange.Extensions;
using Exchange.Model.ConfigApp;
using Exchange.Resources;
using Microsoft.Extensions.Logging;
using EnumVmEditResult = Exchange.Enum.EnumVmEditResult;


namespace BaseApp.ViewModel.Infrastructure
{
    /// <summary>
    ///     <para>Messwertdefinition bearbeiten</para>
    /// Klasse VmEditMeasurementDefinition. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewEditMeasurementDefinition", true)]
    public class VmEditMeasurementDefinition : VmEditDcListPoint<ExMeasurementDefinition>
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmEditMeasurementDefinition.DesignInstance}"
        /// </summary>
        public static VmEditMeasurementDefinition DesignInstance = new VmEditMeasurementDefinition();

        /// <summary>
        /// Gc Downstream Base
        /// </summary>
        private GcBaseConverter<GcDownstreamBase> _configBaseOriginal = null!;

        /// <summary>
        /// Config Base
        /// </summary>
        private GcDownstreamCustom? _customConfigBase;

        /// <summary>
        /// MeasurementInterval
        /// </summary>
        private bool _iotMeasurementInterval = true;

        /// <summary>
        /// Platform
        /// </summary>
        private ConfigPlattformBase _plattform = null!;

        /// <summary>
        /// Downstream Prebuilt
        /// </summary>
        private GcDownstreamPrebuilt? _prebuiltConfigBase;

        /// <summary>
        /// Messwertdefinition
        /// </summary>
        private ExMeasurementDefinition? _startSelectedExMeasurementDefinition;

        /// <summary>
        ///     VmEditMeasurementDefinition
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public VmEditMeasurementDefinition() : base("Messwertdefinition", subTitle: "Messwertdefinition erstellen oder bearbeiten")
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            foreach (var item in EnumUtil.GetValues<EnumMeasurementType>())
            {
                PickerMeasurementType.AddKey(item, item.GetDisplayName());
            }

            foreach (var item in EnumUtil.GetValues<EnumValueTypes>())
            {
                PickerValueType.AddKey(item, item.GetDisplayName());
            }

            foreach (var t in EnumUtil.GetValues<EnumRawValueTypes>())
            {
                PickerRawValueTypes.AddKey(t, $"{t}");
            }

            if (DeviceInfo.Plattform != EnumPlattform.Web)
            {
                View.ShowSubTitle = false;
            }
        }

        #region Properties

        /// <summary>
        /// Zuweisung zur Messwertdefinition
        /// </summary>
        public ExMeasurementDefinitionAssignment MeasurementDefinitionAssignment { get; set; } = new ExMeasurementDefinitionAssignment();

        /// <summary>
        ///     Advanced Mode ein/aus
        /// </summary>
        public bool AdvancedMode { get; set; }

        /// <summary>
        ///     Abo auf eigenen Messwert
        /// </summary>
        public DcListTypeAbo DcListTypeOwnAbo { get; set; }

        /// <summary>
        ///     Ob neues Abo
        /// </summary>
        public bool NeedsNewAbo { get; set; }

        /// <summary>
        ///     Messintervall des Iot Geräts verwenden
        /// </summary>
        public bool IotMeasurementInterval
        {
            get => _iotMeasurementInterval;
            set
            {
                _iotMeasurementInterval = value;
                if (_iotMeasurementInterval)
                {
                    if (EntryMeasurmentInterval != null!)
                    {
                        EntryMeasurmentInterval.Value = "-1";
                    }
                }
                else
                {
                    if (EntryMeasurmentInterval != null!)
                    {
                        EntryMeasurmentInterval.Value = "10";
                    }
                }
            }
        }

        /// <summary>
        ///     Iot Gerät des Messwerts
        /// </summary>
        public DcListDataPoint<ExIotDevice> IotDevice { get; set; } = null!;

        /// <summary>
        ///     Dev Infos
        /// </summary>
        public string StateMachineBytes { get; set; } = string.Empty;

        /// <summary>
        ///     EntryAdditionalProperties
        /// </summary>
        public VmEntry EntryAdditionalProperties { get; set; } = null!;

        /// <summary>
        ///     EntryMeasurmentInterval
        /// </summary>
        public VmEntry EntryMeasurmentInterval { get; set; } = null!;

        /// <summary>
        ///     EntryDescription
        /// </summary>
        public VmEntry EntryDescription { get; set; } = null!;

        /// <summary>
        ///     ConfigBase
        /// </summary>
        public GcBaseConverter<GcDownstreamBase> ConfigBase { get; set; } = null!;

        /// <summary>
        ///     EntryName
        /// </summary>
        public VmEntry EntryName { get; set; } = null!;

        /// <summary>
        ///     CanEditPickerRawValueTypes
        /// </summary>
        public bool CanEditPickerRawValueTypes { get; set; }

        /// <summary>
        ///     CanEditRawValueByteCount
        /// </summary>
        public bool CanEditRawValueByteCount { get; set; }

        /// <summary>
        ///     ShowEditPickerRawValueTypes
        /// </summary>
        public bool ShowEditPickerRawValueTypes { get; set; } = true;

        /// <summary>
        ///     ShowCanEditRawValueByteCount
        /// </summary>
        public bool ShowCanEditRawValueByteCount { get; set; } = true;

        /// <summary>
        ///     CanValuePickerChanged
        /// </summary>
        public bool CanValuePickerChanged { get; set; }

        /// <summary>
        ///     OpCode Eingabe
        /// </summary>
        public bool ShowCustomOpCode { get; set; }

        /// <summary>
        ///     Op-Code für "eigene Funktion"
        /// </summary>
        public VmEntry EntryCustomOpCode { get; private set; } = null!;

        /// <summary>
        ///     RawValueByteCount
        /// </summary>
        public VmEntry EntryRawValueByteCount { get; private set; } = null!;

        /// <summary>
        ///     PickerRawValueTypes
        /// </summary>
        public VmPicker<EnumRawValueTypes> PickerRawValueTypes { get; private set; } = new VmPicker<EnumRawValueTypes>(nameof(PickerRawValueTypes));

        /// <summary>
        ///     PickerDownstreamType
        /// </summary>
        public VmPicker<EnumIotDeviceDownstreamTypes> PickerDownstreamType { get; private set; } = new VmPicker<EnumIotDeviceDownstreamTypes>(nameof(PickerDownstreamType));

        /// <summary>
        ///     PickerValueType
        /// </summary>
        public VmPicker<EnumValueTypes> PickerValueType { get; private set; } = new VmPicker<EnumValueTypes>(nameof(PickerValueType));

        /// <summary>
        ///     PickerMeasurementType
        /// </summary>
        public VmPicker<EnumMeasurementType> PickerMeasurementType { get; private set; } = new VmPicker<EnumMeasurementType>(nameof(PickerMeasurementType));

        /// <summary>
        ///     PickerPredefinedMeasurements
        /// </summary>
        public VmPicker<ExMeasurementDefinition> PickerPredefinedMeasurements { get; private set; } = new VmPicker<ExMeasurementDefinition>(nameof(PickerPredefinedMeasurements));


        /// <summary>
        ///     Einstellungen für virtuellen Float anzeigen
        /// </summary>
        public bool ShowVirtualFloat { get; set; }

        #endregion

        /// <summary>
        ///     OnActivated (2) für View geladen noch nicht sichtbar
        ///     Nur einmal
        /// </summary>
        public override async Task OnActivated(object? args = null)
        {
            await base.OnActivated(args).ConfigureAwait(true);

            var iot = Dc.DcExIotDevices.FirstOrDefault(f => f.Index == Data.IotDeviceId);
            if (iot == null || Data.IotDeviceId <= 0)
            {
                await MsgBox.Show("Das Iot Gerät des Messwerts kann zurzeit nicht bearbeitet werden.").ConfigureAwait(true);
                await Nav.Back().ConfigureAwait(true);
            }

            IotDevice = iot!;

            if (!IotDevice.Data.IsIotDotnetSensor)
            {
                Data.MeasurementInterval = -1;
            }

            InitConfigBase(Data.AdditionalConfiguration);

            InitPlattform();

            var currAssignment = ExMesDefAssignHelper.GetAssignment(Data, Dc);

            PickerMeasurementType.SelectKey(currAssignment.Data.Type);
            PickerMeasurementType.SelectedItemChanged += PickerMeasurementTypeOnSelectedItemChanged;

            if (PickerDownstreamType.Any(f => f.Key == EnumIotDeviceDownstreamTypes.Prebuilt))
            {
                PickerDownstreamType.SelectKey(EnumIotDeviceDownstreamTypes.Prebuilt);
            }
            else
            {
                PickerDownstreamType.SelectedItem = PickerDownstreamType.First(f => f.Key == Data.DownstreamType);
            }

            PickerDownstreamType.SelectedItemChanged += PickerDownstreamTypeOnSelectedItemChanged;

            PickerDownstreamTypeOnSelectedItemChanged(this, null!);
            PickerPredefinedMeasurements.SelectedItemChanged += PickerPredefinedMeasurementsOnSelectedItemChanged;

            PickerRawValueTypes.SelectedItemChanged += PickerRawValueTypesOnSelectedItemChanged;
            PickerValueType.SelectedItemChanged += PickerValueTypeOnSelectedItemChanged;

            InitViewElements();

            UpdateStateMachineBytes();

            await Dc.DcExAbos.Sync().ConfigureAwait(true);
            DcListTypeOwnAbo = Dc.DcExAbos.FirstOrDefault(x => x.Data.MeasurementDefinitionAssignment.Id == currAssignment.Id && x.Data.User.Id == Dc.DcExUser.Data.Id);

            if (DcListTypeOwnAbo is null)
            {
                NeedsNewAbo = true;
                DcListTypeOwnAbo = new DcListTypeAbo(new ExAbo
                                                     {
                                                         MeasurementDefinitionAssignment = currAssignment.Data,
                                                         User = Dc.DcExUser.Data,
                                                     });
            }


            InitEntries();

            Data.PropertyChanged += DataOnPropertyChanged;
        }

        #region Commands

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            base.InitializeCommands();
            View.CmdSaveHeader = new VmCommand(ResCommon.CmdSave, async () =>
            {
                var pageVmEntries = this.GetAllInstancesWithType<VmEntry>();
                if (pageVmEntries.Any(p => !p.DataValid))
                {
                    await MsgBox.Show("Speichern leider nicht möglich!", "Eingabefehler").ConfigureAwait(false);
                    return;
                }

                if (DcListDataPoint.State != EnumDcListElementState.New)
                {
                    DcListDataPoint.State = EnumDcListElementState.Modified;
                }

                if (CheckBeforeSaving != null)
                {
                    var check = await CheckBeforeSaving().ConfigureAwait(true);
                    if (!check)
                    {
                        Logging.Log.LogInfo($"[VmEditMeasurementDefinition]({nameof(InitializeCommands)}): StoreData canceled by BeforeSave!");
                        return;
                    }
                }

                var r = await DcListDataPoint.StoreData(true).ConfigureAwait(true);
                if (!r.DataOk)
                {
                    Logging.Log.LogWarning($"[VmEditMeasurementDefinition]({nameof(InitializeCommands)}): {r.ErrorType}-{r.ServerExceptionText}");

                    var msg = "Speichern leider nicht möglich!";
                    if (Constants.AppConfiguration.CurrentBuildType == EnumCurrentBuildType.Developer)
                    {
                        msg += $"\r\n{r.ErrorType}\r\n{r.ServerExceptionText}";
                    }

                    await MsgBox.Show(msg, "Serverfehler").ConfigureAwait(false);
                    return;
                }


                var x = new DcListTypeMeasurementDefinitionAssignment(MeasurementDefinitionAssignment);
                x.Data.MeasurementDefinition = DcListDataPoint.Data;
                x.Data.MeasurementDefinition.Id = DcListDataPoint.Index;
                Dc.DcExMeasurementDefinitionAssignments.Add(x);
                await Dc.DcExMeasurementDefinitionAssignments.StoreAll().ConfigureAwait(true);
#pragma warning disable CS0618 // Type or member is obsolete
                await Dc.DcExMeasurementDefinitionAssignments.WaitDataFromServerAsync(reload: true).ConfigureAwait(true);
#pragma warning restore CS0618 // Type or member is obsolete

                var mA = Dc.DcExMeasurementDefinitionAssignments.FirstOrDefault(mA => mA.Data.MeasurementDefinition.Id == DcListDataPoint.Index);
                //await x.StoreData(true).ConfigureAwait(true);


                //eigenes Abo speichern
                if (NeedsNewAbo)
                {
                    //neues Abo
                    DcListTypeOwnAbo.Data.MeasurementDefinitionAssignment.Id = /*DcListDataPoint.Index*/mA.Index;
                    Dc.DcExAbos.Add(DcListTypeOwnAbo);
                    NeedsNewAbo = false;
                }

                var aboRes = await DcListTypeOwnAbo.StoreData().ConfigureAwait(true);

                if (!aboRes.DataOk)
                {
                    Logging.Log.LogWarning($"[VmEditMeasurementDefinition]({nameof(InitializeCommands)}): {aboRes.ErrorType}-{aboRes.ServerExceptionText}");

                    var msg = "\"Eigenes Abo\" konnte nicht gespeichert werden!";
                    if (Constants.AppConfiguration.CurrentBuildType == EnumCurrentBuildType.Developer)
                    {
                        msg += $"\r\n{aboRes.ErrorType}\r\n{aboRes.ServerExceptionText}";
                    }

                    await MsgBox.Show(msg, "Serverfehler").ConfigureAwait(false);
                }

                CheckSaveBehavior = null;
                ViewResult = EnumVmEditResult.ModifiedAndStored;
                await Nav.Back().ConfigureAwait(true);
            }, (() => true) /*CanExecuteSaveCommand*/, glyph: Glyphs.Floppy_disk);
        }

        #endregion

        /// <summary>
        /// Entries initialisieren
        /// </summary>
        private void InitEntries()
        {
            EntryExceedNotifyValue = new VmEntry(rootObject: DcListTypeOwnAbo.Data,
                rootObjectPropertyName: nameof(DcListTypeOwnAbo.Data.ExceedNotifyValue),
                showMaxChar: false,
                showTitle: false,
                validateFunc: VmEntryValidators.ValidateFuncDouble);

            EntryUndercutNotifyValue = new VmEntry(rootObject: DcListTypeOwnAbo.Data,
                rootObjectPropertyName: nameof(DcListTypeOwnAbo.Data.UndercutNotifyValue),
                showMaxChar: false,
                showTitle: false,
                validateFunc: VmEntryValidators.ValidateFuncDouble);

            EntryFailureForMinutesNotifyValue = new VmEntry(rootObject: DcListTypeOwnAbo.Data,
                rootObjectPropertyName: nameof(DcListTypeOwnAbo.Data.FailureForMinutesNotifyValue),
                showMaxChar: false,
                showTitle: false,
                validateFunc: VmEntryValidators.ValidateFuncDouble);

            EntryMovingAverageNotifyValue = new VmEntry(rootObject: DcListTypeOwnAbo.Data,
                rootObjectPropertyName: nameof(DcListTypeOwnAbo.Data.MovingAverageNotifyValue),
                showMaxChar: false,
                showTitle: false,
                validateFunc: VmEntryValidators.ValidateFuncDouble);
        }

        /// <summary>
        /// Measurementtype Picker hat sich geaendert
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event parameter</param>
        private void PickerMeasurementTypeOnSelectedItemChanged(object sender, SelectedItemEventArgs<VmPickerElement<EnumMeasurementType>> e)
        {
            MeasurementDefinitionAssignment.Type = e.CurrentItem.Key;
        }

        /// <summary>
        /// Property hat sich geaendert
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event parameter</param>
        private void DataOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExMeasurementDefinition.AdditionalConfiguration))
            {
                UpdateStateMachineBytes();
            }
        }

        /// <summary>
        /// Update der State Machine Bytes
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void UpdateStateMachineBytes()
        {
            var baseTmp = new GcBaseConverter<GcDownstreamBase>(Data.AdditionalConfiguration);
            // ReSharper disable once RedundantAssignment
            GcDownstreamBase? tmp = null;

            switch (PickerDownstreamType.SelectedItem!.Key)
            {
                case EnumIotDeviceDownstreamTypes.Virtual:
                    tmp = baseTmp.ConvertTo<GcDownstreamVirtualFloat>();
                    break;
                case EnumIotDeviceDownstreamTypes.I2C:
                    throw new NotImplementedException();
                case EnumIotDeviceDownstreamTypes.Spi:
                    throw new NotImplementedException();
                case EnumIotDeviceDownstreamTypes.Modbus:
                    throw new NotImplementedException();
                case EnumIotDeviceDownstreamTypes.Pi:
                    throw new NotImplementedException();
                case EnumIotDeviceDownstreamTypes.Arduino:
                    throw new NotImplementedException();
                case EnumIotDeviceDownstreamTypes.Esp32:
                    tmp = baseTmp.ConvertTo<GcDownstreamEsp32>();
                    break;
                case EnumIotDeviceDownstreamTypes.DotNet:
                    tmp = baseTmp.ConvertTo<GcDownstreamDotNet>();
                    break;
                case EnumIotDeviceDownstreamTypes.Custom:
                    tmp = baseTmp.ConvertTo<GcDownstreamCustom>();
                    break;
                case EnumIotDeviceDownstreamTypes.Prebuilt:
                    tmp = baseTmp.ConvertTo<GcDownstreamPrebuilt>();
                    break;
                case EnumIotDeviceDownstreamTypes.OpenSense:
                    tmp = baseTmp.ConvertTo<GcDownstreamOpenSense>();
                    break;
                case EnumIotDeviceDownstreamTypes.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (tmp != null!)
            {
                StateMachineBytes = GcByteHelper.BytesToHexString(tmp.ToStateMachine(PickerDownstreamType.SelectedItem.Key));
            }
        }

        /// <summary>
        /// Value-Type Picker, anderes Element selektiert
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumnte</param>
        private void PickerValueTypeOnSelectedItemChanged(object sender, SelectedItemEventArgs<VmPickerElement<EnumValueTypes>> e)
        {
            PickerRawValueTypesOnSelectedItemChanged(sender, null!);
            Data.ValueType = PickerValueType.SelectedItem!.Key;
        }

        /// <summary>
        /// Config base initialisieren
        /// </summary>
        /// <param name="data">Daten</param>
        private void InitConfigBase(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                var temp = new GcDownstreamPrebuilt().ToExMeasurementDefinition();
                data = temp.AdditionalConfiguration;
            }

            if (_configBaseOriginal == null!)
            {
                _configBaseOriginal = new GcBaseConverter<GcDownstreamBase>(data);
                ConfigBase = _configBaseOriginal;
            }
            else
            {
                ConfigBase = new GcBaseConverter<GcDownstreamBase>(data);
            }

            ConfigBase.Base.RawValueDefinition.PropertyChanged += RawValueDefinitionOnPropertyChanged;
            PickerRawValueTypes.SelectKey(ConfigBase.Base.RawValueDefinition.RawValueType);
            PickerRawValueTypesOnSelectedItemChanged(this, null!);

            PickerValueType.SelectKey(Data.ValueType);
        }

        /// <summary>
        /// RawValueDefinition selektiertes item aenderung
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        private void PickerRawValueTypesOnSelectedItemChanged(object sender, SelectedItemEventArgs<VmPickerElement<EnumRawValueTypes>> e)
        {
            if (PickerDownstreamType.SelectedItem == null!)
            {
                return;
            }

            if (PickerDownstreamType.SelectedItem.Key == EnumIotDeviceDownstreamTypes.Custom || PickerDownstreamType.SelectedItem.Key == EnumIotDeviceDownstreamTypes.Prebuilt)
            {
                CanValuePickerChanged = true;

                if (_customConfigBase != null)
                {
                    _customConfigBase.ValueType = PickerValueType.SelectedItem!.Key;
                }

                if (_prebuiltConfigBase != null)
                {
                    _prebuiltConfigBase.ValueType = PickerValueType.SelectedItem!.Key;
                }

                switch (PickerValueType.SelectedItem!.Key)
                {
                    case EnumValueTypes.Number:
                        CanEditPickerRawValueTypes = true;
                        CanEditRawValueByteCount = false;
                        ShowCanEditRawValueByteCount = true;
                        ShowEditPickerRawValueTypes = true;
                        break;
                    case EnumValueTypes.Data:
                    case EnumValueTypes.Image:
                        CanEditPickerRawValueTypes = false;
                        CanEditRawValueByteCount = false;
                        ShowCanEditRawValueByteCount = true;
                        ShowEditPickerRawValueTypes = false;
                        ConfigBase.Base.RawValueDefinition.ByteCount = -1;
                        if (e == null! || e.CurrentItem == null! || e.CurrentItem.Key != EnumRawValueTypes.ByteArray)
                        {
                            PickerRawValueTypes.SelectedItemChanged -= PickerRawValueTypesOnSelectedItemChanged;
                            PickerRawValueTypes.SelectKey(EnumRawValueTypes.ByteArray);
                            PickerRawValueTypes.SelectedItemChanged += PickerRawValueTypesOnSelectedItemChanged;
                        }

                        break;
                    case EnumValueTypes.Text:
                        CanEditPickerRawValueTypes = false;
                        CanEditRawValueByteCount = false;
                        ShowCanEditRawValueByteCount = false;
                        ShowEditPickerRawValueTypes = false;
                        ConfigBase.Base.RawValueDefinition.ByteCount = -1;
                        if (e == null! || e.CurrentItem == null! || e.CurrentItem.Key != EnumRawValueTypes.Custom)
                        {
                            PickerRawValueTypes.SelectedItemChanged -= PickerRawValueTypesOnSelectedItemChanged;
                            PickerRawValueTypes.SelectKey(EnumRawValueTypes.Custom);
                            PickerRawValueTypes.SelectedItemChanged += PickerRawValueTypesOnSelectedItemChanged;
                        }

                        break;
                    case EnumValueTypes.Bit:
                        CanEditPickerRawValueTypes = false;
                        CanEditRawValueByteCount = false;

                        ShowCanEditRawValueByteCount = true;
                        ShowEditPickerRawValueTypes = true;
                        if (e == null! || e.CurrentItem == null! || e.CurrentItem.Key != EnumRawValueTypes.Bit)
                        {
                            PickerRawValueTypes.SelectedItemChanged -= PickerRawValueTypesOnSelectedItemChanged;
                            PickerRawValueTypes.SelectKey(EnumRawValueTypes.Bit);
                            PickerRawValueTypes.SelectedItemChanged += PickerRawValueTypesOnSelectedItemChanged;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                ConfigBase.Base.RawValueDefinition.RawValueType = PickerRawValueTypes.SelectedItem!.Key;

                switch (PickerRawValueTypes.SelectedItem.Key)
                {
                    case EnumRawValueTypes.Custom:
                    case EnumRawValueTypes.ByteArray:
                        CanEditRawValueByteCount = true;
                        break;
                    case EnumRawValueTypes.Bit:
                    case EnumRawValueTypes.Float:
                    case EnumRawValueTypes.Double:
                    case EnumRawValueTypes.Int16:
                    case EnumRawValueTypes.UInt16:
                    case EnumRawValueTypes.Int32:
                    case EnumRawValueTypes.UInt32:
                    case EnumRawValueTypes.Int64:
                    case EnumRawValueTypes.UInt64:
                    case EnumRawValueTypes.Byte:
                        CanEditRawValueByteCount = false;
                        ConfigBase.Base.RawValueDefinition.ByteCount = ConfigRawValueDefinition.GetByteCount(PickerRawValueTypes.SelectedItem.Key);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(PickerRawValueTypes.SelectedItem.Key));
                }
            }
            else
            {
                CanEditRawValueByteCount = false;
                CanValuePickerChanged = false;
                CanEditPickerRawValueTypes = false;
                ShowCanEditRawValueByteCount = true;
                ShowEditPickerRawValueTypes = true;
            }
        }

        /// <summary>
        /// RawValueDefinition property aenderung
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        private void RawValueDefinitionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ConfigRawValueDefinition d)
            {
                PickerRawValueTypes.SelectKey(d.RawValueType);
            }
        }

        /// <summary>
        /// PickerPredefinedMeasurements selektiertes item aenderung
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        private void PickerPredefinedMeasurementsOnSelectedItemChanged(object sender, SelectedItemEventArgs<VmPickerElement<ExMeasurementDefinition>> e)
        {
            Data.DownstreamType = e.CurrentItem.Key.DownstreamType;
            Data.Information = e.CurrentItem.Key.Information;
            Data.ValueType = e.CurrentItem.Key.ValueType;
            Data.AdditionalConfiguration = e.CurrentItem.Key.AdditionalConfiguration;

            InitConfigBase(Data.AdditionalConfiguration);
            InitViewElements();
        }

        /// <summary>
        /// Property hat sich geaendert
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        private void ConfigBaseOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is GcDownstreamBase b)
            {
                Data.AdditionalConfiguration = b.ToExMeasurementDefinition().AdditionalConfiguration;
            }
        }

        /// <summary>
        ///     Downstream Typ geändert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PickerDownstreamTypeOnSelectedItemChanged(object sender, SelectedItemEventArgs<VmPickerElement<EnumIotDeviceDownstreamTypes>> e)
        {
            if (e != null! && !_plattform.SupportedDownstreamTypes.Contains(e.CurrentItem.Key))
            {
                await MsgBox.Show($"{e.CurrentItem.Description} wird von der Plattform nicht unterstützt.").ConfigureAwait(true);
                PickerDownstreamType.SelectedItemChanged -= PickerDownstreamTypeOnSelectedItemChanged;
#pragma warning disable CS0618 // Type or member is obsolete
                PickerDownstreamType.SelectedItem = e.OldItem;
#pragma warning restore CS0618 // Type or member is obsolete
                PickerDownstreamType.SelectedItemChanged += PickerDownstreamTypeOnSelectedItemChanged;
            }
            else
            {
                InitMeasurements();
                if (_startSelectedExMeasurementDefinition != null)
                {
                    if (PickerPredefinedMeasurements.Count == 0 && e != null)
                    {
                        await MsgBox.Show("Keine freie Hardware bei der Plattform.").ConfigureAwait(true);
                        PickerDownstreamType.SelectedItemChanged -= PickerDownstreamTypeOnSelectedItemChanged;
#pragma warning disable CS0618 // Type or member is obsolete
                        PickerDownstreamType.SelectedItem = e.OldItem;
#pragma warning restore CS0618 // Type or member is obsolete
                        PickerDownstreamType.SelectedItemChanged += PickerDownstreamTypeOnSelectedItemChanged;
                        InitMeasurements();
                    }

                    if (PickerPredefinedMeasurements.Any(a => a.Key == _startSelectedExMeasurementDefinition))
                    {
                        PickerPredefinedMeasurements.SelectKey(_startSelectedExMeasurementDefinition);
                    }
                    else
                    {
                        PickerPredefinedMeasurements.SelectKey(PickerPredefinedMeasurements.FirstOrDefault().Key);
                    }
                }
            }

            Data.DownstreamType = PickerDownstreamType.SelectedItem!.Key;
        }

        /// <summary>
        /// Messwerte initialisieren
        /// </summary>
        private void InitMeasurements()
        {
            PickerPredefinedMeasurements.Clear();

            foreach (var d in _plattform.BuildInExMeasurementDefinitions)
            {
                var add = false;
                if (PickerDownstreamType.SelectedItem!.Key == EnumIotDeviceDownstreamTypes.Esp32)
                {
                    var configBase = new GcBaseConverter<GcDownstreamBase>(d.AdditionalConfiguration);
                    var config = configBase.ConvertTo<GcDownstreamEsp32>();
                    var original = _configBaseOriginal.ConvertTo<GcDownstreamEsp32>();
                    if (original.Esp32MeasurementType == config.Esp32MeasurementType)
                    {
                        if (_startSelectedExMeasurementDefinition == null!)
                        {
                            _startSelectedExMeasurementDefinition = d;
                        }

                        add = true;
                    }
                    else
                    {
                        var foundInOther = false;
                        foreach (var md in Dc.DcExMeasurementDefinition.Where(w => w.Data.IotDeviceId == IotDevice.Index && w.Data.DownstreamType == EnumIotDeviceDownstreamTypes.Esp32))
                        {
                            var configBaseOther = new GcBaseConverter<GcDownstreamBase>(md.Data.AdditionalConfiguration);
                            var configOther = configBaseOther.ConvertTo<GcDownstreamEsp32>();

                            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                            if (configOther is null)
                            {
                                continue;
                            }

                            //Hardware bereits in anderer Messwertdefinition in Verwendung
                            if (configOther.Esp32MeasurementType == config.Esp32MeasurementType)
                            {
                                foundInOther = true;
                                add = false;
                                break;
                            }
                        }

                        if (!foundInOther)
                        {
                            add = true;
                        }
                    }
                }
                else if (PickerDownstreamType.SelectedItem.Key == EnumIotDeviceDownstreamTypes.DotNet)
                {
                    var configBase = new GcBaseConverter<GcDownstreamBase>(d.AdditionalConfiguration);
                    var config = configBase.ConvertTo<GcDownstreamDotNet>();

                    var original = _configBaseOriginal.ConvertTo<GcDownstreamDotNet>();
                    if (original.DotNetMeasurementType == config.DotNetMeasurementType)
                    {
                        if (_startSelectedExMeasurementDefinition == null!)
                        {
                            _startSelectedExMeasurementDefinition = d;
                        }

                        add = true;
                    }
                    else
                    {
                        var foundInOther = false;
                        foreach (var md in Dc.DcExMeasurementDefinition.Where(w => w.Data.IotDeviceId == IotDevice.Index && w.Data.DownstreamType == EnumIotDeviceDownstreamTypes.DotNet))
                        {
                            var configBaseOther = new GcBaseConverter<GcDownstreamBase>(md.Data.AdditionalConfiguration);
                            var configOther = configBaseOther.ConvertTo<GcDownstreamDotNet>();

                            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                            if (configOther is null)
                            {
                                continue;
                            }

                            //Hardware bereits in anderer Messwertdefinition in Verwendung
                            if (configOther.DotNetMeasurementType == config.DotNetMeasurementType)
                            {
                                foundInOther = true;
                                add = false;
                                break;
                            }
                        }

                        if (!foundInOther)
                        {
                            add = true;
                        }
                    }
                }

                if (add)
                {
                    PickerPredefinedMeasurements.AddKey(d, $"{d.Information.Name}");
                }
            }


            // ReSharper disable once RedundantSuppressNullableWarningExpression
            if (PickerDownstreamType!.SelectedItem!.Key == EnumIotDeviceDownstreamTypes.Virtual)
            {
                var virtualFloat = new GcDownstreamVirtualFloat().ToExMeasurementDefinition();
                PickerPredefinedMeasurements.AddKey(virtualFloat, $"{virtualFloat.Information.Name}");
                if (_startSelectedExMeasurementDefinition == null!)
                {
                    _startSelectedExMeasurementDefinition = virtualFloat;
                }
            }
            else if (PickerDownstreamType.SelectedItem.Key == EnumIotDeviceDownstreamTypes.Custom)
            {
                var custom = new GcDownstreamCustom().ToExMeasurementDefinition();
                PickerPredefinedMeasurements.AddKey(custom, $"{custom.Information.Name}");
                if (_startSelectedExMeasurementDefinition == null!)
                {
                    _startSelectedExMeasurementDefinition = custom;
                }
            }
        }

        /// <summary>
        /// Platform initialisieren
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void InitPlattform()
        {
            switch (IotDevice.Data.Plattform)
            {
                case EnumIotDevicePlattforms.DotNet:
                    _plattform = new GcPlattformDotNet();
                    break;
                case EnumIotDevicePlattforms.RaspberryPi:
                    _plattform = new GcPlatformPi();
                    break;
                case EnumIotDevicePlattforms.Arduino:
                    _plattform = new GcPlatformArduino();
                    break;
                case EnumIotDevicePlattforms.Esp32:
                    _plattform = new GcPlattformEsp32();
                    break;
                case EnumIotDevicePlattforms.Prebuilt:
                    _plattform = new GcPlatformPrebuilt();
                    break;
                case EnumIotDevicePlattforms.OpenSense:
                    _plattform = new GcPlatformOpenSense();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var type in _plattform.SupportedDownstreamTypes)
            {
                switch (type)
                {
                    case EnumIotDeviceDownstreamTypes.Virtual:
                        PickerDownstreamType.AddKey(EnumIotDeviceDownstreamTypes.Virtual, "Anbindung eines virtuellen Sensors");
                        break;
                    case EnumIotDeviceDownstreamTypes.I2C:
                        PickerDownstreamType.AddKey(EnumIotDeviceDownstreamTypes.I2C, "Anbindung via I2C Bus");
                        break;
                    case EnumIotDeviceDownstreamTypes.Spi:
                        PickerDownstreamType.AddKey(EnumIotDeviceDownstreamTypes.Spi, "Anbindung via SPI Bus");
                        break;
                    case EnumIotDeviceDownstreamTypes.Modbus:
                        PickerDownstreamType.AddKey(EnumIotDeviceDownstreamTypes.Modbus, "Anbindung via Modbus Bus");
                        break;
                    case EnumIotDeviceDownstreamTypes.Pi:
                        PickerDownstreamType.AddKey(EnumIotDeviceDownstreamTypes.Pi, "Anbindung der \"builtin\" des Pi");
                        break;
                    case EnumIotDeviceDownstreamTypes.Arduino:
                        PickerDownstreamType.AddKey(EnumIotDeviceDownstreamTypes.Arduino, "Anbindung der \"builtin\" des Arduino");
                        break;
                    case EnumIotDeviceDownstreamTypes.Esp32:
                        PickerDownstreamType.AddKey(EnumIotDeviceDownstreamTypes.Esp32, "Anbindung der \"builtin\" des Esp32");
                        break;
                    case EnumIotDeviceDownstreamTypes.DotNet:
                        PickerDownstreamType.AddKey(EnumIotDeviceDownstreamTypes.DotNet, "Anbindung der \"builtin\" eines PC'S (Mac oder Linux)");
                        break;
                    case EnumIotDeviceDownstreamTypes.Custom:
                        PickerDownstreamType.AddKey(EnumIotDeviceDownstreamTypes.Custom, "Custom Befehle (max 255) - müssen direkt in Firmware behandelt werden");
                        break;
                    case EnumIotDeviceDownstreamTypes.Prebuilt:
                        PickerDownstreamType.AddKey(EnumIotDeviceDownstreamTypes.Prebuilt, "Vorgefertigter Sensor. Keine Einstellung möglich.");
                        break;
                    case EnumIotDeviceDownstreamTypes.OpenSense:
                        PickerDownstreamType.AddKey(EnumIotDeviceDownstreamTypes.OpenSense, "OpenSense Sensor. Keine Einstellung möglich.");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// View Elemente Iitialisieren
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void InitViewElements()
        {
            var entries = this.GetAllInstancesWithType<VmEntry>();
            for (var i = 0; i < entries.Count; i++)
            {
                entries[i] = null!;
            }

            EntryName = new VmEntry(EnumVmEntryBehavior.StopTyping,
                $"{ResViewEditMeasurementDefinition.LblName}:",
                ResViewEditMeasurementDefinition.PlaceholderName,
                Data.Information,
                nameof(ExMeasurementDefinition.Information.Name),
                VmEntryValidators.ValidateFuncStringEmpty,
                showTitle: false
            );

            EntryDescription = new VmEntry(
                EnumVmEntryBehavior.StopTyping,
                $"{ResViewEditMeasurementDefinition.LblDescription}:",
                ResViewEditMeasurementDefinition.PlaceholderDescription,
                Data.Information,
                nameof(ExMeasurementDefinition.Information.Description),
                showTitle: false
            );

            EntryMeasurmentInterval = new VmEntry(
                EnumVmEntryBehavior.StopTyping,
                "Messinterval (1=100ms):",
                "Messinterval in Zehntel Sekunden (1=100ms)",
                Data,
                nameof(ExMeasurementDefinition.MeasurementInterval),
                VmEntryValidators.ValidateFuncInt,
                showTitle: false
            );

            EntryAdditionalProperties = new VmEntry(
                EnumVmEntryBehavior.StopTyping,
                $"{ResViewEditMeasurementDefinition.LblAdditionalSettings}:",
                ResViewEditMeasurementDefinition.PlaceholderAdditionalSettings,
                Data,
                nameof(ExMeasurementDefinition.AdditionalProperties),
                showTitle: false
            );

            ShowVirtualFloat = false;
            CanValuePickerChanged = false;
            ShowCustomOpCode = false;
            _customConfigBase = null!;

            if (Data.DownstreamType == EnumIotDeviceDownstreamTypes.Virtual)
            {
                var virt = ConfigBase.ConvertTo<GcDownstreamVirtualFloat>();
                virt.PropertyChanged += ConfigBaseOnPropertyChanged;

                EntryVirtPosLat = new VmEntry(
                    EnumVmEntryBehavior.StopTyping,
                    "Zufällige Position Latitude:",
                    "GPS Latitude",
                    virt,
                    nameof(GcDownstreamVirtualFloat.AreaLatitude),
                    VmEntryValidators.ValidateFuncDouble,
                    showTitle: false,
                    showMaxChar: false
                );

                EntryVirtPosLon = new VmEntry(
                    EnumVmEntryBehavior.StopTyping,
                    "Zufällige Position :",
                    "GPS Longitude",
                    virt,
                    nameof(GcDownstreamVirtualFloat.AreaLogitute),
                    VmEntryValidators.ValidateFuncDouble,
                    showTitle: false,
                    showMaxChar: false
                );

                EntryVirtPosRadius = new VmEntry(
                    EnumVmEntryBehavior.StopTyping,
                    "Zufällige Position Radius (m):",
                    "Zufällige Position Radius (m)",
                    virt,
                    nameof(GcDownstreamVirtualFloat.AreaRadius),
                    VmEntryValidators.ValidateFuncInt,
                    showTitle: false,
                    showMaxChar: false
                );

                EntryVirtFloatMin = new VmEntry(
                    EnumVmEntryBehavior.StopTyping,
                    "Minimum zufälliger Wert:",
                    "Minimum zufälliger Wert",
                    virt,
                    nameof(GcDownstreamVirtualFloat.Min),
                    VmEntryValidators.ValidateFuncDouble,
                    showTitle: false,
                    showMaxChar: false
                );

                EntryVirtFloatMax = new VmEntry(
                    EnumVmEntryBehavior.StopTyping,
                    "Maximum zufälliger Wert:",
                    "Maximum zufälliger Wert",
                    virt,
                    nameof(GcDownstreamVirtualFloat.Max),
                    VmEntryValidators.ValidateFuncDouble,
                    showTitle: false,
                    showMaxChar: false
                );

                ShowVirtualFloat = true;
            }
            else if (Data.DownstreamType == EnumIotDeviceDownstreamTypes.Custom)
            {
                CanValuePickerChanged = true;
                ShowCustomOpCode = true;

                _customConfigBase = ConfigBase.ConvertTo<GcDownstreamCustom>();
                _customConfigBase.PropertyChanged += ConfigBaseOnPropertyChanged;
                _customConfigBase.RawValueDefinition.PropertyChanged += (sender, args) =>
                    Data.AdditionalConfiguration = _customConfigBase.ToExMeasurementDefinition().AdditionalConfiguration;

                ConfigBase.Base.RawValueDefinition.PropertyChanged += (sender, args) =>
                {
                    if (_customConfigBase != null)
                    {
                        _customConfigBase.RawValueDefinition.ByteCount = ConfigBase.Base.RawValueDefinition.ByteCount;
                        _customConfigBase.RawValueDefinition.RawValueType = ConfigBase.Base.RawValueDefinition.RawValueType;
                        if (!EntryRawValueByteCount.Value.Equals(_customConfigBase.RawValueDefinition.ByteCount.ToString(), StringComparison.InvariantCulture))
                        {
                            //Debugger.Break();
                            EntryRawValueByteCount = new VmEntry(
                                EnumVmEntryBehavior.StopTyping,
                                "Byte Anzahl:",
                                "Anzahl der Bytes (-1 = unbestimmt)",
                                ConfigBase.Base.RawValueDefinition,
                                nameof(GcDownstreamCustom.RawValueDefinition.ByteCount),
                                VmEntryValidators.ValidateFuncInt,
                                showTitle: false,
                                showMaxChar: false
                            );
                        }
                    }
                };


                EntryRawValueByteCount = new VmEntry(
                    EnumVmEntryBehavior.StopTyping,
                    "Byte Anzahl:",
                    "Anzahl der Bytes (-1 = unbestimmt)",
                    ConfigBase.Base.RawValueDefinition,
                    nameof(GcDownstreamCustom.RawValueDefinition.ByteCount),
                    VmEntryValidators.ValidateFuncInt,
                    showTitle: false,
                    showMaxChar: false
                );

                EntryCustomOpCode = new VmEntry(
                    EnumVmEntryBehavior.StopTyping,
                    "Eigene OP-Code Id:",
                    "Byte: 0-255",
                    _customConfigBase,
                    nameof(GcDownstreamCustom.StateMachineId),
                    VmEntryValidators.ValidateFuncByte,
                    showTitle: false,
                    showMaxChar: false
                );

                switch (PickerValueType.SelectedItem!.Key)
                {
                    case EnumValueTypes.Number:
                        CanEditPickerRawValueTypes = true;
                        CanEditRawValueByteCount = false;
                        ShowCanEditRawValueByteCount = true;
                        ShowEditPickerRawValueTypes = true;
                        break;
                    case EnumValueTypes.Data:
                    case EnumValueTypes.Image:
                        CanEditPickerRawValueTypes = false;
                        CanEditRawValueByteCount = false;
                        ShowCanEditRawValueByteCount = true;
                        ShowEditPickerRawValueTypes = false;
                        break;
                    case EnumValueTypes.Text:
                        CanEditPickerRawValueTypes = false;
                        CanEditRawValueByteCount = false;
                        ShowCanEditRawValueByteCount = false;
                        ShowEditPickerRawValueTypes = false;
                        break;
                    case EnumValueTypes.Bit:
                        CanEditPickerRawValueTypes = false;
                        CanEditRawValueByteCount = false;
                        ShowCanEditRawValueByteCount = true;
                        ShowEditPickerRawValueTypes = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (PickerRawValueTypes.SelectedItem!.Key)
                {
                    case EnumRawValueTypes.Custom:
                    case EnumRawValueTypes.ByteArray:
                        CanEditRawValueByteCount = true;
                        break;
                    case EnumRawValueTypes.Bit:
                    case EnumRawValueTypes.Float:
                    case EnumRawValueTypes.Double:
                    case EnumRawValueTypes.Int16:
                    case EnumRawValueTypes.UInt16:
                    case EnumRawValueTypes.Int32:
                    case EnumRawValueTypes.UInt32:
                    case EnumRawValueTypes.Int64:
                    case EnumRawValueTypes.UInt64:
                    case EnumRawValueTypes.Byte:
                        CanEditRawValueByteCount = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(PickerRawValueTypes.SelectedItem.Key));
                }
            }
            else if (Data.DownstreamType == EnumIotDeviceDownstreamTypes.Prebuilt)
            {
                CanValuePickerChanged = true;
                _prebuiltConfigBase = ConfigBase.ConvertTo<GcDownstreamPrebuilt>();
                _prebuiltConfigBase.PropertyChanged += ConfigBaseOnPropertyChanged;
                _prebuiltConfigBase.RawValueDefinition.PropertyChanged += (sender, args) =>
                    Data.AdditionalConfiguration = _prebuiltConfigBase.ToExMeasurementDefinition().AdditionalConfiguration;

                ConfigBase.Base.RawValueDefinition.PropertyChanged += (sender, args) =>
                {
                    if (_prebuiltConfigBase != null)
                    {
                        _prebuiltConfigBase.RawValueDefinition.ByteCount = ConfigBase.Base.RawValueDefinition.ByteCount;
                        _prebuiltConfigBase.RawValueDefinition.RawValueType = ConfigBase.Base.RawValueDefinition.RawValueType;
                        if (!EntryRawValueByteCount.Value.Equals(_prebuiltConfigBase.RawValueDefinition.ByteCount.ToString(), StringComparison.InvariantCulture))
                        {
                            //Debugger.Break();
                        }
                    }
                };


                EntryRawValueByteCount = new VmEntry(
                    EnumVmEntryBehavior.StopTyping,
                    "Byte Anzahl:",
                    "Anzahl der Bytes (-1 = unbestimmt)",
                    ConfigBase.Base.RawValueDefinition,
                    nameof(GcDownstreamCustom.RawValueDefinition.ByteCount),
                    VmEntryValidators.ValidateFuncInt,
                    showTitle: false,
                    showMaxChar: false
                );

                switch (PickerValueType.SelectedItem!.Key)
                {
                    case EnumValueTypes.Number:
                        CanEditPickerRawValueTypes = true;
                        CanEditRawValueByteCount = false;
                        ShowCanEditRawValueByteCount = true;
                        ShowEditPickerRawValueTypes = true;
                        break;
                    case EnumValueTypes.Data:
                    case EnumValueTypes.Image:
                        CanEditPickerRawValueTypes = false;
                        CanEditRawValueByteCount = false;
                        ShowCanEditRawValueByteCount = true;
                        ShowEditPickerRawValueTypes = false;
                        break;
                    case EnumValueTypes.Text:
                        CanEditPickerRawValueTypes = false;
                        CanEditRawValueByteCount = false;
                        ShowCanEditRawValueByteCount = false;
                        ShowEditPickerRawValueTypes = false;
                        break;
                    case EnumValueTypes.Bit:
                        CanEditPickerRawValueTypes = false;
                        CanEditRawValueByteCount = false;
                        ShowCanEditRawValueByteCount = true;
                        ShowEditPickerRawValueTypes = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (PickerRawValueTypes.SelectedItem!.Key)
                {
                    case EnumRawValueTypes.Custom:
                    case EnumRawValueTypes.ByteArray:
                        CanEditRawValueByteCount = true;
                        break;
                    case EnumRawValueTypes.Bit:
                    case EnumRawValueTypes.Float:
                    case EnumRawValueTypes.Double:
                    case EnumRawValueTypes.Int16:
                    case EnumRawValueTypes.UInt16:
                    case EnumRawValueTypes.Int32:
                    case EnumRawValueTypes.UInt32:
                    case EnumRawValueTypes.Int64:
                    case EnumRawValueTypes.UInt64:
                    case EnumRawValueTypes.Byte:
                        CanEditRawValueByteCount = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(PickerRawValueTypes.SelectedItem.Key));
                }
            }
        }

        #region Entries

        /// <summary>
        ///     Entry für Wert Überschreitung
        /// </summary>
        public VmEntry EntryExceedNotifyValue { get; set; } = null!; //new VmEntry();

        /// <summary>
        ///     Entry für Wert Unterschreitung
        /// </summary>
        public VmEntry EntryUndercutNotifyValue { get; set; } = null!; //new VmEntry();

        /// <summary>
        ///     Entry für Wert Ausfallzeit in Minuten
        /// </summary>
        public VmEntry EntryFailureForMinutesNotifyValue { get; set; } = null!; //new VmEntry();

        /// <summary>
        ///     Entry für Wert Abweichung Mittelwert
        /// </summary>
        public VmEntry EntryMovingAverageNotifyValue { get; set; } = null!; //new VmEntry();

        /// <summary>
        ///     EntryPosLat
        /// </summary>
        public VmEntry EntryVirtPosLat { get; private set; } = null!;

        /// <summary>
        ///     EntryPosLon
        /// </summary>
        public VmEntry EntryVirtPosLon { get; private set; } = null!;

        /// <summary>
        ///     EntryVirtPosRadius
        /// </summary>
        public VmEntry EntryVirtPosRadius { get; private set; } = null!;

        /// <summary>
        ///     EntryVirtFloatMin
        /// </summary>
        public VmEntry EntryVirtFloatMin { get; private set; } = null!;

        /// <summary>
        ///     EntryVirtFloatMax
        /// </summary>
        public VmEntry EntryVirtFloatMax { get; private set; } = null!;

        #endregion
    }
}