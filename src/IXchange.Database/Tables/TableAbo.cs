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

// ReSharper disable once CheckNamespace
namespace IXchangeDatabase.Tables
{
    /// <summary>
    /// <para>Tabelle von einem Abo eines Users zu einem Messwert</para>
    /// Klasse TableAbo. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class TableAbo : IDcChangeTracking
    {
        #region Properties

        /// <summary>
        ///     Device ID für DB
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Abweichung von Mittelwert trigger ja nein
        /// </summary>
        public bool MovingAverageNotify { get; set; }

        /// <summary>
        /// Abweichung von Mittelwert trigger(Wert=bei welchem unterschied von mittelwert soll benachrichtigung passieren?)
        /// </summary>
        public float MovingAverageNotifyValue { get; set; }

        /// <summary>
        /// Übersteigung wert trigger ja nein
        /// </summary>
        public bool ExceedNotify { get; set; }

        /// <summary>
        /// Überschreitung Wert trigger
        /// </summary>
        public float ExceedNotifyValue { get; set; }

        /// <summary>
        /// Unterschreitung Wert trigger ja nein
        /// </summary>
        public bool UndercutNotify { get; set; }

        /// <summary>
        /// Unterschreitung Wert trigger
        /// </summary>
        public float UndercutNotifyValue { get; set; }

        /// <summary>
        /// Ausfall Zeit in Minuten trigger ja nein
        /// </summary>
        public bool FailureForMinutesNotify { get; set; }

        /// <summary>
        /// Ausfall Zeit in Minuten trigger
        /// </summary>
        public float FailureForMinutesNotifyValue { get; set; }

        /// <summary>
        ///     User des Abos
        /// </summary>
        public long TblUserId { get; set; }

        /// <summary>
        ///     User des Abos
        /// </summary>
        [ForeignKey(nameof(TblUserId))]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public virtual TableUser TblUser { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        ///     Abonierter Messwert
        /// </summary>
        public long TblMeasurementDefinitionAssignmentId { get; set; }

        /// <summary>
        ///     Abonierter Messwert
        /// </summary>
        [ForeignKey(nameof(TblMeasurementDefinitionAssignmentId))]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public virtual TableMeasurementDefinitionAssignment TblMeasurementDefinitionAssignment { get; set; }

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