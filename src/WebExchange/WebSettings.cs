// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using Biss;
using Biss.Apps.Service.Push;
using Biss.Attributes;
using WebExchange.Interfaces;

namespace WebExchange
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
    public class WebSettings :
        IWebSettingsAzureFiles,
        IAppServiceSettingPush,
        IAppSettingsDataBase,
        IAppSettingsEMail
    {
        private static WebSettings _current = null!;

        /// <summary>
        /// Get default Settings for WebSettings
        /// </summary>
        /// <returns></returns>
        public static WebSettings Current()
        {
            if (_current == null!)
            {
                _current = new WebSettings();
            }

            return _current;
        }

        #region IWebSettingsAzureFiles

        /// <summary>
        /// Connection string für den Blob
        /// </summary>
        [BissRequiredProperty]
        [BissSecureProperty]
        public string BlobConnectionString => BissSettingCsHelper.GetValue<string>(this, "NETIDEE.IXchange:WebExchange:BlobConnectionString");

        /// <summary>
        /// Container Name im Blob
        /// </summary>
        public string BlobContainerName => BissSettingCsHelper.GetValue<string>(this, "NETIDEE.IXchange:WebExchange:BlobContainerName");

        /// <summary>
        /// Cdn link oder public Bloblink für Filelink
        /// </summary>
        public string CdnLink => BissSettingCsHelper.GetValue<string>(this, "NETIDEE.IXchange:WebExchange:CdnLink");

        #endregion IWebSettingsAzureFiles

        #region IAppServiceSettingPush

        /// <summary>
        /// Push - Firebase Project Id - <inheritdoc cref="IAppServiceSettingPush.PushProjectId" />
        /// </summary>
        public string PushProjectId => BissSettingCsHelper.GetValue<string>(this, "NETIDEE.IXchange:WebExchange:PushProjectId");

        /// <summary>
        /// Push - Firebase Service Account Id - <inheritdoc cref="IAppServiceSettingPush.PushServiceAccountId" />
        /// </summary>
        public string PushServiceAccountId => BissSettingCsHelper.GetValue<string>(this, "NETIDEE.IXchange:WebExchange:PushServiceAccountId");

        /// <summary>
        /// Push - Firebase Private Key Id - <inheritdoc cref="IAppServiceSettingPush.PushPrivateKeyId" />
        /// </summary>
        public string PushPrivateKeyId => BissSettingCsHelper.GetValue<string>(this, "NETIDEE.IXchange:WebExchange:PushPrivateKeyId");

        /// <summary>
        /// Push - Firebase Private Key - <inheritdoc cref="IAppServiceSettingPush.PushPrivateKey" />
        /// </summary>
        public string PushPrivateKey => BissSettingCsHelper.GetValue<string>(this, "NETIDEE.IXchange:WebExchange:PushPrivateKey");

        #endregion IAppServiceSettingPush

        #region IAppSettingsDataBase

        public string ConnectionString => BissSettingCsHelper.GetValue<string>(this, "NETIDEE.IXchange:WebExchange:ConnectionString");

        /// <summary>
        /// Datenbank
        /// </summary>
        public string ConnectionStringDb => BissSettingCsHelper.GetValue<string>(this, "NETIDEE.IXchange:WebExchange:ConnectionStringDb");

        /// <summary>
        /// Datenbank-Server
        /// </summary>
        [BissRequiredProperty]
        [BissSecureProperty]
        public string ConnectionStringDbServer => BissSettingCsHelper.GetValue<string>(this, "NETIDEE.IXchange:WebExchange:ConnectionStringDbServer");

        /// <summary>
        /// Datenbank User
        /// </summary>
        [BissRequiredProperty]
        [BissSecureProperty]
        public string ConnectionStringUser => BissSettingCsHelper.GetValue<string>(this, "NETIDEE.IXchange:WebExchange:ConnectionStringUser");

        /// <summary>
        /// Datenbank User Passwort
        /// </summary>
        [BissRequiredProperty]
        [BissSecureProperty]
        public string ConnectionStringUserPwd => BissSettingCsHelper.GetValue<string>(this, "NETIDEE.IXchange:WebExchange:ConnectionStringUserPwd");

        #endregion IAppSettingsDataBase

        #region IAppSettingsEMail

        /// <summary>
        /// Als wer (E-Mail Adresse) wird gesendet (für Antworten)
        /// </summary>
        public string SendEMailAs => BissSettingCsHelper.GetValue<string>(this, "NETIDEE.IXchange:WebExchange:SendEMailAs");

        /// <summary>
        /// Welcher Name des Senders wird angezeigt
        /// </summary>
        public string SendEMailAsDisplayName => BissSettingCsHelper.GetValue<string>(this, "NETIDEE.IXchange:WebExchange:SendEMailAsDisplayName");

        /// <summary>
        /// Sendgrid Key (falls Sendgrid verwendet wird)
        /// </summary>
        public string SendGridApiKey => BissSettingCsHelper.GetValue<string>(this, "NETIDEE.IXchange:WebExchange:SendGridApiKey");

        #endregion IAppSettingsEMail
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
}