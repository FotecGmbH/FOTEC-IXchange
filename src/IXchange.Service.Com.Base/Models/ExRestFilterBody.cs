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
    /// <para>Filter</para>
    /// Klasse ExFilterBody. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExRestFilterBody
    {
        #region Properties

        /// <summary>
        /// Filter für zusätzliche Attribute
        /// </summary>
        public string AdditionalProperties { get; set; } = null!;

        #endregion
    }
}