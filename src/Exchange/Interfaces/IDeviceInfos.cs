// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:56
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using BDA.Common.Exchange.Model;

namespace Exchange.Interfaces
{
    /// <summary>
    ///     <para>Device Infos Interface</para>
    /// Klasse IDeviceInfos. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public interface IDeviceInfos
    {
        /// <summary>
        ///     Plattform Infos auslesen
        /// </summary>
        /// <returns></returns>
        ExDeviceInfo GetInfosDeviceInfo();
    }
}