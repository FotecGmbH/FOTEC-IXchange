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
using BDA.Common.Exchange.Model;
using Biss.Interfaces;

namespace Exchange.Model.ConfigApp
{
    /// <summary>
    /// <para>Rating</para>
    /// Klasse ExRating. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExRating : IBissModel
    {
        #region Properties

        /// <summary>
        ///     ID aus DB
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Rating 0-5 Sterne
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// z.B. "Tolle Werte"
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// User
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ExUser User { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// Sensor
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ExMeasurementDefinitionAssignment MeasurementDefinitionAssignment { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// Zeitpunkt der Rezension
        /// </summary>
        public DateTime TimeStamp { get; set; }

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