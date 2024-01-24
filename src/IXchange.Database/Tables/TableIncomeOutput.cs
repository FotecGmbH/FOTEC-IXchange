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
    /// <para>Income/Output für einen User z.B. "Benutzer X erhält Y IXies weil Z" / "Benutzer X hat Y IXies ausgegeben weil Z"</para>
    /// Klasse TableIncomeOutput. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class TableIncomeOutput : IDcChangeTracking
    {
        #region Properties

        /// <summary>
        ///     Device ID für DB
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Wie viele IXies war diese Einnahme/Ausgabe
        /// </summary>
        public int Ixies { get; set; }

        /// <summary>
        /// "Kontostand" nach dieser Buchung
        /// </summary>
        public int CurrentTotalIxies { get; set; }


        /// <summary>
        /// Zeitpunkt der Einnahme/Ausgabe
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        ///     User des Abos
        /// </summary>
        public long TblUserId { get; set; }

        /// <summary>
        /// MeasurementDefinition id
        /// </summary>
        public long? TblMeasurementDefinitonId { get; set; }

        /// <summary>
        /// Option/grund
        /// </summary>
        public EnumIncomeOutputOption Option { get; set; }


        /// <summary>
        ///     User des Abos
        /// </summary>
        [ForeignKey(nameof(TblUserId))]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public virtual TableUser TblUser { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// MeasurementDefinition
        /// </summary>
        [ForeignKey(nameof(TblMeasurementDefinitonId))]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public virtual TableMeasurementDefinition? TblMeasurementDefinition { get; set; }
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