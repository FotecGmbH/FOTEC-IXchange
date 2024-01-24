﻿// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;

namespace IXchange.Service.AppConnectivity.Helper
{
    /// <summary>
    ///     <para>Aktuelle Einstellungen</para>
    /// Klasse CurrentSettings. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class CurrentSettings
    {
        #region Properties

        /// <summary>
        ///     AGB
        /// </summary>
        public string Agb { get; set; } = null!;

        /// <summary>
        ///     Aktuelle App Version
        /// </summary>
        public string CurrentAppVersion { get; set; } = null!;

        /// <summary>
        ///     Minimale App Version
        /// </summary>
        public string MinAppVersion { get; set; } = null!;

        /// <summary>
        ///     Allgemeine Nachricht
        /// </summary>
        public string CommonMessage { get; set; } = null!;

        #endregion
    }
}