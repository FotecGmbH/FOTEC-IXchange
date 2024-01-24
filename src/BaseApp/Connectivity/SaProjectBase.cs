// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Apps.Base.Connectivity.Model;
using Biss.Apps.Connectivity.Sa;
using Exchange.Model;


namespace BaseApp.Connectivity
{
    /// <summary>
    ///     <para>Projektbezogene REST Calls</para>
    /// Klasse SaProjectBase. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class SaProjectBase : RestAccessBase
    {
        /// <summary>
        /// SA REST Test Funktion
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <returns>erhaltene Daten</returns>
        public Task<ResultData<int>> AddAsGet(int a, int b)
        {
            return Wap.Get<int>("Add", new List<string> {a.ToString(new NumberFormatInfo()), b.ToString(new NumberFormatInfo())});
        }

        /// <summary>
        /// SA REST Test Funktion
        /// </summary>
        /// <param name="demo">demo daten</param>
        /// <returns>erhaltene daten</returns>
        public Task<ResultData<ExDemoData>> DemoDataAsPost(ExDemoData demo)
        {
            return Wap.Post<ExDemoData>("DemoData", demo);
        }

        /// <summary>
        /// Messwerte innerhalb eines Zeitraumes erhalten
        /// </summary>
        /// <param name="measurementDefinitionId">messwertdefinitions id</param>
        /// <param name="fromDate">von ZDatum</param>
        /// <param name="toDate"></param>
        /// <returns>Liste an ergebnissen</returns>
        public Task<ResultData<List<ExMeasurement>>> GetMeasurementsInPeriod(long measurementDefinitionId, DateTime fromDate, DateTime toDate)
        {
            return Wap.Get<List<ExMeasurement>>("measurementresult/timeperiod", new List<string> {measurementDefinitionId.ToString(), fromDate.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture), toDate.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)});
        }
    }
}