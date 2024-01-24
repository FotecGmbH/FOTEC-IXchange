// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biss.Dc.Server;
using Biss.Log.Producer;
using IXchange.GatewayService.Interfaces;
using IXchange.Service.AppConnectivity.DataConnector;
using IXchangeDatabase;
using IXchangeDatabase.Converter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebExchange;
using WebExchange.Interfaces;

namespace IXchange.Service.TriggerAgent;

/// <summary>
///     <para>Service für Nachrichtenaustausch</para>
/// Klasse TriggerAgent. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
/// </summary>
public class TriggerAgent : ITriggerAgent
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly IDcConnections _app;
    private readonly ServerRemoteCalls _dc;
    private readonly IGatewayConnectedClientsManager _gateway;

    /// <summary>
    ///     Service für Nachrichtenaustausch
    /// </summary>
    /// <param name="serviceScopeFactory"></param>
    /// <param name="appConnectivity"></param>
    /// <param name="gateway"></param>
    /// <exception cref="ArgumentException"></exception>
    public TriggerAgent(IServiceScopeFactory serviceScopeFactory, IDcConnections appConnectivity, IGatewayConnectedClientsManager gateway)
    {
        _app = appConnectivity;
        _gateway = gateway;

        if (serviceScopeFactory == null!)
        {
            throw new ArgumentException($"[{nameof(TriggerAgent)}]({nameof(TriggerAgent)}): {nameof(serviceScopeFactory)} is NULL!");
        }

        var scope = serviceScopeFactory.CreateScope();
        var s = scope.ServiceProvider.GetService<IServerRemoteCalls>();
        if (scope == null! || s == null!)
        {
            throw new ArgumentException($"[{nameof(TriggerAgent)}]({nameof(TriggerAgent)}): {nameof(scope)} is NULL!");
        }

        _dc = (ServerRemoteCalls) s;
        _dc.SetClientConnection(_app);
    }

    /// <summary>
    /// Gatewaydaten (inkl. Iot-Devices) haben sich geändert. Das Gateway hat aktualisierte Daten gesendet
    /// Prüfen ob Daten (abhängig von der Configversion des Gateways und der Iot-Devices) an das Gateway gesendet werden müssen
    /// </summary>
    /// <param name="gatewayId"></param>
    /// <returns></returns>
    private async Task ChangedGatewayFromSaConnectivity(long gatewayId)
    {
        // ReSharper disable once UnusedVariable
        var dc = _dc;
        // ReSharper disable once UnusedVariable
        var gw = _gateway;

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var db = new Db();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        var g = await db.TblGateways.Where(t => t.Id == gatewayId).Include(i => i.TblIotDevices).ThenInclude(t => t.TblMeasurementDefinitions).FirstAsync().ConfigureAwait(false);

        if (g == null!)
        {
            Logging.Log.LogError($"[{nameof(TriggerAgent)}]({nameof(ChangeTrackerExtensions)}):  Gateway {gatewayId} not found.");
        }

        var update = g!.DeviceCommon.ConfigversionDevice != g.DeviceCommon.ConfigversionService;
        if (!update)
        {
            foreach (var iot in g.TblIotDevices)
            {
                if (iot.DeviceCommon.ConfigversionService != /*g.DeviceCommon.ConfigversionDevice*/ iot.DeviceCommon.ConfigversionDevice)
                {
                    update = true;
                    break;
                }
            }
        }

        //Gateway informieren
        if (update)
        {
            var r = await _gateway.SendConfig(gatewayId, g.ToExGwServiceGatewayConfig()).ConfigureAwait(false);
            if (r)
            {
                g.DeviceCommon.ConfigversionDevice = g.DeviceCommon.ConfigversionService;
                await db.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }

    private async Task ChangedGatewayFromGatewayService(long gatewayId)
    {
        await _dc.SendGatewayUpdate(gatewayId).ConfigureAwait(false);
    }

    #region Interface Implementations

    /// <summary>
    ///     Gatewaydaten wurden verändert
    /// </summary>
    /// <param name="source">Wer hat die Änderung gemacht?</param>
    /// <param name="gatewayId">Welches Gateway</param>
    public async Task ChangedGateway(EnumTriggerSources source, long gatewayId)
    {
        Logging.Log.LogInfo($"[{nameof(TriggerAgent)}]({nameof(ChangedGateway)}): ChangedGateway from {source} for gateway {gatewayId}");
        //ToDo Gwe: Mit Mko reden
        switch (source)
        {
            case EnumTriggerSources.ServiceAppConnectivity:
                await ChangedGatewayFromSaConnectivity(gatewayId).ConfigureAwait(false);
                break;
            case EnumTriggerSources.GatewayService:
                await ChangedGatewayFromGatewayService(gatewayId).ConfigureAwait(false);
                break;
        }

        await _dc.GatewayDataChanged(gatewayId).ConfigureAwait(false);
        await ChangedGatewayFromSaConnectivity(gatewayId).ConfigureAwait(false);
    }

    /// <summary>
    /// Neue Daten vom Gateway wurden in DB gesichert
    /// </summary>
    /// <param name="gatewayId"></param>
    /// <param name="measurementDefinitionIds"></param>
    public async void NewMeasurementsFromGateway(long gatewayId, List<long> measurementDefinitionIds)
    {
        await _dc.MeasurementDefinitionDataChanged(measurementDefinitionIds, gatewayId).ConfigureAwait(false);

        //await _dc.SendReloadList(EnumReloadDcList.ExMeasurementDefinition).ConfigureAwait(false);
    }

    /// <summary>
    /// Status eines Iot Gerätes (Online/Offline/Configversion/Firmwareverseion ...) haben sich geändert
    /// </summary>
    /// <param name="iotDeviceId"></param>
    public async Task IotDeviceStatusChanged(long iotDeviceId)
    {
        await _dc.IotDeviceDataChanged(iotDeviceId).ConfigureAwait(false);
    }

    #endregion
}