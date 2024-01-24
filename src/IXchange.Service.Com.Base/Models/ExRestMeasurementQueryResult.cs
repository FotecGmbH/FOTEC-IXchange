// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace IXchange.Service.Com.Base
{
    /// <summary>
    /// <para>Messungen und Anzahl der gesamten Elemente</para>
    /// Klasse ExMeasurementQueryResult. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExRestMeasurementQueryResult
    {
        #region Properties

        /// <summary>
        /// Anzahl der gesammten Einträge
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// Ergebnisse der Messungen
        /// </summary>
        public IQueryable<ExRestMeasurementResult>? MeasurementResults { get; set; }

        #endregion
    }
}