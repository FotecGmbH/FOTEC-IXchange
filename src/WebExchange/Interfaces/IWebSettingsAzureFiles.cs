// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using Biss.Apps.Interfaces;

namespace WebExchange.Interfaces
{
    /// <summary>
    ///     <para>Interface for settings for Azure files</para>
    /// Interface IWebSettingsAzureFiles. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public interface IWebSettingsAzureFiles : IAppSettingsBase
    {
        #region Properties

        /// <summary>
        ///     Connection string für den Blob
        ///     azure -> Blob -> Zugriffsschlüssel -> Verbindungszeichenfolge
        /// </summary>
        string BlobConnectionString { get; }

        /// <summary>
        ///     Container Name im Blob
        ///     azure -> Blob -> Container -> Name (Check Zugriff public/private)
        /// </summary>
        string BlobContainerName { get; }

        /// <summary>
        ///     Cdn link oder public Bloblink für Filelink
        ///     azure -> Blob/Cdn -> Hostname
        /// </summary>
        string CdnLink { get; }

        #endregion
    }
}