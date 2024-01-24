﻿// (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 16.10.2021 15:28
// Entwickler      Michael Kollegger
// Projekt         IXchange

using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Util;
using AndroidApp.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(BissEntryRenderer))]

namespace AndroidApp.CustomRenderer
{
    /// <summary>
    ///     <para>BissEntryRenderer</para>
    ///     Klasse BissEntryRenderer. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class BissEntryRenderer : EntryRenderer
    {
        /// <summary>
        /// kontext
        /// </summary>
        readonly Context _context;

        /// <summary>
        /// Konstruktor Biss entry renderer
        /// </summary>
        /// <param name="context"></param>
        public BissEntryRenderer(Context context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Dp zu pixel
        /// </summary>
        /// <param name="context">context fuer metriken</param>
        /// <param name="valueInDp">wert in dp</param>
        /// <returns>pixel</returns>
        public static float DpToPixels(Context context, float valueInDp)
        {
            var metrics = context.Resources.DisplayMetrics;
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, valueInDp, metrics);
        }

        /// <summary>
        /// Element hat sich veraendert
        /// </summary>
        /// <param name="e">event argumente</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            var view = Element;

            
            var gradientBackground = new GradientDrawable();
            //gradientBackground.SetStroke(1, view.BackgroundColor.ToAndroid());
            gradientBackground.SetStroke(1, view.TextColor.ToAndroid());
            gradientBackground.SetCornerRadius(DpToPixels(_context, Convert.ToSingle(8)));
            gradientBackground.SetShape(ShapeType.Rectangle);
            gradientBackground.SetColor(view.BackgroundColor.ToAndroid());
            Control.SetBackground(gradientBackground);
            SetBackgroundColor(Color.Transparent.ToAndroid());

            // Set padding for the internal text from border  
            Control.SetPadding(
                (int)DpToPixels(_context, Convert.ToSingle(12)), Control.PaddingTop,
                (int)DpToPixels(_context, Convert.ToSingle(12)), Control.PaddingBottom);
        }
    }
}