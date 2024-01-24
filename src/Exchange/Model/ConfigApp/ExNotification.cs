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
using Exchange.Enum;

namespace Exchange.Model.ConfigApp
{
    /// <summary>
    /// <para>ExNotification</para>
    /// Klasse ExNotification. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExNotification : IBissModel
    {
        #region Properties

        /// <summary>
        ///     Id aus DB
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Beschreibung z.B. "sensor bewertet"
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Zeitstempel
        /// </summary>
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Welche art an notifizierung z.b. unterschreitung eines Wertes/Bewertung
        /// </summary>
        public EnumNotificationType NotificationType { get; set; }

        /// <summary>
        ///     Abonierter Messwert
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ExMeasurementDefinitionAssignment MeasurementDefinitionAssignment { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        ///     User, welchen diese Benachrichtigung betrifft (Frage notwendig?=>Problem ansonsten schwer darzustellen welche
        /// Trigger welcher benutzer wann hatte bzw welche benachrichtigungen ihm angezeigt werden sollen)
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public virtual ExUser User { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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