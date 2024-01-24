// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;

// ReSharper disable once CheckNamespace
namespace IXchange.Service.Com.Base
{
    /// <summary>
    /// <para>This class represents basic information of an entity</para>
    /// Klasse ExBasicInformation. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExBasicInformation
    {
        #region Properties

        /// <summary>
        ///     ID of Entity
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public long ID { get; set; }

        /// <summary>
        ///     Name of Entity
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Name { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        #endregion
    }
}