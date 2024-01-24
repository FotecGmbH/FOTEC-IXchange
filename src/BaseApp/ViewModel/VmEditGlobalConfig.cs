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
using System.Threading.Tasks;
using BaseApp.Helper;
using BDA.Common.Exchange.Configs.GlobalConfigs;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Apps.Attributes;
using Biss.Apps.Enum;
using Biss.Apps.ViewModel;
using Biss.Serialize;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    ///     <para>Globale Konfiguration bearbeiten</para>
    /// Klasse VmEditGlobalConfig. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewEditGlobalConfig", true)]
    public class VmEditGlobalConfig : VmEditDcListPoint<ExGlobalConfig>
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmEditGlobalConfig.DesignInstance}"
        /// </summary>
        public static VmEditGlobalConfig DesignInstance = new VmEditGlobalConfig();

        /// <summary>
        /// TTn Konfiguration
        /// </summary>
        private GcTtn _ttnConfig = null!;

        /// <summary>
        ///     VmEditGlobalConfig
        /// </summary>
        public VmEditGlobalConfig() : base("Globale Konfiguration", subTitle: "Globale TTN Einstellungen für die Firma")
        {
            SetViewProperties(true);
        }

        #region Properties

        /// <summary>
        ///     EntryName
        /// </summary>
        public VmEntry EntryName { get; private set; } = null!;

        /// <summary>
        ///     EntryTtnZone
        /// </summary>
        public VmEntry EntryTtnZone { get; set; } = null!;

        /// <summary>
        ///     EntryTtnAppId
        /// </summary>
        public VmEntry EntryTtnAppId { get; set; } = null!;

        /// <summary>
        ///     EntryTtnUserid
        /// </summary>
        public VmEntry EntryTtnUserId { get; set; } = null!;


        /// <summary>
        ///     EntryTtnApiKey
        /// </summary>
        public VmEntry EntryTtnApiKey { get; set; } = null!;

        /// <summary>
        ///     EntryDescription
        /// </summary>
        public VmEntry EntryDescription { get; set; } = null!;

        #endregion


        /// <summary>
        ///     OnActivated (2) für View geladen noch nicht sichtbar
        ///     Nur einmal
        /// </summary>
        public override Task OnActivated(object? args = null)
        {
            var r = base.OnActivated(args);

            EntryName = new VmEntry(EnumVmEntryBehavior.StopTyping,
                $"{ResViewEditGlobalConfig.LblTtnName}:",
                ResViewEditGlobalConfig.PlaceholderTtnName,
                Data.Information,
                nameof(ExGlobalConfig.Information.Name),
                VmEntryValidators.ValidateFuncStringEmpty,
                showTitle: false
            );

            EntryDescription = new VmEntry(
                EnumVmEntryBehavior.StopTyping,
                $"{ResViewEditGlobalConfig.LblTtnDescription}:",
                ResViewEditGlobalConfig.PlaceholderTtnDescription,
                Data.Information,
                nameof(ExGlobalConfig.Information.Description),
                showTitle: false
            );

            if (string.IsNullOrEmpty(Data.AdditionalConfiguration))
            {
                _ttnConfig = new GcTtn();
            }
            else
            {
                _ttnConfig = BissDeserialize.FromJson<GcTtn>(Data.AdditionalConfiguration);
            }

            EntryTtnZone = new VmEntry(EnumVmEntryBehavior.StopTyping,
                $"{ResViewEditGlobalConfig.LblTtnZone}:",
                ResViewEditGlobalConfig.PlaceholderTtnZone,
                _ttnConfig,
                nameof(GcTtn.Zone),
                VmEntryValidators.ValidateFuncStringEmpty,
                showTitle: false
            );

            EntryTtnApiKey = new VmEntry(EnumVmEntryBehavior.StopTyping,
                $"{ResViewEditGlobalConfig.LblTtnApiKey}:",
                ResViewEditGlobalConfig.PlaceholderTtnApiKey,
                _ttnConfig,
                nameof(GcTtn.ApiKey),
                VmEntryValidators.ValidateFuncStringEmpty,
                showTitle: false
            );

            EntryTtnAppId = new VmEntry(EnumVmEntryBehavior.StopTyping,
                $"{ResViewEditGlobalConfig.LblTtnApplicationId}:",
                ResViewEditGlobalConfig.PlaceholderTtnAppId,
                _ttnConfig,
                nameof(GcTtn.Applicationid),
                VmEntryValidators.ValidateFuncStringEmpty,
                showTitle: false
            );

            EntryTtnUserId = new VmEntry(EnumVmEntryBehavior.StopTyping,
                $"{ResViewEditGlobalConfig.LblTtnUserId}:",
                ResViewEditGlobalConfig.PlaceholderTtnUserId,
                _ttnConfig,
                nameof(GcTtn.Userid),
                VmEntryValidators.ValidateFuncStringEmpty,
                showTitle: false
            );


            EntryName.PropertyChanged += EntryNameOnPropertyChanged;
            EntryTtnZone.PropertyChanged += EntryTtnOnPropertyChanged;
            EntryTtnApiKey.PropertyChanged += EntryTtnOnPropertyChanged;
            EntryTtnAppId.PropertyChanged += EntryTtnOnPropertyChanged;
            EntryTtnUserId.PropertyChanged += EntryTtnOnPropertyChanged;

            return r;
        }

        /// <summary>
        /// TTN Entry property Aenderung
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">event argumente</param>
        private void EntryTtnOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VmEntry.Value))
            {
                Data.AdditionalConfiguration = _ttnConfig.ToJson();
            }
        }

        /// <summary>
        /// Name Entry property Aenderung
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">event argumente</param>
        private void EntryNameOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VmEntry.Value))
            {
                _ttnConfig.Name = Data.Information.Name;
                Data.AdditionalConfiguration = _ttnConfig.ToJson();
            }
        }
    }
}