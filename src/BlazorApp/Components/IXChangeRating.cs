// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 23.1.2024 10:38
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using Radzen.Blazor;

namespace BlazorApp.Components
{
    /// <summary>
    /// <para>Rating für IXChange</para>
    /// Klasse IXChangeRating. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class IXChangeRating : RadzenRating
    {
        /// <summary>
        /// Initialisierung
        /// </summary>
        public IXChangeRating()
        {
            Stars = 5;
        }
    }
}