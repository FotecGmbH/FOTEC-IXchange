// (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 27.09.2022 11:44
// Entwickler      Kevin Janisch
// Projekt         IXchange

using System;
using System.Globalization;
using System.Linq;
using BaseApp.ViewModel.Infrastructure;
using Xamarin.Forms;
using ViewState = BaseApp.ViewModel.Infrastructure.ViewState;


namespace BaseApp.View.Xamarin.Converter
{
    /// <summary>
    /// <para>ConverterViewstateEditIotDevice</para>
    /// Klasse ConverterViewstateEditIotDevice. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ConverterViewstateEditIotDevice : IValueConverter
    {
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type to which to convert the value.</param>
        /// <param name="parameter">A parameter to use during the conversion.</param>
        /// <param name="culture">The culture to use during the conversion.</param>
        /// <summary>Implement this method to convert <paramref name="value" /> to <paramref name="targetType" /> by using <paramref name="parameter" /> and <paramref name="culture" />.</summary>
        /// <returns>To be added.</returns>
        /// <remarks>To be added.</remarks>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ViewState state && parameter is ViewElement element)
            {
                switch (state)
                {
                    case ViewState.Default:
                       return new [] { ViewElement.MeasurementInterval, ViewElement.TransmissionInterval, ViewElement.Upstream, ViewElement.TransmissionType }.Contains(element);

                    case ViewState.Prebuilt:
                       return new [] { ViewElement.Upstream, ViewElement.ConverterType }.Contains(element);

                    case ViewState.PrebuiltCustomcode:
                        return new [] { ViewElement.Upstream, ViewElement.CodeArea, ViewElement.ConverterType }.Contains(element);

                    case ViewState.OpenSense:
                       return new [] { ViewElement.OpensenseBoxId, ViewElement.OpensenseHistoricalData }.Contains(element);

                    default:
                        return true;
                }
            }

            return false;
        }

        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type to which to convert the value.</param>
        /// <param name="parameter">A parameter to use during the conversion.</param>
        /// <param name="culture">The culture to use during the conversion.</param>
        /// <summary>Implement this method to convert <paramref name="value" /> back from <paramref name="targetType" /> by using <paramref name="parameter" /> and <paramref name="culture" />.</summary>
        /// <returns>To be added.</returns>
        /// <remarks>To be added.</remarks>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}