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
using Biss.Interfaces;
using Exchange.Enum;

namespace Exchange.Model
{
    /// <summary>
    /// <para>Karten Filter.</para>
    /// Klasse ExMapFilter. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExMapFilter : IBissModel
    {
        #region Properties

        /// <summary>
        /// Mess-Typ.
        /// </summary>
        public EnumMeasurementType MeasurementType { get; set; }

        /// <summary>
        ///     Angezeigt.
        /// </summary>
        public bool Shown { get; set; }

        #endregion

        #region Interface Implementations

#pragma warning disable CS0067
#pragma warning disable CS0414
        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
#pragma warning restore CS0414

        #endregion
    }
}