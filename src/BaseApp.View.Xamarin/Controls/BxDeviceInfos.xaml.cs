// (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 02.02.2022 14:20
// Entwickler      Michael Kollegger
// Projekt         IXchange

using System;
using BDA.Common.Exchange.Model.ConfigApp;
using Exchange.Model.ConfigApp;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BaseApp.View.Xamarin.Controls
{
    /// <summary>
    ///     Control für allgemeine Infos eines Iot Device und Gateways
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BxDeviceInfos
    {
        /// <summary>
        ///     BxExCommonInfoProperty Bindable Property
        /// </summary>
        public static BindableProperty ExCommonInfoProperty = BindableProperty.Create(
            nameof(ExCommonInfo),
            typeof(ExCommonInfo),
            typeof(BxDeviceInfos),
            new ExCommonInfo()
        );

     

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        public BxDeviceInfos()
        {
            InitializeComponent();
        }

        #region Properties

        /// <summary>
        ///     ExCommonInfo
        /// </summary>
        public ExCommonInfo ExCommonInfo
        {
            get => (ExCommonInfo) GetValue(ExCommonInfoProperty);
            set => SetValue(ExCommonInfoProperty, value);
        }

        #endregion
    }
}