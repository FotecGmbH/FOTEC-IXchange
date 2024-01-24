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
    /// <para>Client anweisen eine Dc-Liste komplett neu zu landen</para>
    /// Klasse EnumReloadDcList. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public enum EnumReloadDcList
    {
        /// <summary>
        /// Aufforderung zum erneut Laden der Global Configs
        /// </summary>
        ExGlobalConfig,

        /// <summary>
        /// Aufforderung zum erneut Laden der Measurementdefinitions
        /// </summary>
        ExMeasurementDefinition,

        /// <summary>
        /// Aufforderung zum erneut Laden der IotDevices
        /// </summary>
        ExIotDevice,

        /// <summary>
        /// Aufforderung zum erneut Laden der Projekte
        /// </summary>
        ExProject,

        /// <summary>
        /// Aufforderung zum erneut Laden der CompanyUser
        /// </summary>
        ExCompanyUsers,

        /// <summary>
        /// Aufforderung zum erneut Laden der Gateways
        /// </summary>
        ExGateways,

        /// <summary>
        /// Aufforderung zum erneut Laden der Abos
        /// </summary>
        ExAbos,

        /// <summary>
        /// Aufforderung zum erneut Laden der IncomeOutputs
        /// </summary>
        ExIncomeOutputs,

        /// <summary>
        /// Aufforderung zum erneut Laden der Notifications
        /// </summary>
        ExNotifications,

        /// <summary>
        /// Aufforderung zum erneut Laden der Companys
        /// </summary>
        ExCompanys
    }
}