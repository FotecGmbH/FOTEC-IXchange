﻿// (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 30.04.2021 17:47
// Entwickler      Michael Ploy
// Projekt         Biss.Apps.Core

using Biss.Apps.Map.XF.Wpf;
using Exchange;

namespace WpfApp
{
    /// <summary>
    /// <para>MainWindow</para>
    /// Klasse MainWindow. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// MainWindow
        /// </summary>
        public MainWindow()
        {
#pragma warning disable CA1416 // Validate platform compatibility
            Param = new object[]
#pragma warning restore CA1416 // Validate platform compatibility
                    {
                        AppSettings.Current().ProjectWorkUserFolder,
                        AppSettings.Current(),
                    };

            InitializeComponent();

#pragma warning disable CA1416 // Validate platform compatibility
            _ = BissAppsFormsInit();
#pragma warning restore CA1416 // Validate platform compatibility
            _ = this.InitBissMap(AppSettings.Current());
#pragma warning disable CA1416 // Validate platform compatibility
            LoadApplication(new BaseApp.View.Xamarin.BissApp(Initializer));
#pragma warning restore CA1416 // Validate platform compatibility
        }
    }
}