﻿// (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 27.08.2021 11:09
// Entwickler      Michael Kollegger
// Projekt         Biss.Template.BissAppsNuget

using System;
using System.Windows.Controls;
using WpfApp.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(Entry), typeof(BissWpfEntryRenderer))]

namespace WpfApp.CustomRenderer
{
    /// <summary>
    ///     <para>BissWpfSwitchRenderer</para>
    ///     Klasse BissWpfSwitchRenderer. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class BissWpfEntryRenderer : EntryRenderer
    {
        /// <summary>
        ///     OnElementChanged
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (e != null! && e.NewElement != null)
            {
                Control.Resources.Add(Border.CornerRadiusProperty, 12);
            }
        }
    }
}