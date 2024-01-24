// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:56
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.ComponentModel;
using Biss.Interfaces;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>Allgemeine Meldung</para>
    /// Klasse EcDcCommonMessage. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class EcDcCommonMessage : IBissModel
    {
        #region Properties

        /// <summary>
        ///     Meldung
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        ///     Titel
        /// </summary>
        public string Title { get; set; } = string.Empty;

        #endregion

        #region Interface Implementations

#pragma warning disable CS0067
#pragma warning disable CS0414
        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged = null!;
#pragma warning restore CS0067
#pragma warning restore CS0414

        #endregion
    }
}