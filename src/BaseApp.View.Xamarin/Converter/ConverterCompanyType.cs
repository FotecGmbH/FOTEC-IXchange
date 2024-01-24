﻿// (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 18.01.2022 06:46
// Entwickler      Georg Wernitznig
// Projekt         IXchange

using System;
using System.Globalization;
using BDA.Common.Exchange.Enum;
using Exchange.Enum;
using Exchange.Resources;
using Xamarin.Forms;

namespace BaseApp.View.Xamarin.Converter
{
    /// <summary>
    /// <para>CompanyType converter</para>
    /// Klasse ConverterCompanyType. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ConverterCompanyType : IValueConverter
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
            if (!(value is EnumCompanyTypes companyType))
                return null!;

            var returnString = string.Empty;

            switch (companyType)
            {
                case EnumCompanyTypes.PublicCompany:
                    returnString = Glyphs.Lock_unlock_1;
                    break;
                case EnumCompanyTypes.Company:
                    returnString = Glyphs.Lock;
                    break;
                case EnumCompanyTypes.NoCompany:
                    break;
            }

            return returnString;
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