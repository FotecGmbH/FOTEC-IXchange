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
using System.Text;
using System.Threading.Tasks;
using BaseApp.Helper;
using BDA.Common.Exchange.Configs.Downstreams;
using BDA.Common.Exchange.Configs.Downstreams.OpenSense;
using BDA.Common.Exchange.Configs.Enums;
using BDA.Common.Exchange.Configs.Helper;
using BDA.Common.Exchange.Configs.Upstream;
using BDA.Common.Exchange.Configs.Upstream.Opensense;
using BDA.Common.Exchange.Configs.Upstream.Ttn;
using BDA.Common.Exchange.Configs.UserCode;
using BDA.Common.Exchange.Enum;
using BDA.Common.Exchange.Model.ConfigApp;
using BDA.Common.OpensenseClient;
using BDA.Common.ParserCompiler;
using Biss.Apps.Attributes;
using Biss.Apps.Enum;
using Biss.Apps.Interfaces;
using Biss.Apps.Map.Model;
using Biss.Apps.ViewModel;
using Biss.Collections;
using Biss.Common;
using Biss.Interfaces;
using Biss.Log.Producer;
using Biss.Serialize;
using Exchange;
using Exchange.Extensions;
using Exchange.Resources;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace BaseApp.ViewModel.Infrastructure
{
    /// <summary>
    ///     <para>Iot Device anlegen oder bearbeiten</para>
    /// Klasse VmAddIotDevice. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewEditIotDevice", true)]
    public class VmEditIotDevice : VmEditDcListPoint<ExIotDevice>
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmAddIotDevice.DesignInstance}"
        /// </summary>
        public static VmEditIotDevice DesignInstance = new VmEditIotDevice();

        /// <summary>
        /// Falls Tcp Upstream, dann ob secret eingegeben wird(device wurde bereits mit cli app erstellt)
        /// oder ob device neu erstellt wird
        /// </summary>
        private bool _enterSecret = true;

        /// <summary>
        /// Gateway Id
        /// </summary>
        private long? _gwIdOriginal;

        /// <summary>
        ///     VmAddIotDevice - Iot Device bearbeiten oder TTN Device anlegen
        /// </summary>
        public VmEditIotDevice() : base(ResViewEditIotDevice.PageTitle, subTitle: ResViewEditIotDevice.SubTitle)
        {
            View.ShowMenu = true;

            foreach (var item in EnumUtil.GetValues<EnumPositionSource>())
            {
                PickerPositionType.AddKey(item, item.GetDisplayName());
            }

            foreach (var item in EnumUtil.GetValues<EnumIotDeviceUpstreamTypes>()
                         .Where(x => x != EnumIotDeviceUpstreamTypes.None && x != EnumIotDeviceUpstreamTypes.Ble && x != EnumIotDeviceUpstreamTypes.InGateway && x != EnumIotDeviceUpstreamTypes.Serial))
            {
                PickerUpstreamType.AddKey(item, item.GetDisplayName());
            }

            foreach (var item in EnumUtil.GetValues<EnumIotDevicePlattforms>().Where(x => x != EnumIotDevicePlattforms.Arduino && x != EnumIotDevicePlattforms.Esp32))
            {
                PickerPlattformType.AddKey(item, item.GetDisplayName());
            }

            foreach (var item in EnumUtil.GetValues<EnumTransmission>())
            {
                PickerTransmissionType.AddKey(item, item.GetDisplayName());
            }

            foreach (var item in EnumUtil.GetValues<EnumOpensenseDataTimeframe>())
            {
                PickerOpensenseDataTimeframe.AddKey(item, item.GetDisplayName());
            }

            CheckBeforeSaving = BeforeSaving;
        }

        #region Properties

        /// <summary>
        /// DESCRIPTION
        /// </summary>
        public bool AdvancedMode { get; set; }


        /// <summary>
        /// Derzeitiger Zustand der View
        /// </summary>
        public ViewState CurrentViewState { get; set; } = ViewState.Default;

        /// <summary>
        ///     EntryAdditionalProperties
        /// </summary>
        public VmEntry EntryAdditionalProperties { get; set; } = null!;

        /// <summary>
        ///     Vorname
        /// </summary>
        public VmEntry EntryName { get; set; } = null!;

        /// <summary>
        ///     Nachname
        /// </summary>
        public VmEntry EntryDescription { get; set; } = null!;

        /// <summary>
        ///     EntryPosLat
        /// </summary>
        public VmEntry EntryPosLat { get; private set; } = null!;

        /// <summary>
        ///     EntryPosLon
        /// </summary>
        public VmEntry EntryPosLon { get; private set; } = null!;

        /// <summary>
        ///     EntryPosAlt
        /// </summary>
        public VmEntry EntryPosAlt { get; private set; } = null!;

        /// <summary>
        ///     EntryAdditionalConfiguration
        /// </summary>
        public VmEntry EntryAdditionalConfiguration { get; private set; } = null!;

        /// <summary>
        ///     PickerPositionType
        /// </summary>
        public VmPicker<EnumPositionSource> PickerPositionType { get; private set; } = new VmPicker<EnumPositionSource>(nameof(PickerPositionType));

        /// <summary>
        ///     PickerPositionType
        /// </summary>
        public VmPicker<long> PickerGateways { get; private set; } = new VmPicker<long>(nameof(PickerGateways));

        /// <summary>
        ///     PickerUpstreamType
        /// </summary>
        public VmPicker<EnumIotDeviceUpstreamTypes> PickerUpstreamType { get; private set; } = new VmPicker<EnumIotDeviceUpstreamTypes>(nameof(PickerUpstreamType));

        /// <summary>
        ///     PickerPlattformType
        /// </summary>
        public VmPicker<EnumIotDevicePlattforms> PickerPlattformType { get; private set; } = new VmPicker<EnumIotDevicePlattforms>(nameof(PickerPlattformType));

        /// <summary>
        ///     PickerTransmissionType
        /// </summary>
        public VmPicker<EnumTransmission> PickerTransmissionType { get; private set; } = new VmPicker<EnumTransmission>(nameof(PickerTransmissionType));

        /// <summary>
        ///     PickerOpensenseTimeframe
        /// </summary>
        public VmPicker<EnumOpensenseDataTimeframe> PickerOpensenseDataTimeframe { get; private set; } = new VmPicker<EnumOpensenseDataTimeframe>(nameof(PickerOpensenseDataTimeframe));

        /// <summary>
        /// PickerConverter
        /// </summary>
        public VmPicker<long> PickerConverter { get; private set; } = new VmPicker<long>(nameof(PickerConverter));

        /// <summary>
        ///     Beispiel Code Url.
        /// </summary>
        public string SampleCodeUrl => $"{AppSettings.Current().DcSignalHost}/content/SampleCode.png";

        /// <summary>
        ///     EntryTransmissionInterval
        /// </summary>
        public VmEntry EntryTransmissionInterval { get; set; } = null!;

        /// <summary>
        ///     EntryUserCode
        /// </summary>
        public VmEntry EntryUserCode { get; set; } = null!;

        /// <summary>
        /// EntryOpensenseBoxId
        /// </summary>
        public VmEntry EntryOpensenseBoxId { get; set; } = null!;

        /// <summary>
        ///     EntryMeasurmentInterval
        /// </summary>
        public VmEntry EntryMeasurmentInterval { get; set; } = null!;

        /// <summary>
        ///     EntrySecret
        /// </summary>
        public VmEntry EntrySecret { get; set; } = null!;

        /// <summary>
        ///     Bearbeiten eine dynamischen Konfig
        /// </summary>
        public VmCommand CmdEditDynConfig { get; private set; } = null!;

        /// <summary>
        ///     Karte öffnen und Pos auswählen Command
        /// </summary>
        public VmCommand CmdGetPositionFromMap { get; set; } = null!;

        /// <summary>
        ///     Dynamischen Konfig Command anzeigen
        /// </summary>
        public bool ShowCmdEditDynConfig { get; set; }

        /// <summary>
        ///     Kann die AdditionalConfig manuell bearbeitet werden?
        /// </summary>
        public bool ShowAdditionalConfigEntry { get; set; } = true;

        /// <summary>
        /// Codesnipped des Users
        /// </summary>
        public string CodeSnippet { get; set; } = "// Code zum Parsen der Daten";

        /// <summary>
        /// Teil des Codesnippets der die Instanzierung der Result Values beinhaltet.
        /// </summary>
        public string CodeHeader { get; set; } = "";

        /// <summary>
        /// Teil des Codesnippets der das Return-Statement beinhaltet
        /// </summary>
        public string CodeFooter { get; set; } = "";

        /// <summary>
        /// Boxid für Opensense
        /// </summary>
        public string BoxId { get; set; } = "";

        /// <summary>
        /// Datum ab wann historische Daten heruntergeladen werden sollen.
        /// </summary>
        public DateTime? OpensenseDownloadDate { get; set; } = null;

        /// <summary>
        /// Ob Historische Daten heruntergeladen werden sollen.
        /// </summary>
        public bool OpensenseDownloadData { get; set; }

        /// <summary>
        /// Falls Tcp als Upstream ausgewählt wurde hat user die möglichkeit secret zu generieren oder einzugeben
        /// </summary>
        public bool ShowSecretEntry { get; set; }

        /// <summary>
        /// Falls OpenSense als Upstream ausgewählt wurde hat user die möglichkeit die BoxId einzugeben
        /// </summary>
        public bool ShowOpenSenseBoxIdEntry { get; set; }

        /// <summary>
        /// Falls OpenSense als Upstream ausgewählt wurde hat user die möglichkeit die BoxId einzugeben
        /// </summary>
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        public bool ShowTransmissionIntervalEntry => Data?.TransmissionType is EnumTransmission.Elapsedtime || Data?.TransmissionType is EnumTransmission.NumberOfMeasurements;

        /// <summary>
        /// Ob ConverterType-Picker sichtbar ist
        /// </summary>
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        public bool ShowConverterTypePicker => Data?.Plattform == EnumIotDevicePlattforms.Prebuilt;

        /// <summary>
        /// Ob Code-Eingabe-Feld sichtbar ist
        /// </summary>
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        public bool ShowCodeAreaEntry => ShowConverterTypePicker && PickerConverter?.SelectedItem?.Key is -1;

        /// <summary>
        /// Falls Tcp Upstream, dann ob secret eingegeben wird(device wurde bereits mit cli app erstellt)
        /// oder ob device neu erstellt wird
        /// </summary>
        public bool EnterSecret
        {
            get { return _enterSecret; }
            set
            {
                if (value == false)
                {
                    EntrySecret.Value = Guid.NewGuid().ToString();
                }

                _enterSecret = value;
            }
        }

        #endregion

        /// <summary>
        ///     OnActivated (2) für View geladen noch nicht sichtbar
        ///     Nur einmal
        /// </summary>
        public override async Task OnActivated(object? args = null)
        {
            var t = base.OnActivated(args);

            Data.PropertyChanged += Data_PropertyChanged;

            if (!Dc.DcExDataconverters.Loading)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                await Dc.DcExDataconverters.WaitDataFromServerAsync(reload: true).ConfigureAwait(true);
#pragma warning restore CS0618 // Type or member is obsolete
            }

            if (Data.Upstream == EnumIotDeviceUpstreamTypes.Ttn || Data.Plattform == EnumIotDevicePlattforms.OpenSense)
            {
                ShowAdditionalConfigEntry = false;
            }

            var companyId = Data.CompanyId; //Dc.DcExGateways.First(f => f.Index == Data.Id).Data.CompanyId;
            foreach (var gw in Dc.DcExGateways.Where(g => g.Data.CompanyId == companyId))
            {
                PickerGateways.AddKey(gw.Index, $"{gw.Data.Information.Name} ({gw.Index})");
            }

            Data.GatewayId = Dc.DcExGateways.FirstOrDefault(g => g.Data.Information.Name == Dc.DcExUser.Data.LoginName).Id;
            //PickerGateways.SelectedItem = PickerGateways.AllItems.FirstOrDefault(g=>g.Description.Contains(Dc.DcExUser.Data.LoginName));
            _gwIdOriginal = Data.GatewayId;

            EntryName = new VmEntry(EnumVmEntryBehavior.StopTyping,
                "Name:",
                "Name des Iot-Geräts",
                Data.Information,
                nameof(ExIotDevice.Information.Name),
                VmEntryValidators.ValidateFuncStringEmpty,
                showTitle: false
            );

            EntryDescription = new VmEntry(
                EnumVmEntryBehavior.StopTyping,
                "Beschreibung:",
                "Beschreibung",
                Data.Information,
                nameof(ExIotDevice.Information.Description),
                showTitle: false
            );

            EntryPosLat = new VmEntry(
                EnumVmEntryBehavior.StopTyping,
                "Latitude:",
                "GPS Latitude",
                Data.Location,
                nameof(ExIotDevice.Location.Latitude),
                VmEntryValidators.ValidateFuncDouble,
                showTitle: false,
                showMaxChar: false
            );

            EntryPosLon = new VmEntry(
                EnumVmEntryBehavior.StopTyping,
                "Longitude:",
                "GPS Longitude",
                Data.Location,
                nameof(ExIotDevice.Location.Longitude),
                VmEntryValidators.ValidateFuncDouble,
                showTitle: false,
                showMaxChar: false
            );

            EntryPosAlt = new VmEntry(
                EnumVmEntryBehavior.StopTyping,
                "Altitude:",
                "GPS Altitude",
                Data.Location,
                nameof(ExIotDevice.Location.Altitude),
                VmEntryValidators.ValidateFuncDouble,
                showTitle: false,
                showMaxChar: false
            );
            EntrySecret = new VmEntry(
                EnumVmEntryBehavior.StopTyping,
                "Secret:",
                "Secret",
                Data.DeviceCommon,
                nameof(ExIotDevice.DeviceCommon.Secret),
                str =>
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                    if (PickerUpstreamType != null && PickerUpstreamType.SelectedItem != null && PickerUpstreamType.SelectedItem.Key == EnumIotDeviceUpstreamTypes.Tcp)
                    {
                        return VmEntryValidators.ValidateFuncStringEmpty(str);
                    }

                    return (string.Empty, true);
                }
                ,
                showTitle: false,
                showMaxChar: false
            );


            EntryAdditionalProperties = new VmEntry(
                EnumVmEntryBehavior.StopTyping,
                "Zusätzliche Einstellungen:",
                "Zusätzliche Einstellungen",
                Data,
                nameof(ExIotDevice.AdditionalProperties),
                showTitle: false
            );

            EntryMeasurmentInterval = new VmEntry(
                EnumVmEntryBehavior.StopTyping,
                "Messinterval (1=100ms):",
                "Messinterval in Zehntel Sekunden (1=100ms)",
                Data,
                nameof(ExIotDevice.MeasurmentInterval),
                VmEntryValidators.ValidateFuncInt,
                showTitle: false
            );

            EntryTransmissionInterval = new VmEntry(
                EnumVmEntryBehavior.StopTyping,
                "Übertragungsinterval (s/n):",
                "Übertragungsinterval in Sekunden oder nach n Messungen",
                Data,
                nameof(ExIotDevice.TransmissionInterval),
                VmEntryValidators.ValidateFuncInt,
                showTitle: false
            );

            EntryUserCode = new VmEntry(
                EnumVmEntryBehavior.StopTyping,
                "Usercode",
                "",
                this,
                nameof(CodeSnippet),
                showTitle: false
            );

            EntryOpensenseBoxId = new VmEntry(
                EnumVmEntryBehavior.StopTyping,
                "BoxID",
                "Opensense Box ID",
                this,
                nameof(BoxId),
                showTitle: false
            );

            PropertyChanged += (sender, eventArgs) =>
            {
                if (eventArgs.PropertyName == nameof(CurrentViewState) && CurrentViewState == ViewState.PrebuiltCustomcode)
                {
                    InitCodeSnipped();
                }

                if (eventArgs.PropertyName == nameof(CodeSnippet) || eventArgs.PropertyName == nameof(BoxId) || eventArgs.PropertyName == nameof(OpensenseDownloadData))
                {
                    UpdateAdditionalConfig();
                }
            };


            PickerPositionType.SelectedItem = PickerPositionType.First(f => f.Key == Data.Location.Source);
            PickerPositionType.SelectedItemChanged += (sender, eventArgs) => Data.Location.Source = eventArgs.CurrentItem.Key;

            PickerOpensenseDataTimeframe.SelectedItem = PickerOpensenseDataTimeframe.First();
            PickerOpensenseDataTimeframe.SelectedItemChanged += (sender, eventArgs) => UpdateAdditionalConfig();

            PickerGateways.SelectedItem = PickerGateways.FirstOrDefault(f => f.Key == Data.GatewayId);
            PickerGateways.SelectedItemChanged += PickerGatewaysOnSelectedItemChanged;

            PickerUpstreamType.SelectedItem = PickerUpstreamType.First(f => f.Key == Data.Upstream);
            PickerUpstreamType.SelectedItemChanged += PickerUpstreamTypeOnSelectedItemChanged;

            PickerPlattformType.SelectedItem = PickerPlattformType.First(f => f.Key == Data.Plattform);
            PickerPlattformType.SelectedItemChanged += PickerPlattformType_SelectedItemChanged;

            PickerTransmissionType.SelectedItem = PickerTransmissionType.First(f => f.Key == Data.TransmissionType);
            PickerTransmissionType.SelectedItemChanged += PickerTransmissionTypeOnSelectedItemChanged;

            PickerConverter.AddKey(-1, DcListDataPoint.Index == -1 ? "Eigener Code" : "Nur bei neu angelegten Devices", "Eigenen Code schreiben.");
            // ReSharper disable once UnusedVariable
            var conv = Dc.DcExDataconverters.FirstOrDefault(c => c.Index == Data.DataConverterId);
            PickerConverter.SelectedItemChanged += PickerConverterOnSelectedItemChanged;
            PickerConverter.SelectKey(-1);

            if (DcListDataPoint.Index == -1)
            {
                foreach (var converter in Dc.DcExDataconverters)
                {
                    PickerConverter.AddKey(converter.Index, converter.Data.Displayname, converter.Data.Description);
                }
            }

            UpdateState();
            ReadConfig();
            CheckBooleans();

            await t;
        }

        /// <summary>
        /// Usercode in den AdditionalConfiguration updaten
        /// </summary>
        public void UpdateAdditionalConfig()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (Data == null)
            {
                Logging.Log.LogWarning($"[{nameof(VmEditIotDevice)}]({nameof(UpdateAdditionalConfig)}): Data is null");
                return;
            }


            if (Data.Plattform == EnumIotDevicePlattforms.Prebuilt)
            {
                var usercode = new ExUsercode {Header = CodeHeader, UserCode = CodeSnippet, Footer = CodeFooter};
                try
                {
                    var config = BissDeserialize.FromJson<GcTtnIotDevice>(Data.AdditionalConfiguration);
                    config.UserCode = usercode;
                    config.ConfigType = EnumGlobalConfigTypes.Ttn;
                    Data.AdditionalConfiguration = config.ToJson();
                }
                catch (InvalidOperationException e)
                {
                    Dispatcher?.RunOnDispatcher(async () => await MsgBox.Show(e.Message).ConfigureAwait(false));
                }
            }

            if (Data.Plattform == EnumIotDevicePlattforms.OpenSense)
            {
                try
                {
                    var config = new GcBaseConverter<GcOpenSenseIotDevice>(Data.AdditionalConfiguration).Base;
                    config.UserCode = null;
                    config.ConfigType = EnumGlobalConfigTypes.OpenSense;
                    config.BoxId = BoxId;

                    if (OpensenseDownloadData)
                    {
                        config.DownloadDataSince = DateTime.Now.ToUniversalTime().AddDays(-1 * (int) PickerOpensenseDataTimeframe.SelectedItem!.Key);
                    }
                    else
                    {
                        config.DownloadDataSince = null;
                    }

                    Data.AdditionalConfiguration = config.ToJson();
                    Data.Upstream = EnumIotDeviceUpstreamTypes.OpenSense;
                }
                catch (InvalidOperationException e)
                {
                    Dispatcher?.RunOnDispatcher(async () => await MsgBox.Show(e.Message).ConfigureAwait(false));
                }
            }
        }


        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdEditDynConfig = new VmCommand("", async () =>
            {
                IsNavigatedToNavToViewWithResult = true;
                _ = await Nav.ToViewWithResult(typeof(VmConfigs), Data).ConfigureAwait(true);
                await View.RefreshAsync().ConfigureAwait(true);
                IsNavigatedToNavToViewWithResult = false;

                if (DeviceInfo.Plattform == EnumPlattform.XamarinWpf)
                {
                    GCmdShowMenu.Execute(null!);
                }
            }, glyph: Glyphs.Pencil);

            CmdGetPositionFromMap = new VmCommand(ResViewEditIotDevice.LblChooseOnMap, async () =>
            {
                IsNavigatedToNavToViewWithResult = true;
                var result = await Nav.ToViewWithResult(typeof(VmEditMapPosition), Data.Location.ToBissPosition()).ConfigureAwait(true);
                IsNavigatedToNavToViewWithResult = false;

                if (result is BissPosition position)
                {
                    Data.Location.Latitude = position.Latitude;
                    // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                    EntryPosLat.BindingData = Data.Location.Latitude.ToString();

                    Data.Location.Longitude = position.Longitude;
                    // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                    EntryPosLon.BindingData = Data.Location.Longitude.ToString();

                    await View.RefreshAsync().ConfigureAwait(true);
                }
            }, glyph: Glyphs.Maps_search);

            base.InitializeCommands();
        }

        /// <summary>
        /// Property changed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event argumente</param>
        private void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExIotDevice.Upstream))
            {
                CheckBooleans();
            }
        }

        /// <summary>
        /// Booleans checken
        /// </summary>
        private void CheckBooleans()
        {
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            ShowOpenSenseBoxIdEntry = Data?.Upstream is EnumIotDeviceUpstreamTypes.OpenSense;
            ShowSecretEntry = Data?.Upstream is EnumIotDeviceUpstreamTypes.Tcp;
            ShowCmdEditDynConfig = Data?.Upstream is EnumIotDeviceUpstreamTypes.Ttn;
        }

        /// <summary>
        /// Converter-Picker hat sich geaendert
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event parameter</param>
        private void PickerConverterOnSelectedItemChanged(object sender, SelectedItemEventArgs<VmPickerElement<long>> e)
        {
            if (e.CurrentItem.Key == -1)
            {
                Data.DataConverterId = null;
            }
            else
            {
                Data.DataConverterId = e.CurrentItem.Key;
            }

            if (Data.Plattform != EnumIotDevicePlattforms.Prebuilt)
            {
                Data.DataConverterId = null;
                return;
            }

            InitCodeSnipped();
            UpdateState();
            Logging.Log.LogInfo($"[{nameof(VmEditIotDevice)}]({nameof(PickerConverterOnSelectedItemChanged)}): Set Converter to {e.CurrentItem.Description}");
        }

        /// <summary>
        /// Initialisieren des Code-Snippets
        /// </summary>
        private void InitCodeSnipped()
        {
            CodeFooter = $"\treturn results; {Environment.NewLine}}}";

            var definitions = Dc.DcExMeasurementDefinition.Where(l => l.Data.IotDeviceId == DcListDataPoint.Index).ToArray();
            var headerBuilder = new StringBuilder("");
            headerBuilder.AppendLine("public static ExValue[] Convert(byte[] input)");
            headerBuilder.AppendLine("{");
            headerBuilder.AppendLine($"\tvar results = new ExValue[{definitions.Length}];");
            headerBuilder.AppendLine("");

            for (var i = 0; i < definitions.Length; i++)
            {
                headerBuilder.AppendLine($"\t// {definitions[i].Data.Information.Name}");
                headerBuilder.AppendLine($"\tresults[{i}] = new ExValue() {{ Identifier = {definitions[i].Index}, ValueType = {definitions[i].Data.ValueType.GetType().Name}.{definitions[i].Data.ValueType}}};");
                headerBuilder.AppendLine("");
            }

            //EnsureCorrectConfiguration();
            CodeHeader = headerBuilder.ToString();
            var currentPropsObject = BissDeserialize.FromJson<JObject>(Data.AdditionalConfiguration);
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            var currentUsercode = currentPropsObject?.GetValue("UserCode", StringComparison.InvariantCulture);
            CodeSnippet = currentUsercode?.ToObject<ExUsercode>()?.UserCode ?? "";
            EntryUserCode.Value = CodeSnippet;
        }

        /// <summary>
        /// Konfiguration lesen
        /// </summary>
        private void ReadConfig()
        {
            var currentConfigObj = BissDeserialize.FromJson<JObject?>(Data.AdditionalConfiguration);
            if (currentConfigObj is null)
            {
                Data.AdditionalConfiguration = new GcIotDevice().ToJson();
                currentConfigObj = BissDeserialize.FromJson<JObject>(Data.AdditionalConfiguration);
            }

            if (currentConfigObj.TryGetValue("BoxId", StringComparison.InvariantCultureIgnoreCase, out var boxId))
            {
                BoxId = boxId.Value<string>() ?? "";
                EntryOpensenseBoxId.Value = BoxId;
                this.InvokeOnPropertyChanged(nameof(BoxId));
            }
        }

        /// <summary>
        /// Arbeiten vor dem Speichern
        /// </summary>
        /// <returns>Ob erfolgreich</returns>
        private async Task<bool> BeforeSaving()
        {
            if (Data.Upstream == EnumIotDeviceUpstreamTypes.Ttn)
            {
                var additionalConfigOk = false;
                if (!string.IsNullOrEmpty(Data.AdditionalConfiguration))
                {
                    try
                    {
                        _ = BissDeserialize.FromJson<GcTtnIotDevice>(Data.AdditionalConfiguration);
                        additionalConfigOk = true;
                    }
                    catch (Exception exception)
                    {
                        Logging.Log.LogWarning($"[{nameof(VmEditIotDevice)}]({nameof(BeforeSaving)}): {exception}");
                        throw;
                    }
                }

                if (!additionalConfigOk)
                {
                    await MsgBox.Show("Bitte prüfen Sie Ihre TTN Konfiguration.\r\n Speichern nicht möglich").ConfigureAwait(true);
                    return false;
                }
            }

            if (CurrentViewState == ViewState.PrebuiltCustomcode)
            {
                try
                {
                    var usercodeToken = BissDeserialize.FromJson<JObject>(Data.AdditionalConfiguration)["UserCode"];
                    if (usercodeToken is null)
                    {
                        Dispatcher?.RunOnDispatcher(async () => await MsgBox.Show("Kein Usercode gefunden", "Error").ConfigureAwait(false));
                        return false;
                    }

                    var codeJson = usercodeToken.ToString();
                    var code = BissDeserialize.FromJson<ExUsercode>(codeJson);

                    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                    if (code is null)
                    {
                        Dispatcher?.RunOnDispatcher(async () => await MsgBox.Show("Kein Usercode gefunden", "Error").ConfigureAwait(false));
                        return false;
                    }


                    // Testen ob der Usercode valide ist.
                    var assemblyData = await Compiler.GetAssembly(code.CompleteCode).ConfigureAwait(false);
                    _ = assemblyData.DisposeAsync().ConfigureAwait(false);
                }
                catch (InvalidOperationException e)
                {
                    Dispatcher?.RunOnDispatcher(async () => await MsgBox.Show(e.Message, "Error").ConfigureAwait(false));
                    return false;
                }
            }

            if (Data.Upstream == EnumIotDeviceUpstreamTypes.OpenSense)
            {
                var client = new OpensenseClient();
                var config = new GcBaseConverter<GcIotDevice>(Data.AdditionalConfiguration).ConvertTo<GcOpenSenseIotDevice>();
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (config != null && config.BoxId != null)
                {
                    var sensors = await client.GetCurrentValuesAsync(config.BoxId).ConfigureAwait(false);
                    var existingIds = Dc.DcExMeasurementDefinition.Where(def => def.Data.IotDeviceId == DcListDataPoint.Index).Select(d =>
                    {
                        var result = "";
                        try
                        {
                            result = new GcBaseConverter<GcDownstreamBase>(d.Data.AdditionalConfiguration).ConvertTo<GcDownstreamOpenSense>().SensorID;
                        }
                        catch
                        {
                            //Ignored
                        }

                        return result;
                    }).Where(s => !string.IsNullOrEmpty(s));
                    foreach (var sensor in sensors)
                    {
                        // ReSharper disable once PossibleMultipleEnumeration
                        if (existingIds.Contains(sensor.OpensenseId))
                        {
                            continue;
                        }

                        var definition = new ExMeasurementDefinition
                                         {
                                             AdditionalConfiguration = (new GcDownstreamOpenSense {SensorID = sensor.OpensenseId}).ToJson(),
                                             CompanyId = Data.CompanyId ?? -1,
                                             DownstreamType = EnumIotDeviceDownstreamTypes.OpenSense,
                                             ValueType = EnumValueTypes.Text,
                                             Information = new ExInformation
                                                           {
                                                               CreatedDate = DateTime.UtcNow,
                                                               Description = $"Sensorid: {sensor.OpensenseId}",
                                                               Name = sensor.Title
                                                           },
                                         };
                        Data.MeasurementDefinitions.Add(definition);
                    }
                }
                else
                {
                    Dispatcher?.RunOnDispatcher(async () => await MsgBox.Show("Konnte Box auf Opensense nicht finden.", "Error").ConfigureAwait(false));
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// TransmissionType Picker aenderung
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">eventargumente</param>
        private async void PickerTransmissionTypeOnSelectedItemChanged(object sender, SelectedItemEventArgs<VmPickerElement<EnumTransmission>> e)
        {
            await MsgBox.Show("Diese Funktion is in einer späteren Version möglich!").ConfigureAwait(true);
            PickerTransmissionType.SelectedItemChanged -= PickerTransmissionTypeOnSelectedItemChanged;
#pragma warning disable CS0618 // Type or member is obsolete
            // ReSharper disable once RedundantSuppressNullableWarningExpression
            PickerTransmissionType.SelectedItem = e.OldItem!;
#pragma warning restore CS0618 // Type or member is obsolete
            PickerTransmissionType.SelectedItemChanged += PickerTransmissionTypeOnSelectedItemChanged;
        }

        /// <summary>
        /// Statusaenderung
        /// </summary>
        private void UpdateState()
        {
            if (Data.Plattform == EnumIotDevicePlattforms.OpenSense)
            {
                CurrentViewState = ViewState.OpenSense;
            }
            else if (Data.Plattform == EnumIotDevicePlattforms.Prebuilt && PickerConverter.SelectedItem!.Key == -1)
            {
                CurrentViewState = ViewState.PrebuiltCustomcode;
            }
            else if (Data.Plattform == EnumIotDevicePlattforms.Prebuilt)
            {
                CurrentViewState = ViewState.Prebuilt;
            }
            else
            {
                CurrentViewState = ViewState.Default;
            }

            MatchPlatformAndUpstream(false);
        }

        /// <summary>
        /// PickerPlattformType geaendert
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        private void PickerPlattformType_SelectedItemChanged(object sender, SelectedItemEventArgs<VmPickerElement<EnumIotDevicePlattforms>> e)
        {
            Data.Plattform = e.CurrentItem.Key;
            if (Data.Plattform != EnumIotDevicePlattforms.Prebuilt)
            {
                Data.DataConverterId = null;
            }

            UpdateState();
            UpdateAdditionalConfig();
        }

        /// <summary>
        /// UpstreamType Picker aenderung
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        private async void PickerUpstreamTypeOnSelectedItemChanged(object sender, SelectedItemEventArgs<VmPickerElement<EnumIotDeviceUpstreamTypes>> e)
        {
            if (e.CurrentItem.Key == EnumIotDeviceUpstreamTypes.OpenSense || e.CurrentItem.Key == EnumIotDeviceUpstreamTypes.Ttn || e.CurrentItem.Key == EnumIotDeviceUpstreamTypes.Tcp)
            {
                Data.Upstream = e.CurrentItem.Key;
                MatchPlatformAndUpstream(true);

                return;
            }

            await MsgBox.Show("Das ist leider nicht möglich!").ConfigureAwait(true);
            PickerUpstreamType.SelectedItemChanged -= PickerUpstreamTypeOnSelectedItemChanged;
#pragma warning disable CS0618 // Type or member is obsolete
            PickerUpstreamType.SelectedItem = e.OldItem;
#pragma warning restore CS0618 // Type or member is obsolete
            View.Refresh();
            PickerUpstreamType.SelectedItemChanged += PickerUpstreamTypeOnSelectedItemChanged;
        }

        /// <summary>
        ///     Gateway Änderung
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        private async void PickerGatewaysOnSelectedItemChanged(object sender, SelectedItemEventArgs<VmPickerElement<long>> e)
        {
            if (_gwIdOriginal != null && e.CurrentItem.Key != _gwIdOriginal)
            {
                var msg = await MsgBox.Show("Iot Gerät wirklich einem anderen Gateway zuweisen?",
                    "Achtung",
                    VmMessageBoxButton.YesNo).ConfigureAwait(true);

                if (msg == VmMessageBoxResult.No)
                {
                    PickerGateways.SelectedItemChanged -= PickerGatewaysOnSelectedItemChanged;
#pragma warning disable CS0618 // Type or member is obsolete
                    PickerGateways.SelectedItem = e.OldItem;
#pragma warning restore CS0618 // Type or member is obsolete
                    PickerGateways.SelectedItemChanged += PickerGatewaysOnSelectedItemChanged;

                    return;
                }
            }

            Data.GatewayId = e.CurrentItem.Key;
        }

        /// <summary>
        /// Sobald etwas ausgewaehlt wird was nicht sein soll, z.b. Upstream TTN & Platform OpenSense
        /// </summary>
        /// <param name="triggeredFromUpstream">ausgehend von upstream oder platform?</param>
        private void MatchPlatformAndUpstream(bool triggeredFromUpstream)
        {
            if (triggeredFromUpstream)
            {
                switch (Data.Upstream)
                {
                    case EnumIotDeviceUpstreamTypes.OpenSense:
                        if (Data.Plattform != EnumIotDevicePlattforms.OpenSense)
                        {
                            Data.Plattform = EnumIotDevicePlattforms.OpenSense;
                            PickerPlattformType.SelectedItem = PickerPlattformType.FirstOrDefault(pp => pp.Key == EnumIotDevicePlattforms.OpenSense);
                        }

                        break;
                    case EnumIotDeviceUpstreamTypes.Tcp:
                        if (Data.Plattform != EnumIotDevicePlattforms.RaspberryPi && Data.Plattform != EnumIotDevicePlattforms.DotNet)
                        {
                            Data.Plattform = EnumIotDevicePlattforms.RaspberryPi;
                            PickerPlattformType.SelectedItem = PickerPlattformType.FirstOrDefault(pp => pp.Key == EnumIotDevicePlattforms.RaspberryPi);
                        }

                        break;
                    case EnumIotDeviceUpstreamTypes.Ttn:
                        if (Data.Plattform != EnumIotDevicePlattforms.Prebuilt)
                        {
                            Data.Plattform = EnumIotDevicePlattforms.Prebuilt;
                            PickerPlattformType.SelectedItem = PickerPlattformType.FirstOrDefault(pp => pp.Key == EnumIotDevicePlattforms.Prebuilt);
                        }

                        break;
                }
            }
            else
            {
                switch (Data.Plattform)
                {
                    case EnumIotDevicePlattforms.OpenSense:
                        if (Data.Upstream != EnumIotDeviceUpstreamTypes.OpenSense)
                        {
                            Data.Upstream = EnumIotDeviceUpstreamTypes.OpenSense;
                            PickerUpstreamType.SelectedItem = PickerUpstreamType.FirstOrDefault(pu => pu.Key == EnumIotDeviceUpstreamTypes.OpenSense);
                        }

                        break;
                    case EnumIotDevicePlattforms.DotNet:
                    case EnumIotDevicePlattforms.RaspberryPi:
                        if (Data.Upstream != EnumIotDeviceUpstreamTypes.Tcp)
                        {
                            Data.Upstream = EnumIotDeviceUpstreamTypes.Tcp;
                            PickerUpstreamType.SelectedItem = PickerUpstreamType.FirstOrDefault(pu => pu.Key == EnumIotDeviceUpstreamTypes.Tcp);
                        }

                        break;
                    case EnumIotDevicePlattforms.Prebuilt:
                        if (Data.Upstream != EnumIotDeviceUpstreamTypes.Ttn)
                        {
                            Data.Upstream = EnumIotDeviceUpstreamTypes.Ttn;
                            PickerUpstreamType.SelectedItem = PickerUpstreamType.FirstOrDefault(pu => pu.Key == EnumIotDeviceUpstreamTypes.Ttn);
                        }

                        break;
                }
            }
        }
    }

    /// <summary>
    /// View Status
    /// </summary>
    public enum ViewState
    {
        /// <summary>
        /// PrebuiltCustomcode
        /// </summary>
        PrebuiltCustomcode,
        /// <summary>
        /// Prebuilt
        /// </summary>
        Prebuilt,
        /// <summary>
        /// OpenSense
        /// </summary>
        OpenSense,
        /// <summary>
        /// Default
        /// </summary>
        Default
    }

    /// <summary>
    /// View Element
    /// </summary>
    public enum ViewElement
    {
        /// <summary>
        /// Upstream
        /// </summary>
        Upstream,
        /// <summary>
        /// ConverterType
        /// </summary>
        ConverterType,
        /// <summary>
        /// TransmissionType
        /// </summary>
        TransmissionType,
        /// <summary>
        /// TransmissionInterval
        /// </summary>
        TransmissionInterval,
        /// <summary>
        /// MeasurementInterval
        /// </summary>
        MeasurementInterval,
        /// <summary>
        /// CodeArea
        /// </summary>
        CodeArea,
        /// <summary>
        /// OpensenseBoxId
        /// </summary>
        OpensenseBoxId,
        /// <summary>
        /// OpensenseHistoricalData
        /// </summary>
        OpensenseHistoricalData
    }
}