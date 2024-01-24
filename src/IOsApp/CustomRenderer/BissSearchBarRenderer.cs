// (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 16.10.2021 15:28
// Entwickler      Michael Kollegger
// Projekt         IXchange

using System;
using IOsApp.CustomRenderer;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


//https://www.c-sharpcorner.com/article/xamarin-forms-custom-entry/
[assembly: ExportRenderer(typeof(SearchBar), typeof(BissSearchBarRenderer))]

namespace IOsApp.CustomRenderer
{
    /// <summary>
    ///     <para>BissEntryRenderer</para>
    ///     Klasse BissEntryRenderer. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class BissSearchBarRenderer : SearchBarRenderer
    {
        /// <summary>
        /// Wenn sich ein element veraendert hat
        /// </summary>
        /// <param name="e">event argumente</param>
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var view = Element;

                Control.KeyboardAppearance = UIKeyboardAppearance.Dark;
                Control.ReturnKeyType = UIReturnKeyType.Done;
                // Radius for the curves  
                Control.Layer.CornerRadius = Convert.ToSingle(8);
                // Thickness of the Border Color  
                Control.Layer.BorderColor = view.BackgroundColor.ToCGColor();
                // Thickness of the Border Width  
                Control.Layer.BorderWidth = 1;
                Control.ClipsToBounds = true;
            }
        }
    }
}