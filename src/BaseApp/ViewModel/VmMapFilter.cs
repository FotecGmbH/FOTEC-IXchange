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
using System.Threading.Tasks;
using Biss.Apps.Attributes;
using Biss.Apps.Interfaces;
using Exchange.Model;

namespace BaseApp.ViewModel
{
    /// <summary>
    /// <para>Karten Filter Vm</para>
    /// Klasse VmMapFilter. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewMapFilter")]
    public class VmMapFilter : VmProjectBase
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmMapFilter.DesignInstance}"
        /// </summary>
        public static VmMapFilter DesignInstance = new VmMapFilter();

        /// <summary>
        ///     VmMapFilter
        /// </summary>
        public VmMapFilter() : base("Kartenfilter")
        {
            SetViewProperties();
            View.ShowMenu = false;
            View.ShowBack = true;
        }

        #region Properties

        /// <summary>
        /// Filter-Werte
        /// </summary>
        public IEnumerable<ExMapFilter> FilterValues { get; set; } = new List<ExMapFilter>();

        #endregion


        #region Overrides

        /// <summary>
        ///     OnActivated (2) für View geladen noch nicht sichtbar
        ///     Nur einmal
        /// </summary>
        public override Task OnActivated(object? args = null)
        {
            if (!(args is IEnumerable<ExMapFilter> filters))
            {
                throw new ArgumentException("Falsches Argument");
            }

            FilterValues = filters;

            return base.OnActivated(args);
        }

        /// <summary>
        ///     OnDisappearing (4) wenn View unsichtbar / beendet wird
        ///     Nur einmal
        /// </summary>
        public override Task OnDisappearing(IView view)
        {
            ViewResult = FilterValues;
            return base.OnDisappearing(view);
        }

        #endregion
    }
}