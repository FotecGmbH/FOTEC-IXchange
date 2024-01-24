// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using BDA.Common.Exchange.Enum;
using Biss.Apps.Attributes;
using Biss.Apps.ViewModel;
using Biss.Collections;
using Biss.Common;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    /// Event argument
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventArg<T> : EventArgs
    {
        /// <summary>
        /// Event argument
        /// </summary>
        /// <param name="data">Daten</param>
        public EventArg(T data)
        {
            Data = data;
        }

        #region Properties

        /// <summary>
        /// Daten
        /// </summary>
        public T Data { get; set; }

        #endregion
    }

    /// <summary>
    ///     <para>App Einstellungen</para>
    /// Klasse VmSettings. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewSettings")]
    public class VmSettings : VmProjectBase
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmSettings.DesignInstance}"
        /// </summary>
        public static VmSettings DesignInstance = new VmSettings();

        /// <summary>
        ///     VmSettings
        /// </summary>
        public VmSettings() : base(ResViewSettings.LblTitle, subTitle: ResViewSettings.LblSubTitle)
        {
            SetViewProperties();


            ThemePicker.AddKey(EnumTheme.Dark, ResViewSettings.LblDark);
            ThemePicker.AddKey(EnumTheme.Light, ResViewSettings.LblLight);

            if (Dc.DcExLocalAppData.Data.UseDarkTheme)
            {
                ThemePicker.SelectKey(EnumTheme.Dark);
            }
            else
            {
                ThemePicker.SelectKey(EnumTheme.Light);
            }

            ThemePicker.SelectedItemChanged += ThemePickerOnSelectedItemChanged;
        }

        #region Properties

        /// <summary>
        ///     Picker für das Verhalten
        /// </summary>
        public VmPicker<EnumTheme> ThemePicker { get; } = new VmPicker<EnumTheme>(nameof(ThemePicker));

        /// <summary>
        ///     Test Command
        /// </summary>
        public VmCommand CmdSettingsPush { get; set; } = null!;

        /// <summary>
        ///     Werden die Pushsettings auf der Plattform unterstützt
        /// </summary>
        public bool ShowPushSettings => DeviceInfo.Plattform == EnumPlattform.XamarinIos || DeviceInfo.Plattform == EnumPlattform.XamarinAndroid;

        #endregion


        /// <summary>
        ///     Ereignis für Theme umschalten
        /// </summary>
        public event EventHandler<EventArg<bool>>? SwitchTheme;

        /// <summary>
        ///     Methode von Ereignis für Theme umschalten
        /// </summary>
        /// <param name="eventData"></param>
        protected virtual void OnSwitchTheme(EventArg<bool> eventData)
        {
            var handler = SwitchTheme;
            handler?.Invoke(this, eventData);
        }

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdSettingsPush = new VmCommandSelectable("", async () => { await Nav.ToViewWithResult(typeof(VmSettingsPush)).ConfigureAwait(true); });
        }

        /// <summary>
        /// Theme Picker selektiertes Item aenderung
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event parameter</param>
        private void ThemePickerOnSelectedItemChanged(object sender, SelectedItemEventArgs<VmPickerElement<EnumTheme>> e)
        {
            Dc.DcExLocalAppData.Data.UseDarkTheme = e.CurrentItem.Key == EnumTheme.Dark;
            Dc.DcExLocalAppData.StoreData(true);
            OnSwitchTheme(new EventArg<bool>(Dc.DcExLocalAppData.Data.UseDarkTheme));
        }
    }
}