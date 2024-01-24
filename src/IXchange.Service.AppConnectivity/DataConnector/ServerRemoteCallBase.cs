// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Threading.Tasks;
using BDA.Common.Exchange.Model;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Dc.Core;
using Biss.Dc.Server;
using Biss.Serialize;
using Exchange.Model.ConfigApp;
// ReSharper disable once RedundantUsingDirective
using System.Collections.Generic;
// ReSharper disable once RedundantUsingDirective
using System.Linq;

namespace IXchange.Service.AppConnectivity.DataConnector
{
    /// <summary>
    /// <para>ServerRemoteCallBase</para>
    /// Klasse ServerRemoteCallBase. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public abstract class ServerRemoteCallBase
    {
        /// <summary>
        /// Zugriff auf die Sync-Funktionen der Listen
        /// </summary>
        private static Func<string, long, long, DcListSyncData, DcListSyncProperties, Task<DcListSyncTransferData>> _syncFunc = null!;

        #region Properties

        /// <summary>
        /// Zugriff auf die Kommunikation mit den angemeldeten Clients
        /// </summary>
        public IDcConnections ClientConnection { get; set; } = null!;

        #endregion

        /// <summary>
        /// Workaround um in den Server-Funktionen den Zugriff auf alle angemeldeten Clients zu ermöglichen
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="syncFunc"></param>
        public void SetClientConnection(object connection, Func<string, long, long, DcListSyncData, DcListSyncProperties, Task<DcListSyncTransferData>> syncFunc = null!)
        {
            if (connection is IDcConnections con)
            {
                ClientConnection = con;
            }
            else
            {
                throw new InvalidCastException();
            }

            if (syncFunc != null! && _syncFunc == null!)
            {
                _syncFunc = syncFunc;
            }
        }

        #region Sendefunktionen

        /// <summary>
        /// Daten an DcExDeviceInfo senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        public async Task<int> SendDcExDeviceInfo(ExDeviceInfo data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            return await SendInternal("DcExDeviceInfo", data.ToJson(), deviceId, userId, excludeDeviceId).ConfigureAwait(false);
        }


