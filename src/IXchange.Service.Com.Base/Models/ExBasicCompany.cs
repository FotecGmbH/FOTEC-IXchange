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

namespace IXchange.Service.Com.Base
{
    /// <summary>
    /// <para>This class represents a basic company model.</para>
    /// Klasse ExBasicCompany. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExBasicCompany
    {
        #region Properties

        /// <summary>
        ///     Basic information of the company enity.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ExBasicInformation Information { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        ///     Projects of the company.
        /// </summary>
        public List<ExBasicProject> Projects { get; set; } = new List<ExBasicProject>();

        #endregion
    }
}