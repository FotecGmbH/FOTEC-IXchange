// (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 09.10.2023 15:03
// Entwickler      Roman Jahn (RJa)
// Projekt         IXchange

using System;
using System.Globalization;
using Exchange.Extensions;
using Xamarin.Forms;

namespace BaseApp.View.Xamarin.Converter
{
    /// <summary>
    /// <para>Converter für Enums zu Display Name</para>
    /// Klasse ConverterEnumDisplayName. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ConverterEnumDisplayName : IValueConverter
    {
        /// <summary>
        /// Konvertiert ein Objekt für XAML
        /// </summary>
        /// <param name="value">Wert zum konvertieren für das UI</param>
        /// <param name="targetType">Zieltyp des Werts</param>
        /// <param name="parameter">Zusätzlicher Parameter aus XAML</param>
        /// <param name="culture">Aktuelle Kultur</param>
        /// <returns>Konvertierter Wert oder null</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum e)
            {
                if (parameter != null!)
                {
                    if (parameter.ToString().ToLower().Equals("description", StringComparison.InvariantCulture))
                    {
                        return e.GetDisplayDescription();
                    }

                    if (parameter.ToString().ToLower().Equals("shortname", StringComparison.InvariantCulture))
                    {
                        return e.GetDisplayShortName();
                    }
                }

                return e.GetDisplayName();
            }
            return null!;
        }

        /// <summary>
        /// Konvertiert ein Objekt von XAML
        /// </summary>
        /// <param name="value">Wert zum konvertieren für das Datenobjekt</param>
        /// <param name="targetType">Zieltyp des Werts</param>
        /// <param name="parameter">Zusätzlicher Parameter aus XAML</param>
        /// <param name="culture">Aktuelle Kultur</param>
        /// <returns>Konvertierter Wert oder UnsetValue</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null!;
        }
    }
}