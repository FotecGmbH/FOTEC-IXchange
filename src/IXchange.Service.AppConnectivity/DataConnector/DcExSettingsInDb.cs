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
using Biss.Dc.Core;
using IXchange.Service.AppConnectivity.Helper;
using ExLocalAppSettings = Exchange.Model.ExLocalAppSettings;

namespace IXchange.Service.AppConnectivity.DataConnector
{
    /// <summary>
    ///     <para>Allgemeine DB Einstellungen</para>
    /// Klasse ServerRemoteCalls. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public partial class ServerRemoteCalls
    {
        #region Interface Implementations

        /// <summary>
        ///     Device fordert Daten für DcExLocalAppData
        /// </summary>
        /// <param name="deviceId">Id des Gerätes</param>
        /// <param name="userId">Id des Benutzers oder -1 wenn nicht angemeldet</param>
        /// <returns>Daten oder eine Exception auslösen</returns>
        public Task<ExLocalAppSettings> GetDcExLocalAppData(long deviceId, long userId)
        {
            throw new NotImplementedException($"[DcExSettingsInDb]({nameof(GetDcExLocalAppData)}): not implemented");
        }

        /// <summary>
        ///     Device will Daten für DcExNewUserData sichern
        /// </summary>
        /// <param name="deviceId">Id des Gerätes</param>
        /// <param name="userId">Id des Benutzers oder -1 wenn nicht angemeldet</param>
        /// <param name="data">Eingetliche Daten</param>
        /// <returns>Ergebnis (bzw. Infos zum Fehler)</returns>
        public Task<DcStoreResult> SetDcExLocalAppData(long deviceId, long userId, ExLocalAppSettings data)
        {
            throw new NotImplementedException($"[DcExSettingsInDb]({nameof(SetDcExLocalAppData)}): not implemented");
        }

        /// <summary>
        ///     Device fordert Daten für DcExSettingsInDb
        /// </summary>
        /// <param name="deviceId">Id des Gerätes</param>
        /// <param name="userId">Id des Benutzers oder -1 wenn nicht angemeldet</param>
        /// <returns>Daten oder eine Exception auslösen</returns>
        public Task<ExSettingsInDb> GetDcExSettingsInDb(long deviceId, long userId)
        {
            return Task.FromResult(CurrentSettingsInDb.Current);
        }

        /// <summary>
        ///     Device will Daten für DcExNewUserData sichern
        /// </summary>
        /// <param name="deviceId">Id des Gerätes</param>
        /// <param name="userId">Id des Benutzers oder -1 wenn nicht angemeldet</param>
        /// <param name="data">Eingetliche Daten</param>
        /// <returns>Ergebnis (bzw. Infos zum Fehler)</returns>
        public Task<DcStoreResult> SetDcExSettingsInDb(long deviceId, long userId, ExSettingsInDb data)
        {
            throw new NotImplementedException($"[DcExSettingsInDb]({nameof(SetDcExSettingsInDb)}): not implemented");
        }

        #endregion
    }
}