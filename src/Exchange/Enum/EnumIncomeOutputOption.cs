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
    /// <para>Option ueber Income Output Art</para>
    /// Klasse EnumIncomeOutputOption. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public enum EnumIncomeOutputOption
    {
        /// <summary>
        /// Kosten/Einkommen aufgrund eines Abos
        /// </summary>
        Abo,

        /// <summary>
        /// Einkommen aufgrund einer Bewertung
        /// </summary>
        Rating,

        /// <summary>
        /// Einkommen aufgrund von uebertragen von Daten
        /// </summary>
        Transfer,

        /// <summary>
        /// Einkommen aufgrund von Verknuepfung eines Sensors
        /// </summary>
        CreateIoTDevice,

        /// <summary>
        /// Einkommen aufgrund erstellung eines Accounts
        /// </summary>
        CreateAccount
    }
}