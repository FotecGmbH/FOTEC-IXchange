// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Biss.Dc.Core;
using Database.Tables;
using Exchange.Enum;

// ReSharper disable once CheckNamespace
namespace IXchangeDatabase.Tables
{
    /// <summary>
    /// <para>Benachrichtigung</para>
    /// Klasse TableNotification. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class TableNotification : IDcChangeTracking
    {
        #region Properties

        /// <summary>
        ///     DB Id
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Beschreibung z.B. "sensor bewertet"
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Zeitpunkt der Einnahme/Ausgabe
        /// </summary>
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        ///     Abonierter Messwert
        /// </summary>
        public long TblMeasurementDefinitionAssignmentId { get; set; }

        /// <summary>
        /// Welche art an notifizierung z.b. unterschreitung eines Wertes/Bewertung
        /// </summary>
        public EnumNotificationType NotificationType { get; set; }

        /// <summary>
        ///     Abonierter Messwert
        /// </summary>
        [ForeignKey(nameof(TblMeasurementDefinitionAssignmentId))]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public virtual TableMeasurementDefinitionAssignment TblMeasurementDefinitionAssignment { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        ///     User, welchen diese Benachrichtigung betrifft
        /// </summary>
        public long TblUserId { get; set; }

        /// <summary>
        ///     User, welchen diese Benachrichtigung betrifft (Frage notwendig?=>Problem ansonsten schwer darzustellen welche
        /// Trigger welcher benutzer wann hatte bzw welche benachrichtigungen ihm angezeigt werden sollen)
        /// </summary>
        [ForeignKey(nameof(TblUserId))]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public virtual TableUser TblUser { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Version der Zeile. Wird automatisch durch den SQL Server aktualisiert
        /// https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/handling-concurrency-with-the-entity-framework-in-an-asp-net-mvc-application#add-an-optimistic-concurrency-property-to-the-department-entity
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CA1819 // Properties should not return arrays
        public byte[] DataVersion { get; set; } = Array.Empty<byte>();
#pragma warning restore CA1819 // Properties should not return arrays
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// Archiviert
        /// </summary>
        public bool IsArchived { get; set; }

        #endregion
    }
}