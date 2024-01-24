// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebExchange.Interfaces
{
    /// <summary>
    /// <para>Inteface für den TriggerAgent</para>
    /// Interface ITriggerAgent. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public interface ITriggerAgent
    {
        /// <summary>
        /// Gatewaydaten wurden verändert
        /// </summary>
        /// <param name="source">Wer hat die Änderung gemacht?</param>
        /// <param name="gatewayId">Welches Gateway</param>
        Task ChangedGateway(EnumTriggerSources source, long gatewayId);

        /// <summary>
        /// Neue Daten vom Gateway wurden in DB gesichert
        /// </summary>
        /// <param name="gatewayId"></param>
        /// <param name="measurementDefinitionIds"></param>
        void NewMeasurementsFromGateway(long gatewayId, List<long> measurementDefinitionIds);

        /// <summary>
        /// Status eines Iot Gerätes (Online/Offline/Configversion/Firmwareverseion ...) haben sich geändert
        /// </summary>
        /// <param name="iotDeviceId"></param>
        Task IotDeviceStatusChanged(long iotDeviceId);
    }
}