// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using Biss.CsBuilder.Sql;
using Biss.Email;

namespace WebExchange
{
    /// <summary>
    ///     <para>Konstanten für WebProjekte</para>
    /// Klasse Constants. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class WebConstants
    {
        /// <summary>
        ///     Aktuelle WebSettings
        /// </summary>
        public static WebSettings CurrentWebSettings = WebSettings.Current();

        /// <summary>
        ///     DB Connection String
        /// </summary>
        public static string ConnectionString = string.IsNullOrEmpty(WebSettings.Current().ConnectionString)
            ? new CsBuilderSql(WebSettings.Current().ConnectionStringDbServer, WebSettings.Current().ConnectionStringDb, WebSettings.Current().ConnectionStringUser, WebSettings.Current().ConnectionStringUserPwd, SqlCommonStandardApplicationName.EntityFramework).ToString()
            : WebSettings.Current().ConnectionString;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string ConnectionStringPostGres = string.IsNullOrEmpty(WebSettings.Current().ConnectionString)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            ? $"Host={WebSettings.Current().ConnectionStringDbServer};Port=5432;Database={WebSettings.Current().ConnectionStringDb};Username={WebSettings.Current().ConnectionStringUser};Password={WebSettings.Current().ConnectionStringUserPwd}"
            : WebSettings.Current().ConnectionString;

        /// <summary>
        ///     Biss Email mit Sendgrid Key
        /// </summary>
        public static BissEMail Email = new BissEMail(new SendGridCredentials
                                                      {
                                                          ApiKeyV3 = WebSettings.Current().SendGridApiKey,
                                                      });
    }
}