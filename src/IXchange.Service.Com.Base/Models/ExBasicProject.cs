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

// ReSharper disable once CheckNamespace
namespace IXchange.Service.Com.Base
{
    /// <summary>
    /// <para>This class represents a basic project entity.</para>
    /// Klasse ExBasicProject. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExBasicProject
    {
        #region Properties

        /// <summary>
        ///     Basic information of the project enity.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ExBasicInformation Information { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        ///     Measurement definitions of the company.
        /// </summary>
        public List<ExBasicMeasurementDefinition> MeasurementDefinitions { get; set; } = new List<ExBasicMeasurementDefinition>();

        #endregion
    }
}