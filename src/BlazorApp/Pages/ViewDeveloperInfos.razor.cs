// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 23.1.2024 10:37
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Pages
{
    /// <summary>
    ///     View Dev Infos
    /// </summary>
    public partial class ViewDeveloperInfos
    {
        /// <summary>
        ///     css Klasse für Loglevel zuweisen
        /// </summary>
        /// <param name="loglevel">Loglevel</param>
        /// <returns></returns>
        private string GetCssClassForLogLevel(LogLevel loglevel)
        {
            if (loglevel == LogLevel.Information)
            {
                return "prompt-i";
            }

            if (loglevel == LogLevel.Error)
            {
                return "prompt-e";
            }

            if (loglevel == LogLevel.Warning)
            {
                return "prompt-w";
            }

            return "prompt-t";
        }
    }
}