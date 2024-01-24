// (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:56
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using BDA.Gateway.App.Core;
using Exchange;

namespace IXchange.Gateway
{
    /// <summary>
    /// Einstiegspunkt fuer Gateway Applikation
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Einstiegspunkt fuer Gateway Applikation
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task<int> Main(string[] args)
        {
            if (!args.Any())
            {
                args = new[] {"mode:simple", $"dcsignalhost:{AppSettings.Current().DcSignalHost}"};
            }

            return await GatewayAppEntryPoint.Start(args);
        }
    }
}