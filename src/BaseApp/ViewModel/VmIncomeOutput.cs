// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseApp.Connectivity;
using Biss.Apps.Attributes;
using Biss.Apps.Interfaces;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    /// <para>Viewmodel für Einnahmen/Ausgaben</para>
    /// Klasse VmIncomeOutput. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewIncomeOutput")]
    public class VmIncomeOutput : VmProjectBase
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmIncomeOutput.DesignInstance}"
        /// </summary>
        public static VmIncomeOutput DesignInstance = new VmIncomeOutput();

        /// <summary>
        ///     VmIncomeOutput
        /// </summary>
        public VmIncomeOutput() : base(ResViewIncomeOutput.LblPageTitle)
        {
            SetViewProperties();
        }

        #region Properties

        /// <summary>
        /// Letzte Einnahme/Ausgabe
        /// </summary>
        public DcListTypeIncomeOutput? LastIncomeOutput { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> HowToCollectIxiesItems { get; } = new Dictionary<string, string>
                                                                            {
                                                                                {ResViewIncomeOutput.HowToCollectIxiesItem1Key, ResViewIncomeOutput.HowToCollectIxiesItem1Value},
                                                                                {ResViewIncomeOutput.HowToCollectIxiesItem2Key, ResViewIncomeOutput.HowToCollectIxiesItem2Value},
                                                                                {ResViewIncomeOutput.HowToCollectIxiesItem3Key, ResViewIncomeOutput.HowToCollectIxiesItem3Value},
                                                                                {ResViewIncomeOutput.HowToCollectIxiesItem4Key, ResViewIncomeOutput.HowToCollectIxiesItem4Value}
                                                                            };

        /// <summary>
        /// Wie Ixies erhalten werden koennen
        /// </summary>
        public Dictionary<string, string> HowToSpendIxiesItems { get; } = new Dictionary<string, string>
                                                                          {
                                                                              {ResViewIncomeOutput.HowToSpendIxiesItem1Key, ResViewIncomeOutput.HowToSpendIxiesItem1Value}
                                                                          };

        /// <summary>
        /// Glyph für Info
        /// </summary>
        public string InfoGlyph { get; set; } = Glyphs.Alarm_bell;

        #endregion

        /// <summary>
        /// RefreshLastIncomeOutput
        /// </summary>
        public void RefreshLastIncomeOutput()
        {
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            LastIncomeOutput = Dc.DcExIncomeOutput?.OrderByDescending(x => x.Data.TimeStamp)?.FirstOrDefault();
        }

        #region Commands

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
        }

        #endregion

        #region Overrides

        /// <summary>
        ///     OnAppearing (1) für View geladen noch nicht sichtbar
        ///     Wird Mal wenn View wieder sichtbar ausgeführt
        ///     Unbedingt beim überschreiben auch base. aufrufen!
        /// </summary>
        public override async Task OnAppearing(IView view)
        {
            await Dc.DcExIncomeOutput.Sync().ConfigureAwait(true);
            RefreshLastIncomeOutput();
            await base.OnAppearing(view).ConfigureAwait(true);
        }

        /// <summary>
        ///     OnActivated (2) für View geladen noch nicht sichtbar
        ///     Nur einmal
        /// </summary>
        public override async Task OnActivated(object? args = null)
        {
            await Dc.DcExIncomeOutput.Sync().ConfigureAwait(true);
            RefreshLastIncomeOutput();
            await base.OnActivated(args).ConfigureAwait(true);
            await View.RefreshAsync().ConfigureAwait(true);
        }

        #endregion
    }
}