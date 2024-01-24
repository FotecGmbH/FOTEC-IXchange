// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using BDA.Common.Exchange.Model.ConfigApp;
using Database.Common;
using Database.Tables;
using IXchange.Service.Com.Base;
using IXchange.Service.Com.Base.Helpers;
using NetTopologySuite.Geometries;

namespace IXchange.Service.Com.Rest.Helpers
{
    /// <summary>
    /// <para>This class contains a converter method for the Rest services.</para>
    /// Klasse ConverterHelper. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class ConverterHelper
    {
        /// <summary>
        ///     Converts ExRestMesurementResult to TblMesurementResult
        /// </summary>
        /// <param name="result">Mesurement Result</param>
        /// <param name="dId">Mesurement Definition ID</param>
        /// <returns></returns>
        public static TableMeasurementResult GetTableModel(this ExRestMeasurementResult result, long dId)
        {
            var tblResult = new TableMeasurementResult
                            {
                                Location = new DbPosition
                                           {
                                               Altitude = result.Location.Altitude,
                                               Latitude = result.Location.Latitude,
                                               Longitude = result.Location.Longitude,
                                               Precision = result.Location.Presision,
                                               Source = result.Location.Source,
                                               TimeStamp = result.Location.TimeStamp,
                                           },
                                SpatialPoint = new Point(result.Location.Longitude, result.Location.Latitude) {SRID = 4326},
                                Value = CommonMethodsHelper.GetValueOfMeasurementResult(result.Value, result.ValueType),
                                ValueType = result.ValueType,
                                AdditionalProperties = result.AdditionalProperties,
                                TblMeasurementDefinitionId = dId,
                                TimeStamp = result.TimeStamp == DateTime.MinValue ? DateTime.Now : result.TimeStamp,
                            };
            return tblResult;
        }

        /// <summary>
        /// ToExRestMeasurementResult
        /// </summary>
        /// <param name="tableMeasurementResult">tableMeasurementResult</param>
        /// <returns>ExRestMeasurementResult</returns>
        public static ExRestMeasurementResult ToExRestMeasurementResult(this TableMeasurementResult tableMeasurementResult)
        {
            return new ExRestMeasurementResult
                   {
                       Location = new ExPosition
                                  {
                                      Altitude = tableMeasurementResult.Location.Altitude,
                                      Latitude = tableMeasurementResult.Location.Latitude,
                                      Longitude = tableMeasurementResult.Location.Longitude,
                                      TimeStamp = tableMeasurementResult.Location.TimeStamp,
                                      Presision = tableMeasurementResult.Location.Precision,
                                      Source = tableMeasurementResult.Location.Source
                                  },
                       AdditionalProperties = tableMeasurementResult.AdditionalProperties,
                       Value = CommonMethodsHelper.GetValueOfMeasurementResult(tableMeasurementResult),
                       ValueType = tableMeasurementResult.ValueType,
                       TimeStamp = tableMeasurementResult.TimeStamp,
                       Id = tableMeasurementResult.Id
                   };
        }

        /// <summary>
        ///     Converts TblIoTDevice to ExIoTDevice
        /// </summary>
        /// <param name="device">IoT Device</param>
        /// <returns></returns>
        public static ExIotDevice GetTableIotDevice(this TableIotDevice device)
        {
            if (device == null!)
            {
                throw new ArgumentNullException();
            }

            ExIotDevice ioTDevice = new()
                                    {
                                        Id = device.Id,
                                        Information = new ExInformation
                                                      {
                                                          Name = device.Information.Name,
                                                          Description = device.Information.Description,
                                                          CreatedDate = device.Information.CreatedDate,
                                                          UpdatedDate = device.Information.UpdatedDate,
                                                      }
                                    };
            return ioTDevice;
        }
    }
}