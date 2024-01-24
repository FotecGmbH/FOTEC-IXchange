// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:56
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

namespace Exchange.Enum
{
    /// <summary>
    /// <para>Rückgabewerte wenn eine View mit einem CheckSave bearbeitet wurde</para>
    /// Klasse EnumVmEditResult. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// ToDo MKo - Kommt nach Biss.Apps.Base
    /// </summary>
    public enum EnumVmEditResult
    {
        /// <summary>
        /// Keine Änderrungen am Datensatz
        /// </summary>
        NotModified,

        /// <summary>
        /// Geändert und bereits gesichert
        /// </summary>
        ModifiedAndStored,

        /// <summary>
        /// Geändert
        /// </summary>
        Modified
    }
}