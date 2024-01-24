﻿// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Globalization;
using System.Linq;
using BDA.Common.Exchange.Enum;
using Database.Common;
using Database.Tables;

namespace IXchange.Service.Com.Base.Helpers
{
    /// <summary>
    /// <para>This class contains helper methods to be used for both REST and gRPC interface</para>
    /// Klasse CommonMethodsHelper. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class CommonMethodsHelper
    {
        /// <summary>
        /// Wert umwandeln
        /// </summary>
        /// <param name="mr">Wert mit Wertetyp</param>
        /// <returns>Wert als String</returns>
        public static string GetValueOfMeasurementResult(TableMeasurementResult mr)
        {
            var value = mr.Value;
            var valueType = mr.ValueType;

            switch (valueType)
            {
                case EnumValueTypes.Number:
                    if (value.Number != null)
                    {
                        return value.Number.Value.ToString(CultureInfo.InvariantCulture);
                    }

                    break;
                case EnumValueTypes.Data:
                case EnumValueTypes.Image:
                    if (value.Binary is {Length: > 0})
                    {
                        return Convert.ToBase64String(value.Binary);
                    }

                    break;
                case EnumValueTypes.Text:
                    return value.Text;
                case EnumValueTypes.Bit:
                    if (value.Bit != null)
                    {
                        return value.Bit.Value.ToString();
                    }

                    break;
                default:
                    return value.Text;
            }

            return "";
        }


        /// <summary>
        /// Get value of measurement result
        /// </summary>
        /// <param name="mrValue">mr value</param>
        /// <param name="valueType">value type</param>
        /// <returns>db value</returns>
        public static DbValue GetValueOfMeasurementResult(string mrValue, EnumValueTypes valueType)
        {
            switch (valueType)
            {
                case EnumValueTypes.Number:
                    if (double.TryParse(mrValue, out var nrResult))
                    {
                        return new DbValue
                               {
                                   Number = nrResult,
                               };
                    }

                    if (double.TryParse(mrValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var nrResultInvariant))
                    {
                        return new DbValue
                               {
                                   Number = nrResultInvariant,
                               };
                    }

                    break;
                case EnumValueTypes.Data:
                case EnumValueTypes.Image:
                    return new DbValue
                           {
                               Binary = Convert.FromBase64String(mrValue),
                           };
                case EnumValueTypes.Text:
                    return new DbValue
                           {
                               Text = mrValue,
                           };
                case EnumValueTypes.Bit:
                    if (bool.TryParse(mrValue, out var bResult))
                    {
                        return new DbValue
                               {
                                   Bit = bResult,
                               };
                    }

                    break;
                default:
                    return new DbValue
                           {
                               Text = mrValue,
                           };
            }

            return new DbValue();
        }

        /// <summary>
        /// Apply order by on measurement results
        /// </summary>
        /// <param name="tblMeasurementResults">table measurementresults</param>
        /// <param name="orderBy">order by</param>
        /// <returns>tbale measurementresult</returns>
        public static IQueryable<TableMeasurementResult> ApplyOrderbyOnMeasurementResults(this IQueryable<TableMeasurementResult> tblMeasurementResults, string orderBy)
        {
            if (!string.IsNullOrEmpty(orderBy))
            {
                orderBy = orderBy.ToLower();

                var orderbySplitted = orderBy.Split(' ');

                var orderByAsc = !(orderbySplitted.Length > 1 && orderbySplitted[1] == "desc");

                switch (orderbySplitted[0])
                {
                    case "timestamp":
                        tblMeasurementResults = orderByAsc ? tblMeasurementResults.OrderBy(a => a.TimeStamp) : tblMeasurementResults.OrderByDescending(a => a.TimeStamp);
                        break;
                    case "id":
                        tblMeasurementResults = orderByAsc ? tblMeasurementResults.OrderBy(a => a.Id) : tblMeasurementResults.OrderByDescending(a => a.Id);
                        break;
                }
            }

            return tblMeasurementResults;
        }
    }
}