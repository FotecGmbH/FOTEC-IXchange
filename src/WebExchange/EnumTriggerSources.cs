// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

namespace WebExchange
{
    /// <summary>
    /// <para>Wer hat den TiggerAgent "getriggert"?</para>
    /// Klasse EnumTriggerSources. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public enum EnumTriggerSources
    {
        /// <summary>
        /// Von einem "Drittsystem"
        /// </summary>
        ServiceComBase,

        /// <summary>
        /// Von der Backend App
        /// </summary>
        ServiceAppConnectivity,

        /// <summary>
        /// Von einem Gateway
        /// </summary>
        GatewayService
    }
}