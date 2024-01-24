// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Threading.Tasks;
using BDA.Common.Exchange.Enum;
using BDA.Common.Exchange.Model;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Dc.Client;
using Biss.Dc.Core;
using Exchange.Model.ConfigApp;
using ExLocalAppSettings = Exchange.Model.ExLocalAppSettings;
using ExUserPassword = Exchange.Model.ExUserPassword;


namespace BaseApp.Connectivity
{
    /// <summary>
    ///     <para>Datenconnector für das aktuelle Projekt</para>
    /// Klasse DcProjectBase. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class DcProjectBase : DcDataRoot
    {
        #region Properties

        /// <summary>
        ///     Infos vom Gerät, werden nur an den Server gesendet beim Start der App
        /// </summary>
        public DcDataPoint<ExDeviceInfo> DcExDeviceInfo { get; set; } = new DcDataPoint<ExDeviceInfo>(cacheDataPoint: false, takeDefaultInstance: true);

        /// <summary>
        ///     Benutzerinfos
        /// </summary>
        public DcDataPoint<ExUser> DcExUser { get; set; } = new DcDataPoint<ExUser>(EnumDcPointBehavior.LoadWhenNeeded, true, true);

        /// <summary>
        ///     Benutzerpasswort (bestehender User) ändern
        /// </summary>
        public DcDataPoint<ExUserPassword> DcExUserPassword { get; set; } = new DcDataPoint<ExUserPassword>();

        /// <summary>
        ///     Lokale App Daten
        /// </summary>
        public DcDataPoint<ExLocalAppSettings> DcExLocalAppData { get; set; } = new DcDataPoint<ExLocalAppSettings>(EnumDcPointBehavior.LocalOnly, takeDefaultInstance: true);

        /// <summary>
        ///     Einstellungen in der DB für Update Check, Allgemeine Meldung, ...
        /// </summary>
        public DcDataPoint<ExSettingsInDb> DcExSettingsInDb { get; set; } = new DcDataPoint<ExSettingsInDb>(EnumDcPointBehavior.LoadWhenNeeded, true, true, cacheDataPoint: false);

        /// <summary>
        /// Firmen.
        /// </summary>
        public DcDataList<DcListTypeCompany, ExCompany> DcExCompanies { get; } = new DcDataList<DcListTypeCompany, ExCompany>(false);

        /// <summary>
        /// Benutzer für Firmen
        /// </summary>
        public DcDataList<DcListDataPoint<ExCompanyUser>, ExCompanyUser> DcExCompanyUsers { get; } = new DcDataList<DcListDataPoint<ExCompanyUser>, ExCompanyUser>(false);

        /// <summary>
        /// Alle Gateways des User
        /// </summary>
        public DcDataList<DcListTypeGateway, ExGateway> DcExGateways { get; } = new DcDataList<DcListTypeGateway, ExGateway>(false);

        /// <summary>
        /// Alle Iot Devices des User
        /// </summary>
        public DcDataList<DcListTypeIotDevice, ExIotDevice> DcExIotDevices { get; } = new DcDataList<DcListTypeIotDevice, ExIotDevice>(false);

        /// <summary>
        /// Alle Messwertdefinitionen des User
        /// </summary>
        public DcDataList<DcListTypeMeasurementDefinition, ExMeasurementDefinition> DcExMeasurementDefinition { get; } = new DcDataList<DcListTypeMeasurementDefinition, ExMeasurementDefinition>(false);

        /// <summary>
        /// Alle Abos des User
        /// </summary>
        public DcDataList<DcListTypeMeasurementDefinitionAssignment, ExMeasurementDefinitionAssignment> DcExMeasurementDefinitionAssignments { get; } = new DcDataList<DcListTypeMeasurementDefinitionAssignment, ExMeasurementDefinitionAssignment>(false);

        /// <summary>
        /// Templates für die Konverter
        /// </summary>
        public DcDataList<DcListDataPoint<ExDataconverter>, ExDataconverter> DcExDataconverters { get; } = new DcDataList<DcListDataPoint<ExDataconverter>, ExDataconverter>();


        /// <summary>
        /// Globale Confings aller Firmen des User
        /// </summary>
        public DcDataList<DcListDataPoint<ExGlobalConfig>, ExGlobalConfig> DcExGlobalConfig { get; } = new DcDataList<DcListDataPoint<ExGlobalConfig>, ExGlobalConfig>(false);

        /// <summary>
        /// Alle Projekte eines User
        /// </summary>
        public DcDataList<DcListTypeProject, ExProject> DcExProjects { get; } = new DcDataList<DcListTypeProject, ExProject>(false);

        /// <summary>
        /// Alle Messwertdefinitionen des User
        /// </summary>
        public DcDataList<DcListTypeRating, ExRating> DcExRatings { get; } = new DcDataList<DcListTypeRating, ExRating>(false);

        /// <summary>
        /// Alle Einnahmen/Ausgaben des User
        /// </summary>
        public DcDataList<DcListTypeIncomeOutput, ExIncomeOutput> DcExIncomeOutput { get; } = new DcDataList<DcListTypeIncomeOutput, ExIncomeOutput>(false);

        /// <summary>
        /// Alle Abos des User
        /// </summary>
        public DcDataList<DcListTypeAbo, ExAbo> DcExAbos { get; } = new DcDataList<DcListTypeAbo, ExAbo>(false);

        /// <summary>
        /// Alle Benachrichtigungen des User
        /// </summary>
        public DcDataList<DcListTypeNotification, ExNotification> DcExNotifications { get; } = new DcDataList<DcListTypeNotification, ExNotification>(false);

        #endregion

        #region DcCommonCommands

        /// <summary>
        ///     Datei in der Datenbank und im Azure BlobStorage löschen
        /// </summary>
        /// <param name="fileId">Id der Datei aus der Datenbank</param>
        /// <returns></returns>
        public Task<DcCommonCommandResult> DeleteFile(long fileId)
        {
            return SendCommonData(new DcCommonData
                                  {
                                      Key = EnumDcCommonCommands.FileDelete.ToString(),
                                      Value = fileId.ToString()
                                  });
        }

        /// <summary>
        ///     Bestätigungs E-Mail erneut senden
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<DcCommonCommandResult> ResendAccessEMail(long userId)
        {
            return SendCommonData(new DcCommonData
                                  {
                                      Key = EnumDcCommonCommands.ResendAccessEMail.ToString(),
                                      Value = userId.ToString()
                                  });
        }

        /// <summary>
        ///     Passwort zurücksetzen starten
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<DcCommonCommandResult> ResetPassword(long userId)
        {
            return SendCommonData(new DcCommonData
                                  {
                                      Key = EnumDcCommonCommands.ResetPassword.ToString(),
                                      Value = userId.ToString()
                                  });
        }

        #endregion
    }
}