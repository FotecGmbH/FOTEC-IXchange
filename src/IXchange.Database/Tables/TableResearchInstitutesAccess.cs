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

// ReSharper disable once CheckNamespace
namespace IXchangeDatabase.Tables
{
    /// <summary>
    /// <para>Tabelle für Zugriff von Forschungseinrichtungen auf Daten über OData</para>
    /// Klasse TableResearchInstitutesAccess. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("TableResearchInstitutesAccess")]
    public class TableResearchInstitutesAccess : IDcChangeTracking
    {
        #region Properties

        /// <summary>
        ///     ID für DB
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Name der Forschungseinrichtung
        /// </summary>
        // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
        public string ResearchInstituteName { get; set; } = string.Empty;

        /// <summary>
        /// zusätzliche Daten (falls mal gebraucht)
        /// </summary>
        // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
        public string AdditionalData { get; set; } = string.Empty;

        /// <summary>
        /// Zugriffstoken
        /// </summary>
        // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Gültig ab
        /// </summary>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Gültig bis
        /// </summary>
        public DateTime ValidUntil { get; set; } = new DateTime(2099, 12, 31);

        #endregion

        #region Interface Implementation

        /// <summary>
        ///     Version der Zeile. Wird automatisch durch den SQL Server aktualisiert
        ///     https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/handling-concurrency-with-the-entity-framework-in-an-asp-net-mvc-application#add-an-optimistic-concurrency-property-to-the-department-entity
        /// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
        [Timestamp]
        public byte[] DataVersion { get; set; } = Array.Empty<byte>();
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Archiviert
        /// </summary>
        public bool IsArchived { get; set; }

        #endregion
    }
}