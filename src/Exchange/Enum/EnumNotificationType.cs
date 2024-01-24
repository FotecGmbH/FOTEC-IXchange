// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System.ComponentModel.DataAnnotations;
using Exchange.Resources;

namespace Exchange.Enum
{
    /// <summary>
    /// <para>Which Type is the Notification</para>
    /// Klasse EnumNotificationType. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public enum EnumNotificationType
    {
        /// <summary>
        /// Ausfall seit X Minuten
        /// </summary>
        [Display(Name = "EnumNotificationType_Failure", ResourceType = typeof(ResEnumTranslations))]
        Failure,

        /// <summary>
        /// Bewertung
        /// </summary>
        [Display(Name = "EnumNotificationType_Rating", ResourceType = typeof(ResEnumTranslations))]
        Rating,

        /// <summary>
        /// Überschreitung eines Wertes
        /// </summary>
        [Display(Name = "EnumNotificationType_Exceed", ResourceType = typeof(ResEnumTranslations))]
        Exceed,

        /// <summary>
        /// Unterschreitung eines Wertes
        /// </summary>
        [Display(Name = "EnumNotificationType_Undercut", ResourceType = typeof(ResEnumTranslations))]
        Undercut,

        /// <summary>
        /// Abweichung vom Mittelwert
        /// </summary>
        [Display(Name = "EnumNotificationType_MovingAverage", ResourceType = typeof(ResEnumTranslations))]
        MovingAverage
    }
}