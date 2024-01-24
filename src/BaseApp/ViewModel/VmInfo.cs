// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Linq;
using System.Text;
using BDA.Common.Exchange.GatewayService;
using Biss.Apps.Attributes;
using Biss.Apps.ViewModel;
using Biss.Serialize;
using Exchange;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    ///     <para>View Infos</para>
    /// Klasse VmInfo. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewInfo")]
    public class VmInfo : VmProjectBase
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmInfo.DesignInstance}"
        /// </summary>
        public static VmInfo DesignInstance = new VmInfo();

        /// <summary>
        ///     VmInfo
        /// </summary>
        public VmInfo() : base("Inbetriebnahme", subTitle: "Inbetriebnahme der Sensoren und des Gateways")
        {
            SetViewProperties();
        }

        #region Properties

        /// <summary>
        ///     Gateway Applikation herunterladen Command
        /// </summary>
        public VmCommand CmdDownloadGateway { get; set; } = null!;

        /// <summary>
        ///     GatewayConfig herunterladen
        /// </summary>
        public VmCommand CmdDownloadGatewayConfig { get; set; } = null!;

        /// <summary>
        ///     TCP Applikation herunterladen
        /// </summary>
        public VmCommand CmdDownloadTcpApplication { get; set; } = null!;

        /// <summary>
        ///     App Settings
        /// </summary>
        public AppSettings CurrentSettings => AppSettings.Current();

        #endregion

        /// <summary>
        ///     Generate Config Data.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public byte[] GenerateData()
        {
            var user = Dc.DcExUser.Data;
            if (user == null)
            {
                throw new Exception($"[{nameof(VmInfo)}][{nameof(GenerateData)}]No User in DcExUser");
            }

            var name = user.LoginName;


            var gateways = Dc.DcExGateways.Select(g => g.Data).ToList();

            var gateway = gateways.FirstOrDefault(g => g.Information.Name == name);

            if (gateway == null)
            {
                throw new Exception($"[{nameof(VmInfo)}][{nameof(GenerateData)}] Gateway not found");
            }

            var gwConfig = new ExGwServiceGatewayConfig
                           {
                               DbId = gateway.Id,
                               Description = gateway.Information.Description,
                               ConfigVersion = -1,
                               FirmwareVersion = gateway.DeviceCommon.FirmwareversionDevice,
                               Name = gateway.Information.Name,
                               Position = gateway.Location,
                               Secret = gateway.DeviceCommon.Secret,
                               IotDevices = null!
                           };

            var gwConfigJson = gwConfig.ToJson();

            var gwConfigByteArray = Encoding.UTF8.GetBytes(gwConfigJson);

            return gwConfigByteArray;
        }

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdDownloadGateway = new VmCommand(ResViewInfo.CmdDownloadGateway, async () => { await Open.Browser("https://ixchangeblobdev.blob.core.windows.net/ixchangeblob/GatewayAppWindows.zip").ConfigureAwait(true); });


            CmdDownloadGatewayConfig = new VmCommand(ResViewInfo.CmdDownloadGatewayConfig, async x =>
            {
                var bytes = GenerateData();
                var path = await Files.GetLocalFileNameForSave("gwConfig.json", true).ConfigureAwait(true);
                Files.WriteAllBytes(path, bytes);
            });

            CmdDownloadTcpApplication = new VmCommand(ResViewInfo.CmdDownloadTcpApplication, async () => { await Open.Browser("https://ixchangeblobdev.blob.core.windows.net/ixchangeblob/TcpSensorApplication.zip").ConfigureAwait(true); });

            base.InitializeCommands();
        }
    }
}