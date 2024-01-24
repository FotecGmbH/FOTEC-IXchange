// (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 09.10.2023 10:22
// Entwickler      Roman Jahn (RJa)
// Projekt         IXchange

using System;
using System.Globalization;
using BaseApp.Connectivity;
using Exchange.Model.ConfigApp;
using Xamarin.Forms;

namespace BaseApp.View.Xamarin.Converter
{
    /// <summary>
    /// <para>Converter für Einnahme/Ausgabe zu Farbe</para>
    /// Klasse ConverterIncomeOutputColor. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ConverterIncomeOutputColor : IValueConverter
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
            var posColor = Color.Green;
            var negColor = Color.Red;

            if (value is DcListTypeIncomeOutput listTypeIncomeOutput)
            {
                value = listTypeIncomeOutput.Data;
            }

            if (value is ExIncomeOutput incomeOutput)
            {
                value = incomeOutput.Ixies;
            }

            if (value is int intValue)
            {
                return intValue >= 0 ? posColor : negColor;
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