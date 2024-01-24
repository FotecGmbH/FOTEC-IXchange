// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Biss.Dc.Core;
using Database.Tables;
using Exchange.Enum;


// ReSharper disable once CheckNamespace
namespace IXchangeDatabase.Tables;

/// <summary>
///     <para>Definiton einer Messung</para>
/// Klasse TableMeasurementDefinition.cs (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
/// </summary>
[Table("MeasurementDefinitionAssignment")]
public class TableMeasurementDefinitionAssignment : IDcChangeTracking
{
    #region Properties

    /// <summary>
    ///     DB Id
    /// </summary>
    [Key]
    public long Id { get; set; }


    /// <summary>
    /// Typ, z.b. feuchtigkeit, temperatur etc etc
    /// </summary>
    public EnumMeasurementType Type { get; set; }

    /// <summary>
    ///     Zugehöriges Iot-Gerät
    /// </summary>
    public long TblMeasurementDefinitionId { get; set; }

    /// <summary>
    /// Benachrichtigungen senden?
    /// </summary>
    public bool SendNotifications { get; set; }

    /// <summary>
    /// Benachrichtigung bei neuem Rating?
    /// </summary>
    public bool NotificationOnNewRating { get; set; }

    /// <summary>
    /// Benachrichtigung bei neuem Abo
    /// </summary>
    public bool NotificationOnSubscription { get; set; }

    /// <summary>
    /// Benachrichtigung bei "Deabonnierung"
    /// </summary>
    public bool NotificationOnUnsubscription { get; set; }

    /// <summary>
    /// Zugriff auf Daten für Forschungsinstitute gewährt
    /// </summary>
    public bool AccessForResearchInstitutesGranted { get; set; }

    /// <summary>
    ///     Zugehöriges IoT Device
    /// </summary>
    [ForeignKey(nameof(TblMeasurementDefinitionId))]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public virtual TableMeasurementDefinition TblMeasurementDefinition { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    ///     Abos
    /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
    public ICollection<TableAbo> TblAbos { get; set; } = new List<TableAbo>();
#pragma warning restore CA2227 // Collection properties should be read only

    /// <summary>
    ///     Ratings an diese Messwertdefinition
    /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
    public ICollection<TableRating> TblRatings { get; set; } = new List<TableRating>();
#pragma warning restore CA2227 // Collection properties should be read only

    /// <summary>
    ///     Benachrichtigungen welche diese Messwertdefinition betreffen
    /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
    public ICollection<TableNotification> TblNotifications { get; set; } = new List<TableNotification>();
#pragma warning restore CA2227 // Collection properties should be read only

    #endregion

    #region Interface Implementation

    /// <summary>
    /// Version der Zeile. Wird automatisch durch den SQL Server aktualisiert
    /// https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/handling-concurrency-with-the-entity-framework-in-an-asp-net-mvc-application#add-an-optimistic-concurrency-property-to-the-department-entity
    /// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
    public byte[] DataVersion { get; set; } = Array.Empty<byte>();
#pragma warning restore CA1819 // Properties should not return arrays

    /// <summary>
    /// Archiviert
    /// </summary>
    public bool IsArchived { get; set; }

    #endregion
}