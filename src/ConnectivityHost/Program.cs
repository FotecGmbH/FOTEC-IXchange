// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using ConnectivityHost.Helper;
using Exchange;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConnectivityHost
{
    /// <summary>
    ///     <para>Program</para>
    /// Klasse Program. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     Main
        /// </summary>
        /// <param name="args">Args</param>
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            try
            {
                var backgroundIxiesWorker = host.Services.CreateScope().ServiceProvider.GetService<BackgroundIxiesWorker>();
                backgroundIxiesWorker!.StartBackgroundWorker();

                var backgroundTriggerWorker = host.Services.CreateScope().ServiceProvider.GetService<BackgroundTriggerWorker>();
                backgroundTriggerWorker!.StartBackgroundWorker();
            }
            catch (Exception)
            {
                // ignored
            }


            host.Run();
        }

        /// <summary>
        ///     Host Builder erzeugen
        /// </summary>
        /// <param name="args">args</param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseUrls(AppSettings.Current().DcSignalHost).UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
                    webBuilder.ConfigureKestrel(options => { options.ConfigureEndpointDefaults(endpoints => { endpoints.Protocols = HttpProtocols.Http1AndHttp2; }); });
                });
        }
    }
}