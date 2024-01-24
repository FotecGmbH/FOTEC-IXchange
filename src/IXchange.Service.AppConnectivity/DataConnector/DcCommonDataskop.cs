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
using BDA.Common.Exchange.Enum;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Dc.Core;
using Database.Converter;
using Database.Tables;
using IXchangeDatabase;
using Microsoft.EntityFrameworkCore;


namespace IXchange.Service.AppConnectivity.DataConnector
{
    /// <summary>
    ///     <para>Allgemeine IXchange DC Hilfsfunktionen</para>
    /// Klasse ServerRemoteCalls. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public partial class ServerRemoteCalls
    {
        /// <summary>
        ///     (Status) Iot Gerät hat sich an/abgemeldet
        /// </summary>
        /// <param name="iotDeviceId"></param>
        /// <returns></returns>
        public async Task IotDeviceDataChanged(long iotDeviceId)
        {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await using var db = new Db();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

            var tmp = db.TblIotDevices.Where(f => f.Id == iotDeviceId).Include(i => i.TblGateway).First()
                .ToExIoTDevice();

            var update = new DcServerListItem<ExIotDevice>
                         {
                             Data = tmp,
                             Index = iotDeviceId,
                             SortIndex = iotDeviceId
                         };

            //var gw = db.TblGateways.Where(g => g.Id == gatewayId).Include(i => i.TblCompany).ThenInclude(i => i.TblPermissions).ThenInclude(p => p.TblUser);
            var usersToInform = db.TblUsers.Where(u =>
                u.IsAdmin ||
                u.TblPermissions.Any(p => p.TblCompany.CompanyType == EnumCompanyTypes.PublicCompany) ||
                u.TblPermissions.Any(p2 => p2.TblCompany.TblGateways.Any(g => g.Id == tmp.GatewayId)
                )).Select(i => i.Id).ToList().Distinct();

            foreach (var u in usersToInform)
            {
                if (ClientConnection.GetClients().Any(c => c.UserId == u))
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    await SendDcExIotDevices(new List<DcServerListItem<ExIotDevice>> {update}, userId: u).ConfigureAwait(false);
#pragma warning restore CS0618 // Type or member is obsolete
                }
            }
        }

