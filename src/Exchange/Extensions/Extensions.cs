// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Apps.Map.Model;

namespace Exchange.Extensions
{
    /// <summary>
    /// <para>Extension methods</para>
    /// Klasse Extensions. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Konvertierung zu Biss Position
        /// </summary>
        /// <param name="position">exposition</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static BissPosition ToBissPosition(this ExPosition position)
        {
            if (position == null!)
            {
                throw new ArgumentNullException($"[{nameof(Extensions)}]({nameof(ToBissPosition)}): {nameof(position)} is null");
            }

            return new BissPosition(position.Latitude, position.Longitude, position.Altitude);
        }

        /// <summary>
        /// Display name fuer enum Wert
        /// </summary>
        /// <param name="enumValue">enum</param>
        /// <returns>Display name</returns>
        public static string GetDisplayName(this System.Enum enumValue)
        {
            var text = enumValue.ToString();
            return enumValue.GetType().GetMember(text).FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? text;
        }

        /// <summary>
        /// Kurzer Display name fuer enum wert
        /// </summary>
        /// <param name="enumValue">enum wert</param>
        /// <returns>Display name</returns>
        public static string GetDisplayShortName(this System.Enum enumValue)
        {
            var text = enumValue.ToString();
            return enumValue.GetType().GetMember(text).FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>()?.GetShortName() ?? text;
        }

        /// <summary>
        /// Display beschreibung eines enums
        /// </summary>
        /// <param name="enumValue">enum wert</param>
        /// <returns>beschreibung</returns>
        public static string GetDisplayDescription(this System.Enum enumValue)
        {
            var text = enumValue.ToString();
            return enumValue.GetType().GetMember(text).FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>()?.GetDescription() ?? text;
        }
    }
}