        /// <summary>
        /// Daten an DcExUser senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        public async Task<int> SendDcExUser(ExUser data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            return await SendInternal("DcExUser", data.ToJson(), deviceId, userId, excludeDeviceId).ConfigureAwait(false);
        }


        /// <summary>
        /// Daten an DcExUserPassword senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        public async Task<int> SendDcExUserPassword(ExUserPassword data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            return await SendInternal("DcExUserPassword", data.ToJson(), deviceId, userId, excludeDeviceId).ConfigureAwait(false);
        }


        /// <summary>
        /// Daten an DcExLocalAppData senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        public async Task<int> SendDcExLocalAppData(ExLocalAppSettings data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            return await SendInternal("DcExLocalAppData", data.ToJson(), deviceId, userId, excludeDeviceId).ConfigureAwait(false);
        }


        /// <summary>
        /// Daten an DcExSettingsInDb senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        public async Task<int> SendDcExSettingsInDb(ExSettingsInDb data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            return await SendInternal("DcExSettingsInDb", data.ToJson(), deviceId, userId, excludeDeviceId).ConfigureAwait(false);
        }


        /// <summary>
        /// Listen Daten an DcExCompanies senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        [Obsolete("Bitte SyncDcExCompanies verwenden!")]
        public async Task<int> SendDcExCompanies(List<DcServerListItem<ExCompany>> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            var tmp = data.Select(item => item.ToDcTransferListItem()).ToList();
            return await SendInternal("DcExCompanies", tmp.ToJson(), deviceId, userId).ConfigureAwait(false);
        }

        /// <summary>
        /// Datenänderungen für DcExCompanies an Client weiter geben
        /// </summary>
        /// <param name="data">Sync-Daten</param>
        /// <param name="deviceId">Gerät</param>
        /// <param name="userId">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> SyncDcExCompanies(DcListSyncResultData<ExCompany> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            if (data is IDcListSyncResultData result)
            {
                var tmp = result.ToDcListSyncTransferData();
                var tmp2 = new DcListSyncResult("DcExCompanies")
                           {
                               Data = tmp
                           };

                return await SendInternal("DcExCompanies", tmp2.ToJson(), deviceId, userId, excludeDeviceId, true).ConfigureAwait(false);
            }

            throw new ArgumentException("SyncDcExCompanies has wrong Data");
        }

        /// <summary>
        /// Datenänderungen für DcExCompanies an Client weiter geben
        /// </summary>
        /// <param name="userIds">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf</param>
        /// <returns></returns>
        public async Task<int> SyncDcExCompanies(List<long> userIds, long? excludeDeviceId = null)
        {
            if (userIds == null)
            {
                throw new ArgumentNullException(nameof(userIds));
            }

            var couter = 0;
            foreach (var userId in userIds)
            {
                var clientDevices = ClientConnection.GetClients().Where(w => w.UserId == userId);
                foreach (var device in clientDevices)
                {
                    if (excludeDeviceId.HasValue && excludeDeviceId.Value == device.DeviceId)
                    {
                        continue;
                    }

                    if (!device.ListLastSyncData.ContainsKey("DcExCompanies"))
                    {
                        //Noch kein Sync
                        continue;
                    }

                    if (device.ListLastSyncData["DcExCompanies"].DcListSyncData == null!)
                    {
                        continue;
                    }

                    var data = await _syncFunc("DcExCompanies", device.DeviceId, device.UserId, device.ListLastSyncData["DcExCompanies"].DcListSyncData!, device.ListLastSyncData["DcExCompanies"].DcListSyncProperties!).ConfigureAwait(false);
                    var tmp2 = new DcListSyncResult("DcExCompanies")
                               {
                                   Data = data
                               };
                    await SendInternal("DcExCompanies", tmp2.ToJson(), device.DeviceId, device.UserId, excludeDeviceId, true).ConfigureAwait(false);
                    couter++;
                }
            }

            return couter;
        }


        /// <summary>
        /// Listen Daten an DcExCompanyUsers senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        [Obsolete("Bitte SyncDcExCompanyUsers verwenden!")]
        public async Task<int> SendDcExCompanyUsers(List<DcServerListItem<ExCompanyUser>> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            var tmp = data.Select(item => item.ToDcTransferListItem()).ToList();
            return await SendInternal("DcExCompanyUsers", tmp.ToJson(), deviceId, userId).ConfigureAwait(false);
        }

        /// <summary>
        /// Datenänderungen für DcExCompanyUsers an Client weiter geben
        /// </summary>
        /// <param name="data">Sync-Daten</param>
        /// <param name="deviceId">Gerät</param>
        /// <param name="userId">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> SyncDcExCompanyUsers(DcListSyncResultData<ExCompanyUser> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            if (data is IDcListSyncResultData result)
            {
                var tmp = result.ToDcListSyncTransferData();
                var tmp2 = new DcListSyncResult("DcExCompanyUsers")
                           {
                               Data = tmp
                           };

                return await SendInternal("DcExCompanyUsers", tmp2.ToJson(), deviceId, userId, excludeDeviceId, true).ConfigureAwait(false);
            }

            throw new ArgumentException("SyncDcExCompanyUsers has wrong Data");
        }

        /// <summary>
        /// Datenänderungen für DcExCompanyUsers an Client weiter geben
        /// </summary>
        /// <param name="userIds">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf</param>
        /// <returns></returns>
        public async Task<int> SyncDcExCompanyUsers(List<long> userIds, long? excludeDeviceId = null)
        {
            if (userIds == null)
            {
                throw new ArgumentNullException(nameof(userIds));
            }

            var couter = 0;
            foreach (var userId in userIds)
            {
                var clientDevices = ClientConnection.GetClients().Where(w => w.UserId == userId);
                foreach (var device in clientDevices)
                {
                    if (excludeDeviceId.HasValue && excludeDeviceId.Value == device.DeviceId)
                    {
                        continue;
                    }

                    if (!device.ListLastSyncData.ContainsKey("DcExCompanyUsers"))
                    {
                        //Noch kein Sync
                        continue;
                    }

                    if (device.ListLastSyncData["DcExCompanyUsers"].DcListSyncData == null!)
                    {
                        continue;
                    }

                    var data = await _syncFunc("DcExCompanyUsers", device.DeviceId, device.UserId, device.ListLastSyncData["DcExCompanyUsers"].DcListSyncData!, device.ListLastSyncData["DcExCompanyUsers"].DcListSyncProperties!).ConfigureAwait(false);
                    var tmp2 = new DcListSyncResult("DcExCompanyUsers")
                               {
                                   Data = data
                               };
                    await SendInternal("DcExCompanyUsers", tmp2.ToJson(), device.DeviceId, device.UserId, excludeDeviceId, true).ConfigureAwait(false);
                    couter++;
                }
            }

            return couter;
        }


        /// <summary>
        /// Listen Daten an DcExGateways senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        [Obsolete("Bitte SyncDcExGateways verwenden!")]
        public async Task<int> SendDcExGateways(List<DcServerListItem<ExGateway>> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            var tmp = data.Select(item => item.ToDcTransferListItem()).ToList();
            return await SendInternal("DcExGateways", tmp.ToJson(), deviceId, userId).ConfigureAwait(false);
        }

        /// <summary>
        /// Datenänderungen für DcExGateways an Client weiter geben
        /// </summary>
        /// <param name="data">Sync-Daten</param>
        /// <param name="deviceId">Gerät</param>
        /// <param name="userId">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> SyncDcExGateways(DcListSyncResultData<ExGateway> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            if (data is IDcListSyncResultData result)
            {
                var tmp = result.ToDcListSyncTransferData();
                var tmp2 = new DcListSyncResult("DcExGateways")
                           {
                               Data = tmp
                           };

                return await SendInternal("DcExGateways", tmp2.ToJson(), deviceId, userId, excludeDeviceId, true).ConfigureAwait(false);
            }

            throw new ArgumentException("SyncDcExGateways has wrong Data");
        }

        /// <summary>
        /// Datenänderungen für DcExGateways an Client weiter geben
        /// </summary>
        /// <param name="userIds">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf</param>
        /// <returns></returns>
        public async Task<int> SyncDcExGateways(List<long> userIds, long? excludeDeviceId = null)
        {
            if (userIds == null)
            {
                throw new ArgumentNullException(nameof(userIds));
            }

            var couter = 0;
            foreach (var userId in userIds)
            {
                var clientDevices = ClientConnection.GetClients().Where(w => w.UserId == userId);
                foreach (var device in clientDevices)
                {
                    if (excludeDeviceId.HasValue && excludeDeviceId.Value == device.DeviceId)
                    {
                        continue;
                    }

                    if (!device.ListLastSyncData.ContainsKey("DcExGateways"))
                    {
                        //Noch kein Sync
                        continue;
                    }

                    if (device.ListLastSyncData["DcExGateways"].DcListSyncData == null!)
                    {
                        continue;
                    }

                    var data = await _syncFunc("DcExGateways", device.DeviceId, device.UserId, device.ListLastSyncData["DcExGateways"].DcListSyncData!, device.ListLastSyncData["DcExGateways"].DcListSyncProperties!).ConfigureAwait(false);
                    var tmp2 = new DcListSyncResult("DcExGateways")
                               {
                                   Data = data
                               };
                    await SendInternal("DcExGateways", tmp2.ToJson(), device.DeviceId, device.UserId, excludeDeviceId, true).ConfigureAwait(false);
                    couter++;
                }
            }

            return couter;
        }


        /// <summary>
        /// Listen Daten an DcExIotDevices senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        [Obsolete("Bitte SyncDcExIotDevices verwenden!")]
        public async Task<int> SendDcExIotDevices(List<DcServerListItem<ExIotDevice>> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            var tmp = data.Select(item => item.ToDcTransferListItem()).ToList();
            return await SendInternal("DcExIotDevices", tmp.ToJson(), deviceId, userId).ConfigureAwait(false);
        }

        /// <summary>
        /// Datenänderungen für DcExIotDevices an Client weiter geben
        /// </summary>
        /// <param name="data">Sync-Daten</param>
        /// <param name="deviceId">Gerät</param>
        /// <param name="userId">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> SyncDcExIotDevices(DcListSyncResultData<ExIotDevice> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            if (data is IDcListSyncResultData result)
            {
                var tmp = result.ToDcListSyncTransferData();
                var tmp2 = new DcListSyncResult("DcExIotDevices")
                           {
                               Data = tmp
                           };

                return await SendInternal("DcExIotDevices", tmp2.ToJson(), deviceId, userId, excludeDeviceId, true).ConfigureAwait(false);
            }

            throw new ArgumentException("SyncDcExIotDevices has wrong Data");
        }

        /// <summary>
        /// Datenänderungen für DcExIotDevices an Client weiter geben
        /// </summary>
        /// <param name="userIds">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf</param>
        /// <returns></returns>
        public async Task<int> SyncDcExIotDevices(List<long> userIds, long? excludeDeviceId = null)
        {
            if (userIds == null)
            {
                throw new ArgumentNullException(nameof(userIds));
            }

            var couter = 0;
            foreach (var userId in userIds)
            {
                var clientDevices = ClientConnection.GetClients().Where(w => w.UserId == userId);
                foreach (var device in clientDevices)
                {
                    if (excludeDeviceId.HasValue && excludeDeviceId.Value == device.DeviceId)
                    {
                        continue;
                    }

                    if (!device.ListLastSyncData.ContainsKey("DcExIotDevices"))
                    {
                        //Noch kein Sync
                        continue;
                    }

                    if (device.ListLastSyncData["DcExIotDevices"].DcListSyncData == null!)
                    {
                        continue;
                    }

                    var data = await _syncFunc("DcExIotDevices", device.DeviceId, device.UserId, device.ListLastSyncData["DcExIotDevices"].DcListSyncData!, device.ListLastSyncData["DcExIotDevices"].DcListSyncProperties!).ConfigureAwait(false);
                    var tmp2 = new DcListSyncResult("DcExIotDevices")
                               {
                                   Data = data
                               };
                    await SendInternal("DcExIotDevices", tmp2.ToJson(), device.DeviceId, device.UserId, excludeDeviceId, true).ConfigureAwait(false);
                    couter++;
                }
            }

            return couter;
        }


        /// <summary>
        /// Listen Daten an DcExMeasurementDefinition senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        [Obsolete("Bitte SyncDcExMeasurementDefinition verwenden!")]
        public async Task<int> SendDcExMeasurementDefinition(List<DcServerListItem<ExMeasurementDefinition>> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            var tmp = data.Select(item => item.ToDcTransferListItem()).ToList();
            return await SendInternal("DcExMeasurementDefinition", tmp.ToJson(), deviceId, userId).ConfigureAwait(false);
        }

        /// <summary>
        /// Datenänderungen für DcExMeasurementDefinition an Client weiter geben
        /// </summary>
        /// <param name="data">Sync-Daten</param>
        /// <param name="deviceId">Gerät</param>
        /// <param name="userId">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> SyncDcExMeasurementDefinition(DcListSyncResultData<ExMeasurementDefinition> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            if (data is IDcListSyncResultData result)
            {
                var tmp = result.ToDcListSyncTransferData();
                var tmp2 = new DcListSyncResult("DcExMeasurementDefinition")
                           {
                               Data = tmp
                           };

                return await SendInternal("DcExMeasurementDefinition", tmp2.ToJson(), deviceId, userId, excludeDeviceId, true).ConfigureAwait(false);
            }

            throw new ArgumentException("SyncDcExMeasurementDefinition has wrong Data");
        }

        /// <summary>
        /// Datenänderungen für DcExMeasurementDefinition an Client weiter geben
        /// </summary>
        /// <param name="userIds">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf</param>
        /// <returns></returns>
        public async Task<int> SyncDcExMeasurementDefinition(List<long> userIds, long? excludeDeviceId = null)
        {
            if (userIds == null)
            {
                throw new ArgumentNullException(nameof(userIds));
            }

            var couter = 0;
            foreach (var userId in userIds)
            {
                var clientDevices = ClientConnection.GetClients().Where(w => w.UserId == userId);
                foreach (var device in clientDevices)
                {
                    if (excludeDeviceId.HasValue && excludeDeviceId.Value == device.DeviceId)
                    {
                        continue;
                    }

                    if (!device.ListLastSyncData.ContainsKey("DcExMeasurementDefinition"))
                    {
                        //Noch kein Sync
                        continue;
                    }

                    if (device.ListLastSyncData["DcExMeasurementDefinition"].DcListSyncData == null!)
                    {
                        continue;
                    }

                    var data = await _syncFunc("DcExMeasurementDefinition", device.DeviceId, device.UserId, device.ListLastSyncData["DcExMeasurementDefinition"].DcListSyncData!, device.ListLastSyncData["DcExMeasurementDefinition"].DcListSyncProperties!).ConfigureAwait(false);
                    var tmp2 = new DcListSyncResult("DcExMeasurementDefinition")
                               {
                                   Data = data
                               };
                    await SendInternal("DcExMeasurementDefinition", tmp2.ToJson(), device.DeviceId, device.UserId, excludeDeviceId, true).ConfigureAwait(false);
                    couter++;
                }
            }

            return couter;
        }


        /// <summary>
        /// Listen Daten an DcExDataconverters senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        [Obsolete("Bitte SyncDcExDataconverters verwenden!")]
        public async Task<int> SendDcExDataconverters(List<DcServerListItem<ExDataconverter>> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            var tmp = data.Select(item => item.ToDcTransferListItem()).ToList();
            return await SendInternal("DcExDataconverters", tmp.ToJson(), deviceId, userId).ConfigureAwait(false);
        }

        /// <summary>
        /// Datenänderungen für DcExDataconverters an Client weiter geben
        /// </summary>
        /// <param name="data">Sync-Daten</param>
        /// <param name="deviceId">Gerät</param>
        /// <param name="userId">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> SyncDcExDataconverters(DcListSyncResultData<ExDataconverter> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            if (data is IDcListSyncResultData result)
            {
                var tmp = result.ToDcListSyncTransferData();
                var tmp2 = new DcListSyncResult("DcExDataconverters")
                           {
                               Data = tmp
                           };

                return await SendInternal("DcExDataconverters", tmp2.ToJson(), deviceId, userId, excludeDeviceId, true).ConfigureAwait(false);
            }

            throw new ArgumentException("SyncDcExDataconverters has wrong Data");
        }

        /// <summary>
        /// Datenänderungen für DcExDataconverters an Client weiter geben
        /// </summary>
        /// <param name="userIds">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf</param>
        /// <returns></returns>
        public async Task<int> SyncDcExDataconverters(List<long> userIds, long? excludeDeviceId = null)
        {
            if (userIds == null)
            {
                throw new ArgumentNullException(nameof(userIds));
            }

            var couter = 0;
            foreach (var userId in userIds)
            {
                var clientDevices = ClientConnection.GetClients().Where(w => w.UserId == userId);
                foreach (var device in clientDevices)
                {
                    if (excludeDeviceId.HasValue && excludeDeviceId.Value == device.DeviceId)
                    {
                        continue;
                    }

                    if (!device.ListLastSyncData.ContainsKey("DcExDataconverters"))
                    {
                        //Noch kein Sync
                        continue;
                    }

                    if (device.ListLastSyncData["DcExDataconverters"].DcListSyncData == null!)
                    {
                        continue;
                    }

                    var data = await _syncFunc("DcExDataconverters", device.DeviceId, device.UserId, device.ListLastSyncData["DcExDataconverters"].DcListSyncData!, device.ListLastSyncData["DcExDataconverters"].DcListSyncProperties!).ConfigureAwait(false);
                    var tmp2 = new DcListSyncResult("DcExDataconverters")
                               {
                                   Data = data
                               };
                    await SendInternal("DcExDataconverters", tmp2.ToJson(), device.DeviceId, device.UserId, excludeDeviceId, true).ConfigureAwait(false);
                    couter++;
                }
            }

            return couter;
        }


        /// <summary>
        /// Listen Daten an DcExGlobalConfig senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        [Obsolete("Bitte SyncDcExGlobalConfig verwenden!")]
        public async Task<int> SendDcExGlobalConfig(List<DcServerListItem<ExGlobalConfig>> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            var tmp = data.Select(item => item.ToDcTransferListItem()).ToList();
            return await SendInternal("DcExGlobalConfig", tmp.ToJson(), deviceId, userId).ConfigureAwait(false);
        }

        /// <summary>
        /// Datenänderungen für DcExGlobalConfig an Client weiter geben
        /// </summary>
        /// <param name="data">Sync-Daten</param>
        /// <param name="deviceId">Gerät</param>
        /// <param name="userId">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> SyncDcExGlobalConfig(DcListSyncResultData<ExGlobalConfig> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            if (data is IDcListSyncResultData result)
            {
                var tmp = result.ToDcListSyncTransferData();
                var tmp2 = new DcListSyncResult("DcExGlobalConfig")
                           {
                               Data = tmp
                           };

                return await SendInternal("DcExGlobalConfig", tmp2.ToJson(), deviceId, userId, excludeDeviceId, true).ConfigureAwait(false);
            }

            throw new ArgumentException("SyncDcExGlobalConfig has wrong Data");
        }

        /// <summary>
        /// Datenänderungen für DcExGlobalConfig an Client weiter geben
        /// </summary>
        /// <param name="userIds">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf</param>
        /// <returns></returns>
        public async Task<int> SyncDcExGlobalConfig(List<long> userIds, long? excludeDeviceId = null)
        {
            if (userIds == null)
            {
                throw new ArgumentNullException(nameof(userIds));
            }

            var couter = 0;
            foreach (var userId in userIds)
            {
                var clientDevices = ClientConnection.GetClients().Where(w => w.UserId == userId);
                foreach (var device in clientDevices)
                {
                    if (excludeDeviceId.HasValue && excludeDeviceId.Value == device.DeviceId)
                    {
                        continue;
                    }

                    if (!device.ListLastSyncData.ContainsKey("DcExGlobalConfig"))
                    {
                        //Noch kein Sync
                        continue;
                    }

                    if (device.ListLastSyncData["DcExGlobalConfig"].DcListSyncData == null!)
                    {
                        continue;
                    }

                    var data = await _syncFunc("DcExGlobalConfig", device.DeviceId, device.UserId, device.ListLastSyncData["DcExGlobalConfig"].DcListSyncData!, device.ListLastSyncData["DcExGlobalConfig"].DcListSyncProperties!).ConfigureAwait(false);
                    var tmp2 = new DcListSyncResult("DcExGlobalConfig")
                               {
                                   Data = data
                               };
                    await SendInternal("DcExGlobalConfig", tmp2.ToJson(), device.DeviceId, device.UserId, excludeDeviceId, true).ConfigureAwait(false);
                    couter++;
                }
            }

            return couter;
        }


        /// <summary>
        /// Listen Daten an DcExProjects senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        [Obsolete("Bitte SyncDcExProjects verwenden!")]
        public async Task<int> SendDcExProjects(List<DcServerListItem<ExProject>> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            var tmp = data.Select(item => item.ToDcTransferListItem()).ToList();
            return await SendInternal("DcExProjects", tmp.ToJson(), deviceId, userId).ConfigureAwait(false);
        }

        /// <summary>
        /// Datenänderungen für DcExProjects an Client weiter geben
        /// </summary>
        /// <param name="data">Sync-Daten</param>
        /// <param name="deviceId">Gerät</param>
        /// <param name="userId">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> SyncDcExProjects(DcListSyncResultData<ExProject> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            if (data is IDcListSyncResultData result)
            {
                var tmp = result.ToDcListSyncTransferData();
                var tmp2 = new DcListSyncResult("DcExProjects")
                           {
                               Data = tmp
                           };

                return await SendInternal("DcExProjects", tmp2.ToJson(), deviceId, userId, excludeDeviceId, true).ConfigureAwait(false);
            }

            throw new ArgumentException("SyncDcExProjects has wrong Data");
        }

        /// <summary>
        /// Datenänderungen für DcExProjects an Client weiter geben
        /// </summary>
        /// <param name="userIds">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf</param>
        /// <returns></returns>
        public async Task<int> SyncDcExProjects(List<long> userIds, long? excludeDeviceId = null)
        {
            if (userIds == null)
            {
                throw new ArgumentNullException(nameof(userIds));
            }

            var couter = 0;
            foreach (var userId in userIds)
            {
                var clientDevices = ClientConnection.GetClients().Where(w => w.UserId == userId);
                foreach (var device in clientDevices)
                {
                    if (excludeDeviceId.HasValue && excludeDeviceId.Value == device.DeviceId)
                    {
                        continue;
                    }

                    if (!device.ListLastSyncData.ContainsKey("DcExProjects"))
                    {
                        //Noch kein Sync
                        continue;
                    }

                    if (device.ListLastSyncData["DcExProjects"].DcListSyncData == null!)
                    {
                        continue;
                    }

                    var data = await _syncFunc("DcExProjects", device.DeviceId, device.UserId, device.ListLastSyncData["DcExProjects"].DcListSyncData!, device.ListLastSyncData["DcExProjects"].DcListSyncProperties!).ConfigureAwait(false);
                    var tmp2 = new DcListSyncResult("DcExProjects")
                               {
                                   Data = data
                               };
                    await SendInternal("DcExProjects", tmp2.ToJson(), device.DeviceId, device.UserId, excludeDeviceId, true).ConfigureAwait(false);
                    couter++;
                }
            }

            return couter;
        }


        /// <summary>
        /// Listen Daten an DcExRatings senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        [Obsolete("Bitte SyncDcExRatings verwenden!")]
        public async Task<int> SendDcExRatings(List<DcServerListItem<ExRating>> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            var tmp = data.Select(item => item.ToDcTransferListItem()).ToList();
            return await SendInternal("DcExRatings", tmp.ToJson(), deviceId, userId).ConfigureAwait(false);
        }

        /// <summary>
        /// Datenänderungen für DcExRatings an Client weiter geben
        /// </summary>
        /// <param name="data">Sync-Daten</param>
        /// <param name="deviceId">Gerät</param>
        /// <param name="userId">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> SyncDcExRatings(DcListSyncResultData<ExRating> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            if (data is IDcListSyncResultData result)
            {
                var tmp = result.ToDcListSyncTransferData();
                var tmp2 = new DcListSyncResult("DcExRatings")
                           {
                               Data = tmp
                           };

                return await SendInternal("DcExRatings", tmp2.ToJson(), deviceId, userId, excludeDeviceId, true).ConfigureAwait(false);
            }

            throw new ArgumentException("SyncDcExRatings has wrong Data");
        }

        /// <summary>
        /// Datenänderungen für DcExRatings an Client weiter geben
        /// </summary>
        /// <param name="userIds">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf</param>
        /// <returns></returns>
        public async Task<int> SyncDcExRatings(List<long> userIds, long? excludeDeviceId = null)
        {
            if (userIds == null)
            {
                throw new ArgumentNullException(nameof(userIds));
            }

            var couter = 0;
            foreach (var userId in userIds)
            {
                var clientDevices = ClientConnection.GetClients().Where(w => w.UserId == userId);
                foreach (var device in clientDevices)
                {
                    if (excludeDeviceId.HasValue && excludeDeviceId.Value == device.DeviceId)
                    {
                        continue;
                    }

                    if (!device.ListLastSyncData.ContainsKey("DcExRatings"))
                    {
                        //Noch kein Sync
                        continue;
                    }

                    if (device.ListLastSyncData["DcExRatings"].DcListSyncData == null!)
                    {
                        continue;
                    }

                    var data = await _syncFunc("DcExRatings", device.DeviceId, device.UserId, device.ListLastSyncData["DcExRatings"].DcListSyncData!, device.ListLastSyncData["DcExRatings"].DcListSyncProperties!).ConfigureAwait(false);
                    var tmp2 = new DcListSyncResult("DcExRatings")
                               {
                                   Data = data
                               };
                    await SendInternal("DcExRatings", tmp2.ToJson(), device.DeviceId, device.UserId, excludeDeviceId, true).ConfigureAwait(false);
                    couter++;
                }
            }

            return couter;
        }


        /// <summary>
        /// Listen Daten an DcExIncomeOutput senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        [Obsolete("Bitte SyncDcExIncomeOutput verwenden!")]
        public async Task<int> SendDcExIncomeOutput(List<DcServerListItem<ExIncomeOutput>> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            var tmp = data.Select(item => item.ToDcTransferListItem()).ToList();
            return await SendInternal("DcExIncomeOutput", tmp.ToJson(), deviceId, userId).ConfigureAwait(false);
        }

        /// <summary>
        /// Datenänderungen für DcExIncomeOutput an Client weiter geben
        /// </summary>
        /// <param name="data">Sync-Daten</param>
        /// <param name="deviceId">Gerät</param>
        /// <param name="userId">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> SyncDcExIncomeOutput(DcListSyncResultData<ExIncomeOutput> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            if (data is IDcListSyncResultData result)
            {
                var tmp = result.ToDcListSyncTransferData();
                var tmp2 = new DcListSyncResult("DcExIncomeOutput")
                           {
                               Data = tmp
                           };

                return await SendInternal("DcExIncomeOutput", tmp2.ToJson(), deviceId, userId, excludeDeviceId, true).ConfigureAwait(false);
            }

            throw new ArgumentException("SyncDcExIncomeOutput has wrong Data");
        }

        /// <summary>
        /// Datenänderungen für DcExIncomeOutput an Client weiter geben
        /// </summary>
        /// <param name="userIds">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf</param>
        /// <returns></returns>
        public async Task<int> SyncDcExIncomeOutput(List<long> userIds, long? excludeDeviceId = null)
        {
            if (userIds == null)
            {
                throw new ArgumentNullException(nameof(userIds));
            }

            var couter = 0;
            foreach (var userId in userIds)
            {
                var clientDevices = ClientConnection.GetClients().Where(w => w.UserId == userId);
                foreach (var device in clientDevices)
                {
                    if (excludeDeviceId.HasValue && excludeDeviceId.Value == device.DeviceId)
                    {
                        continue;
                    }

                    if (!device.ListLastSyncData.ContainsKey("DcExIncomeOutput"))
                    {
                        //Noch kein Sync
                        continue;
                    }

                    if (device.ListLastSyncData["DcExIncomeOutput"].DcListSyncData == null!)
                    {
                        continue;
                    }

                    var data = await _syncFunc("DcExIncomeOutput", device.DeviceId, device.UserId, device.ListLastSyncData["DcExIncomeOutput"].DcListSyncData!, device.ListLastSyncData["DcExIncomeOutput"].DcListSyncProperties!).ConfigureAwait(false);
                    var tmp2 = new DcListSyncResult("DcExIncomeOutput")
                               {
                                   Data = data
                               };
                    await SendInternal("DcExIncomeOutput", tmp2.ToJson(), device.DeviceId, device.UserId, excludeDeviceId, true).ConfigureAwait(false);
                    couter++;
                }
            }

            return couter;
        }


        /// <summary>
        /// Listen Daten an DcExAbos senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        [Obsolete("Bitte SyncDcExAbos verwenden!")]
        public async Task<int> SendDcExAbos(List<DcServerListItem<ExAbo>> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            var tmp = data.Select(item => item.ToDcTransferListItem()).ToList();
            return await SendInternal("DcExAbos", tmp.ToJson(), deviceId, userId).ConfigureAwait(false);
        }

        /// <summary>
        /// Datenänderungen für DcExAbos an Client weiter geben
        /// </summary>
        /// <param name="data">Sync-Daten</param>
        /// <param name="deviceId">Gerät</param>
        /// <param name="userId">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> SyncDcExAbos(DcListSyncResultData<ExAbo> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            if (data is IDcListSyncResultData result)
            {
                var tmp = result.ToDcListSyncTransferData();
                var tmp2 = new DcListSyncResult("DcExAbos")
                           {
                               Data = tmp
                           };

                return await SendInternal("DcExAbos", tmp2.ToJson(), deviceId, userId, excludeDeviceId, true).ConfigureAwait(false);
            }

            throw new ArgumentException("SyncDcExAbos has wrong Data");
        }

        /// <summary>
        /// Datenänderungen für DcExAbos an Client weiter geben
        /// </summary>
        /// <param name="userIds">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf</param>
        /// <returns></returns>
        public async Task<int> SyncDcExAbos(List<long> userIds, long? excludeDeviceId = null)
        {
            if (userIds == null)
            {
                throw new ArgumentNullException(nameof(userIds));
            }

            var couter = 0;
            foreach (var userId in userIds)
            {
                var clientDevices = ClientConnection.GetClients().Where(w => w.UserId == userId);
                foreach (var device in clientDevices)
                {
                    if (excludeDeviceId.HasValue && excludeDeviceId.Value == device.DeviceId)
                    {
                        continue;
                    }

                    if (!device.ListLastSyncData.ContainsKey("DcExAbos"))
                    {
                        //Noch kein Sync
                        continue;
                    }

                    if (device.ListLastSyncData["DcExAbos"].DcListSyncData == null!)
                    {
                        continue;
                    }

                    var data = await _syncFunc("DcExAbos", device.DeviceId, device.UserId, device.ListLastSyncData["DcExAbos"].DcListSyncData!, device.ListLastSyncData["DcExAbos"].DcListSyncProperties!).ConfigureAwait(false);
                    var tmp2 = new DcListSyncResult("DcExAbos")
                               {
                                   Data = data
                               };
                    await SendInternal("DcExAbos", tmp2.ToJson(), device.DeviceId, device.UserId, excludeDeviceId, true).ConfigureAwait(false);
                    couter++;
                }
            }

            return couter;
        }

        /// <summary>
        /// Listen Daten an DcExAbos senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        [Obsolete("Bitte SyncDcExMeasurementDefinitionAssignment verwenden!")]
        public async Task<int> SendDcExMeasurementDefinitionAssignment(List<DcServerListItem<ExMeasurementDefinitionAssignment>> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            var tmp = data.Select(item => item.ToDcTransferListItem()).ToList();
            return await SendInternal("DcExMeasurementDefinitionAssignment", tmp.ToJson(), deviceId, userId).ConfigureAwait(false);
        }

        /// <summary>
        /// Datenänderungen für DcExMeasurementDefinitionAssignment an Client weiter geben
        /// </summary>
        /// <param name="data">Sync-Daten</param>
        /// <param name="deviceId">Gerät</param>
        /// <param name="userId">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> SyncDcExMeasurementDefinitionAssignment(DcListSyncResultData<ExMeasurementDefinitionAssignment> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            if (data is IDcListSyncResultData result)
            {
                var tmp = result.ToDcListSyncTransferData();
                var tmp2 = new DcListSyncResult("DcExMeasurementDefinitionAssignment")
                           {
                               Data = tmp
                           };

                return await SendInternal("DcExAbos", tmp2.ToJson(), deviceId, userId, excludeDeviceId, true).ConfigureAwait(false);
            }

            throw new ArgumentException("SyncDcExAbos has wrong Data");
        }

        /// <summary>
        /// Datenänderungen für DcExAbos an Client weiter geben
        /// </summary>
        /// <param name="userIds">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf</param>
        /// <returns></returns>
        public async Task<int> SyncDcExMeasurementDefinitionAssignment(List<long> userIds, long? excludeDeviceId = null)
        {
            if (userIds == null)
            {
                throw new ArgumentNullException(nameof(userIds));
            }

            var couter = 0;
            foreach (var userId in userIds)
            {
                var clientDevices = ClientConnection.GetClients().Where(w => w.UserId == userId);
                foreach (var device in clientDevices)
                {
                    if (excludeDeviceId.HasValue && excludeDeviceId.Value == device.DeviceId)
                    {
                        continue;
                    }

                    if (!device.ListLastSyncData.ContainsKey("DcExMeasurementDefinitionAssignment"))
                    {
                        //Noch kein Sync
                        continue;
                    }

                    if (device.ListLastSyncData["DcExMeasurementDefinitionAssignment"].DcListSyncData == null!)
                    {
                        continue;
                    }

                    var data = await _syncFunc("DcExMeasurementDefinitionAssignment", device.DeviceId, device.UserId, device.ListLastSyncData["DcExMeasurementDefinitionAssignment"].DcListSyncData!, device.ListLastSyncData["DcExMeasurementDefinitionAssignment"].DcListSyncProperties!).ConfigureAwait(false);
                    var tmp2 = new DcListSyncResult("DcExMeasurementDefinitionAssignment")
                               {
                                   Data = data
                               };
                    await SendInternal("DcExMeasurementDefinitionAssignment", tmp2.ToJson(), device.DeviceId, device.UserId, excludeDeviceId, true).ConfigureAwait(false);
                    couter++;
                }
            }

            return couter;
        }


        /// <summary>
        /// Listen Daten an DcExNotifications senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        [Obsolete("Bitte SyncDcExNotifications verwenden!")]
        public async Task<int> SendDcExNotifications(List<DcServerListItem<ExNotification>> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            var tmp = data.Select(item => item.ToDcTransferListItem()).ToList();
            return await SendInternal("DcExNotifications", tmp.ToJson(), deviceId, userId).ConfigureAwait(false);
        }

        /// <summary>
        /// Datenänderungen für DcExNotifications an Client weiter geben
        /// </summary>
        /// <param name="data">Sync-Daten</param>
        /// <param name="deviceId">Gerät</param>
        /// <param name="userId">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> SyncDcExNotifications(DcListSyncResultData<ExNotification> data, long? deviceId = null, long? userId = null, long? excludeDeviceId = null)
        {
            if (data is IDcListSyncResultData result)
            {
                var tmp = result.ToDcListSyncTransferData();
                var tmp2 = new DcListSyncResult("DcExNotifications")
                           {
                               Data = tmp
                           };

                return await SendInternal("DcExNotifications", tmp2.ToJson(), deviceId, userId, excludeDeviceId, true).ConfigureAwait(false);
            }

            throw new ArgumentException("SyncDcExNotifications has wrong Data");
        }

        /// <summary>
        /// Datenänderungen für DcExNotifications an Client weiter geben
        /// </summary>
        /// <param name="userIds">Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf</param>
        /// <returns></returns>
        public async Task<int> SyncDcExNotifications(List<long> userIds, long? excludeDeviceId = null)
        {
            if (userIds == null)
            {
                throw new ArgumentNullException(nameof(userIds));
            }

            var couter = 0;
            foreach (var userId in userIds)
            {
                var clientDevices = ClientConnection.GetClients().Where(w => w.UserId == userId);
                foreach (var device in clientDevices)
                {
                    if (excludeDeviceId.HasValue && excludeDeviceId.Value == device.DeviceId)
                    {
                        continue;
                    }

                    if (!device.ListLastSyncData.ContainsKey("DcExNotifications"))
                    {
                        //Noch kein Sync
                        continue;
                    }

                    if (device.ListLastSyncData["DcExNotifications"].DcListSyncData == null!)
                    {
                        continue;
                    }

                    var data = await _syncFunc("DcExNotifications", device.DeviceId, device.UserId, device.ListLastSyncData["DcExNotifications"].DcListSyncData!, device.ListLastSyncData["DcExNotifications"].DcListSyncProperties!).ConfigureAwait(false);
                    var tmp2 = new DcListSyncResult("DcExNotifications")
                               {
                                   Data = data
                               };
                    await SendInternal("DcExNotifications", tmp2.ToJson(), device.DeviceId, device.UserId, excludeDeviceId, true).ConfigureAwait(false);
                    couter++;
                }
            }

            return couter;
        }


        /// <summary>
        /// Daten senden.
        /// Wenn deviceId und userId null sind werden die Daten an alle Geräte gesendet
        /// </summary>
        /// <param name="dpId">Datenpunkt Id</param>
        /// <param name="data">Daten in Json serialisiert</param>
        /// <param name="deviceId">An ein bestimmtes Gerät</param>
        /// <param name="userId">An einen bestimmten Benutzer</param>
        /// <param name="excludeDeviceId">An alle Geräte auf Dieses</param>
        /// <param name="sync">Neuer Sync Mode ab Version 8.x</param>
        /// <returns>Anzahl der erreichten Geräte</returns>
        private async Task<int> SendInternal(string dpId, string data, long? deviceId, long? userId, long? excludeDeviceId = null, bool sync = false)
        {
            var d = new DcResult(dpId, sync) {JsonData = data};
            d.Checksum = DcChecksum.Generate(d.JsonData);

            var result = 0;

            if (deviceId == null && userId == null)
            {
                result = await ClientConnection.SendData(d, excludeDeviceId).ConfigureAwait(false);
            }
            else if (userId != null)
            {
                result = await ClientConnection.SendDataToUser(userId.Value, d, excludeDeviceId).ConfigureAwait(false);
            }
            else if (deviceId != null)
            {
                if (await ClientConnection.SendDataToDevice(deviceId.Value, d).ConfigureAwait(false))
                {
                    result = 1;
                }
            }

            return result;
        }

        #endregion
    }
}