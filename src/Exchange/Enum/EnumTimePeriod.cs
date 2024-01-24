// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

namespace Exchange.Enum
{
    /// <summary>
    /// <para>Zeitspanne</para>
    /// Klasse EnumTimePeriod. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public enum EnumTimePeriod
    {
        /// <summary>
        /// Alle Werte
        /// </summary>
        All = 0,

        /// <summary>
        /// Werte des letzten Tages
        /// </summary>
        Day = 1,

        /// <summary>
        /// Werte der letzten Woche
        /// </summary>
        Week = 2,

        /// <summary>
        /// Werte des letzten Monats
        /// </summary>
        Month = 3,

        /// <summary>
        /// Werte des letzten Jahres
        /// </summary>
        Year = 4
    }
}