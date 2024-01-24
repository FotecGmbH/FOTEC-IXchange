// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System.Threading.Tasks;
using BaseApp.ViewModel;
using BaseApp.ViewModel.Infrastructure;
using Biss.Apps.ViewModel;
using Exchange.Resources;

namespace BaseApp
{
    /// <summary>
    ///     <para>Commands für alle Views und das Menü</para>
    /// Klasse VmProjectBaseCommands. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public abstract partial class VmProjectBase
    {
        /// <summary>
        /// Selektierbarer Command "Mehr"
        /// </summary>
        private static VmCommandSelectable _gcmdMore = null!;

        /// <summary>
        /// Selektierbarer Command "Header"
        /// </summary>
        private static VmCommandSelectable _gcmdHeader = null!;

        /// <summary>
        /// Selektierbarer Command "Home"
        /// </summary>
        private static VmCommandSelectable _gcmdHome = null!;

        /// <summary>
        /// Selektierbarer Command "Login"
        /// </summary>
        private static VmCommandSelectable _gcmdLogin = null!;

        /// <summary>
        /// Selektierbarer Command "Einstellungen"
        /// </summary>
        private static VmCommandSelectable _gcmdSettings = null!;

        /// <summary>
        /// Selektierbarer Command "Infrastruktur"
        /// </summary>
        private static VmCommandSelectable _gcmdInfrastructure = null!;

        /// <summary>
        /// Selektierbarer Command "Uebersicht"
        /// </summary>
        private static VmCommandSelectable _gcmdIotDevicesOverview = null!;

        /// <summary>
        /// Selektierbarer Command "Kommentare"
        /// </summary>
        private static VmCommandSelectable _gcmdMyRatings = null!;

        /// <summary>
        /// Selektierbarer Command "Einnahmen/Ausgaben"
        /// </summary>
        private static VmCommandSelectable _gcmdIncomeOutput = null!;

        /// <summary>
        /// Selektierbarer Command "Benachrichtigungen"
        /// </summary>
        private static VmCommandSelectable _gcmdNotifications = null!;

        /// <summary>
        /// Selektierbarer Command "Abo Uebersicht"
        /// </summary>
        private static VmCommandSelectable _gcmdAboOverview = null!;

        /// <summary>
        /// Selektierbarer Command "Entwicklerinfomationen"
        /// </summary>
        private static VmCommandSelectable _gcmdDeveloperInfos = null!;

        /// <summary>
        /// Selektierbarer Command "Informationen"
        /// </summary>
        private static VmCommandSelectable _gcmdInfos = null!;

        /// <summary>
        ///     Projektbeogene, globale VmCommands(Selectable) initialisieren
        /// </summary>
        protected override bool InitializeProjectBaseCommands()
        {
            _gcmdMore = new VmCommandSelectable(ResCommon.CmdMore, () =>
            {
                GCmdShowMenu.Execute(null!);
                _ = Task.Run(async () =>
                {
                    await Task.Delay(250).ConfigureAwait(false);
                    _gcmdMore.IsSelected = false;
                }).ConfigureAwait(false);
            }, ResCommon.CmdMoreToolTip, Glyphs.Navigation_menu_horizontal);

            _gcmdHeader = new VmCommandSelectable(string.Empty, async () => { await MsgBox.Show(ResCommon.MsgHeaderInfos, ResCommon.MsgTitleHeaderInfos).ConfigureAwait(true); });

            _gcmdHome = new VmCommandSelectable(ResCommon.CmdHome, () => { Nav.ToView(typeof(VmMain), showMenu: true, cachePage: true); }, glyph: Glyphs.House_chimney_2);

            _gcmdLogin = new VmCommandSelectable(ResCommon.CmdLogin, () => { Nav.ToView(typeof(VmLogin), showMenu: true, cachePage: false); }, glyph: Glyphs.Monitor_upload);

            VmViewProperties.SetGcmdUserCommand(new VmCommandSelectable(ResCommon.CmdUser, () =>
            {
                if (!Dc.DeviceAndUserRegisteredLocal)
                {
                    _gcmdLogin.Execute(null!);
                }
                else
                {
                    Nav.ToView(typeof(VmUser), showMenu: true, cachePage: false);
                }
            }, glyph: Glyphs.Single_man));

            _gcmdSettings = new VmCommandSelectable(ResCommon.CmdSettings, () => { Nav.ToView(typeof(VmSettings), showMenu: true, cachePage: true); }, glyph: Glyphs.Cog);

            _gcmdInfrastructure = new VmCommandSelectable("Infrastruktur",
                () => { Nav.ToView(typeof(VmInfrastructure), showMenu: true, cachePage: true); },
                glyph: Glyphs.Hierarchy_9);

            _gcmdIotDevicesOverview = new VmCommandSelectable(ResViewIotDevicesOverview.LblPageTitle,
                () => { Nav.ToView(typeof(VmIotDevicesOverview), showMenu: true, cachePage: true); },
                glyph: Glyphs.Smart_house_phone_connect);

            _gcmdMyRatings = new VmCommandSelectable(ResViewMyRatings.LblRatings,
                () => { Nav.ToView(typeof(VmMyRatings), showMenu: true, cachePage: true); },
                glyph: Glyphs.Messages_bubble);

            _gcmdIncomeOutput = new VmCommandSelectable(ResViewIncomeOutput.LblPageTitle,
                () => { Nav.ToView(typeof(VmIncomeOutput), showMenu: true, cachePage: true); },
                glyph: Glyphs.Diamond);

            _gcmdNotifications = new VmCommandSelectable(ResViewNotifications.LblPageTitle,
                () => { Nav.ToView(typeof(VmNotifications), showMenu: true, cachePage: true); },
                glyph: Glyphs.Alarm_bell);

            _gcmdAboOverview = new VmCommandSelectable(ResViewAboOverview.LblPageTitle,
                () => { Nav.ToView(typeof(VmAboOverview), showMenu: true, cachePage: true); },
                glyph: Glyphs.Alarm_bell);

            _gcmdDeveloperInfos = new VmCommandSelectable("DEV Infos", () => { Nav.ToView(typeof(VmDeveloperInfos), showMenu: true, cachePage: true); }, glyph: Glyphs.Computer_bug);

            _gcmdInfos = new VmCommandSelectable(ResCommon.CmdInfo, () => { Nav.ToView(typeof(VmInfo), showMenu: true, cachePage: true); }, glyph: Glyphs.Information_circle);

            return true;
        }

#pragma warning disable 1591
        /// <summary>
        /// Selektierbarer Command "Header"
        /// </summary>
        public VmCommandSelectable GCmdHeader => _gcmdHeader;

        /// <summary>
        /// Selektierbarer Command "Mehr"
        /// </summary>
        public VmCommandSelectable GCmdMore => _gcmdMore;

        /// <summary>
        /// Selektierbarer Command "Home"
        /// </summary>
        public VmCommandSelectable GCmdHome => _gcmdHome;

        /// <summary>
        /// Selektierbarer Command "Login"
        /// </summary>
        public VmCommandSelectable GCmdLogin => _gcmdLogin;

        /// <summary>
        /// Selektierbarer Command "Einstellungen"
        /// </summary>
        public VmCommandSelectable GCmdSettings => _gcmdSettings;


        /// <summary>
        /// Selektierbarer Command "Infrastruktur"
        /// </summary>
        public VmCommandSelectable GCmdInfrastructure => _gcmdInfrastructure;

        /// <summary>
        /// Selektierbarer Command "IotDevice Uebersicht"
        /// </summary>
        public VmCommandSelectable GCmdIotDevicesOverview => _gcmdIotDevicesOverview;

        /// <summary>
        /// Selektierbarer Command "Kommentare"
        /// </summary>
        public VmCommandSelectable GCmdMyRatings => _gcmdMyRatings;

        /// <summary>
        /// Selektierbarer Command "Einnahmen/Ausgaben"
        /// </summary>
        public VmCommandSelectable GCmdIncomeOutput => _gcmdIncomeOutput;

        /// <summary>
        /// Selektierbarer Command "Benachrichtigungen"
        /// </summary>
        public VmCommandSelectable GCmdNotifications => _gcmdNotifications;

        /// <summary>
        /// Selektierbarer Command "Abo Uebersicht"
        /// </summary>
        public VmCommandSelectable GCmdAboOverview => _gcmdAboOverview;

        /// <summary>
        /// Selektierbarer Command "Entwicklerinfomationen"
        /// </summary>
        public VmCommandSelectable GCmdDeveloperInfos => _gcmdDeveloperInfos;

        /// <summary>
        /// Selektierbarer Command "Informationen"
        /// </summary>
        public VmCommandSelectable GCmdInfos => _gcmdInfos;
#pragma warning restore 1591
    }
}