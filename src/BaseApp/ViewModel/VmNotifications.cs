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
using Biss.Apps.Attributes;
using Biss.Apps.ViewModel;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    /// <para>Viewmodel für ViewNotifications</para>
    /// Klasse VmNotifications. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewNotifications", true)]
    public class VmNotifications : VmProjectBase
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmNotifications.DesignInstance}"
        /// </summary>
        public static VmNotifications DesignInstance = new VmNotifications();

        /// <summary>
        ///     VmNotifications
        /// </summary>
        public VmNotifications() : base(ResViewNotifications.LblPageTitle)
        {
            SetViewProperties();
        }

        #region Overrides

        /// <summary>
        ///     OnLoaded (3) für View geladen
        ///     Jedes Mal wenn View wieder sichtbar
        /// </summary>
        public override async Task OnLoaded()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            await Dc.DcExNotifications.WaitDataFromServerAsync().ConfigureAwait(true);
#pragma warning restore CS0618 // Type or member is obsolete
            await base.OnLoaded().ConfigureAwait(true);
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            CmdOpenDetailView = new VmCommand(string.Empty, async arg =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                // ReSharper disable once UnusedVariable
                if (arg is DcListTypeNotification notification)
                {
                    //Todo Detailansicht öffnen (siehe Mockup)
                }
            });
        }

        /// <summary>
        ///     Command um Detailansicht zu öffnen
        /// </summary>
        public VmCommand CmdOpenDetailView { get; set; } = null!;

        #endregion
    }
}