// (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 16.10.2021 15:28
// Entwickler      Michael Kollegger
// Projekt         IXchange

using System;
using Xamarin.Forms;

namespace BaseApp.View.Xamarin.View
{
    /// <summary>
    ///     ViewPush
    /// </summary>
    public partial class ViewSettingPush
    {
        /// <summary>
        ///     ViewPush
        /// </summary>
        public ViewSettingPush()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     ViewPush
        /// </summary>
        /// <param name="args"></param>
        public ViewSettingPush(object args = null!) : base(args)
        {
            InitializeComponent();
        }

        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            await SwitchPushEnabled.TranslateTo(-5, 0, 100).ConfigureAwait(true);
            await SwitchPushEnabled.TranslateTo(15, 0, 100).ConfigureAwait(true);
            await SwitchPushEnabled.TranslateTo(0, 0, 100).ConfigureAwait(true);
        }
    }
}