// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.ComponentModel;
using System.Globalization;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Interfaces;

namespace Exchange.Model.ConfigApp
{
    /// <summary>
    /// <para>ExMeasurement with double value</para>
    /// Klasse ExMeasurementWithDoubleValue. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExMeasurementWithDoubleValue : ExMeasurement, IBissModel
    {
        #region Properties

        /// <summary>
        /// Value als Zahl (falls möglich, sonst 0)
        /// </summary>
        public double ValueAsNumber => double.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var num) ? num : 0;

        #endregion

        #region Interface Implementations

#pragma warning disable CS0067
#pragma warning disable CS0414
        /// <inheritdoc />
#pragma warning disable CS0108, CS0114
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0108, CS0114
#pragma warning restore CS0067
#pragma warning restore CS0414

        #endregion
    }
}