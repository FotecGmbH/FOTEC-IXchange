// (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 16.10.2021 15:28
// Entwickler      Michael Kollegger
// Projekt         IXchange

using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using AndroidApp.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Picker), typeof(BissPickerRenderer))]

namespace AndroidApp.CustomRenderer
{
    /// <summary>
    ///     <para>BissEntryRenderer</para>
    ///     Klasse BissEntryRenderer. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class BissPickerRenderer : PickerRenderer
    {
        /// <summary>
        /// kontext
        /// </summary>
        readonly Context _context;

        /// <summary>
        /// Konstruktor Biss editor renderer
        /// </summary>
        /// <param name="context">kontext</param>
        public BissPickerRenderer(Context context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Element hat sich veraendert
        /// </summary>
        /// <param name="e">event argumente</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                var view = Element;


                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    Control.BackgroundTintList = ColorStateList.ValueOf(view.TextColor.ToAndroid());
                }
                else
#pragma warning disable CS0618 // Type or member is obsolete
                {
                    Control.Background.SetColorFilter(view.TextColor.ToAndroid(), PorterDuff.Mode.SrcAtop);
                }
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }
    }
}