        /// <summary>
        ///     Eine Messwertdefinition hat neue Daten
        /// </summary>
        /// <param name="measurementDefinitionIds"></param>
        /// <param name="gatewayId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task MeasurementDefinitionDataChanged(List<long> measurementDefinitionIds, long gatewayId)
        {
            if (measurementDefinitionIds == null!)
            {
                throw new ArgumentNullException($"[{nameof(ServerRemoteCalls)}]({nameof(MeasurementDefinitionDataChanged)}): {nameof(measurementDefinitionIds)}");
            }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await using var db = new Db();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task


            var data = new List<DcServerListItem<ExMeasurementDefinition>>();
            foreach (var id in measurementDefinitionIds)
            {
                var tmp = db.TblMeasurementDefinitions.AsNoTracking().Where(c => c.Id == id).Include(i => i.TblIoTDevice).ThenInclude(t => t.TblGateway).ThenInclude(t => t!.TblCompany).Include(i => i.TblMeasurements).Select(s => new TableMeasurementDefinition
                                                                                                                                                                                                                                     {
                                                                                                                                                                                                                                         TblMeasurements = s.TblMeasurements.Where(w => w.Id == s.TblMeasurements.Max(m => m.Id)).ToList(),
                                                                                                                                                                                                                                         AdditionalConfiguration = s.AdditionalConfiguration,
                                                                                                                                                                                                                                         AdditionalProperties = s.AdditionalProperties,
                                                                                                                                                                                                                                         DownstreamType = s.DownstreamType,
                                                                                                                                                                                                                                         Id = s.Id,
                                                                                                                                                                                                                                         Information = s.Information,
                                                                                                                                                                                                                                         MeasurementInterval = s.MeasurementInterval,
                                                                                                                                                                                                                                         TblIoTDevice = s.TblIoTDevice,
                                                                                                                                                                                                                                         TblIotDeviceId = s.TblIotDeviceId,
                                                                                                                                                                                                                                         ValueType = s.ValueType,
                                                                                                                                                                                                                                     }).FirstOrDefault()?.ToExMeasurementDefinition();

                if (tmp is null)
                {
                    return;
                }

                var update = new DcServerListItem<ExMeasurementDefinition>
                             {
                                 Data = tmp,
                                 Index = id,
                                 SortIndex = id
                             };
                data.Add(update);
            }

            var usersToInform = db.TblUsers.Where(u =>
                u.IsAdmin ||
                u.TblPermissions.Any(p => p.TblCompany.CompanyType == EnumCompanyTypes.PublicCompany) ||
                u.TblPermissions.Any(p2 => p2.TblCompany.TblGateways.Any(g => g.Id == gatewayId)
                )).Select(i => i.Id).ToList().Distinct();

            foreach (var u in usersToInform)
            {
                if (ClientConnection.GetClients().Any(c => c.UserId == u))
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    await SendDcExMeasurementDefinition(data, userId: u).ConfigureAwait(false);
#pragma warning restore CS0618 // Type or member is obsolete
                }
            }
        }

        /// <summary>
        ///     Devices über Gateway Änderung informieren.
        /// </summary>
        /// <param name="gatewayId">Id des Gateways</param>
        /// <param name="oldCompanyId">Die company aus welcher der gateway entfernt wurde.</param>
        public async Task SendGatewayUpdate(long gatewayId, long oldCompanyId = 0)
        {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await using var db = new Db();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
            var sendList = new List<DcServerListItem<ExCompany>>();
            var sendDeviceIds = new List<long>();

            if (oldCompanyId > 0)
            {
                var oldCompany = await db.TblCompanies
                    .Include(c => c.TblProjects)
                    .Include(c => c.TblGateways)
                    .Include(c => c.TblPermissions)
                    .FirstOrDefaultAsync(c => c.Id == oldCompanyId)
                    .ConfigureAwait(false);

                if (oldCompany == null!)
                {
                    throw new InvalidOperationException($"[{nameof(DcCommonData)}]({nameof(SendGatewayUpdate)}): OldCompany konnte nicht gefunden werden!");
                }

                sendList.Add(new()
                             {
                                 // ReSharper disable once RedundantCast
                                 Data = (ExCompany) oldCompany.ToExCompany(),
                                 Index = oldCompany.Id,
                                 SortIndex = oldCompany.Id
                             });

                var userIds = oldCompany.TblPermissions.Select(p => p.TblUserId);

                foreach (var id in userIds)
                {
                    sendDeviceIds.AddRange(ClientConnection.GetClients().Where(c => c.UserId == id).Select(c => c.DeviceId));
                }
            }

            var gateway = await db.TblGateways
                .Include(g => g.TblCompany)
                .ThenInclude(g => g.TblPermissions)
                .Include(g => g.TblCompany)
                .ThenInclude(c => c.TblProjects)
                .Include(g => g.TblCompany)
                .ThenInclude(c => c.TblGateways)
                .FirstOrDefaultAsync(g => g.Id == gatewayId)
                .ConfigureAwait(false);

            if (gateway != null!)
            {
                sendList.Add(new()
                             {
                                 // ReSharper disable once RedundantCast
                                 Data = (ExCompany) gateway.TblCompany.ToExCompany(),
                                 Index = gateway.TblCompany.Id,
                                 SortIndex = gateway.TblCompany.Id
                             });


                if (gateway.TblCompany.CompanyType == EnumCompanyTypes.NoCompany)
                {
                    sendDeviceIds.AddRange(ClientConnection.GetClients().Select(c => c.DeviceId));
                }
                else
                {
                    var userIds = gateway.TblCompany.TblPermissions.Select(p => p.TblUserId);

                    foreach (var id in userIds)
                    {
                        sendDeviceIds.AddRange(ClientConnection.GetClients().Where(c => c.UserId == id).Select(c => c.DeviceId));
                    }
                }
            }

            _ = Task.Run(async () =>
            {
                foreach (var device in sendDeviceIds)
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    await SendDcExCompanies(sendList, device).ConfigureAwait(false);
#pragma warning restore CS0618 // Type or member is obsolete
                }
            });
        }
    }
}