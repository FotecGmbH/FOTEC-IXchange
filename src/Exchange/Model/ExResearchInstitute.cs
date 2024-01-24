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

namespace Exchange.Model
{
    /// <summary>
    /// <para>Forschungseinrichtung</para>
    /// Klasse ExResearchInstitute. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExResearchInstitute : IBissModel
    {
        #region Properties

        /// <summary>
        /// Db-Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// zusätzliche Daten (falls mal gebraucht)
        /// </summary>
        public string AdditionalData { get; set; } = string.Empty;

        /// <summary>
        /// Zugriffstoken
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Gültig ab
        /// </summary>
        public DateTime? AccessValidFrom { get; set; }

        /// <summary>
        /// Gültig bis
        /// </summary>
        public DateTime AccessValidUntil { get; set; }

        #endregion

        /// <summary>
        /// Ob Zugriff
        /// </summary>
        /// <returns>ob zugriff</returns>
        public bool HasAccess()
        {
            var now = DateTime.UtcNow;

            return !string.IsNullOrEmpty(AccessToken) &&
                   (AccessValidFrom is null || AccessValidFrom.Value <= now) &&
                   AccessValidUntil >= now;
        }

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