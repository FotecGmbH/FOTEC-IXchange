// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Threading.Tasks;
using BaseApp.Connectivity;
using BDA.Common.Exchange.Model.ConfigApp;

namespace BaseApp.ViewModel.ModalVM
{
    /// <summary>
    /// <para>Information ueber IoTDevice</para>
    /// Klasse VmIotDeviceInfo. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class VmIotDeviceInfo : VmProjectBase
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmIotDeviceInfo.DesignInstance}"
        /// </summary>
        public static VmIotDeviceInfo DesignInstance = new VmIotDeviceInfo();

        /// <summary>
        ///     VmIotDeviceInfo
        /// </summary>
        public VmIotDeviceInfo() : base("Device Info.")
        {
            SetViewProperties();
        }

        #region Properties

        /// <summary>
        /// Iot Device Data.
        /// </summary>
        public DcListTypeIotDevice DcListTypeIotDevice { get; set; } = null!;

        /// <summary>
        ///     Daten.
        /// </summary>
        public ExIotDevice? Data { get; set; }

        /// <summary>
        ///     Information.
        /// </summary>
        public ExInformation? Information { get; set; }

        /// <summary>
        ///     Kann abbonieren.
        /// </summary>
        public bool CanSubscribe { get; set; }

        #endregion

        #region Overrides

        /// <summary>
        ///     OnActivated (2) für View geladen noch nicht sichtbar
        ///     Nur einmal
        /// </summary>
        public override Task OnActivated(object? args = null)
        {
            if (!(args is DcListTypeIotDevice device))
            {
                throw new ArgumentException("Falsches Argument");
            }

            DcListTypeIotDevice = device;
            Data = DcListTypeIotDevice.Data;
            Information = Data.Information;
            CanSubscribe = !Dc.DcExUser.Data.HasPermissionTo(Data);
            return base.OnActivated(args);
        }

        #endregion
    }
}