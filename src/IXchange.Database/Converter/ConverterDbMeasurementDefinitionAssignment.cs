// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.ObjectModel;
using System.Linq;
using BDA.Common.Exchange.Model.ConfigApp;
using Database.Converter;
using Exchange.Model.ConfigApp;
using IXchangeDatabase.Tables;

// ReSharper disable once CheckNamespace
namespace IXchangeDatabase.Converter
{
    /// <summary>
    /// <para>DESCRIPTION</para>
    /// Klasse ConverterDbMeasurementDefinitionAssignment. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class ConverterDbMeasurementDefinitionAssignment
    {
        /// <summary>
        ///     ExMeasurementDefinition nach TableMeasurementDefinition
        /// </summary>
        /// <param name="m"></param>
        /// <param name="t"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ToTableMeasurementDefinitionAssignment(this ExMeasurementDefinitionAssignment m, TableMeasurementDefinitionAssignment t)
        {
            if (t == null!)
            {
                throw new ArgumentNullException($"[{nameof(ConverterDbMeasurementDefinitionAssignment)}]({nameof(ToTableMeasurementDefinitionAssignment)}): {nameof(t)}");
            }

            if (m == null!)
            {
                throw new ArgumentNullException($"[{nameof(ConverterDbMeasurementDefinitionAssignment)}]({nameof(ToTableMeasurementDefinitionAssignment)}): {nameof(m)}");
            }

            //m.MeasurementDefinition.ToTableMeasurementDefinition(t.TblMeasurementDefinition);
            t.AccessForResearchInstitutesGranted = m.AccessForResearchInstitutesGranted;
            t.Type = m.Type;
            t.SendNotifications = m.SendNotifications;
            t.NotificationOnNewRating = m.NotificationOnNewRating;
            t.Id = m.Id;
            t.NotificationOnNewRating = m.NotificationOnNewRating;
            t.NotificationOnSubscription = m.NotificationOnSubscription;
            t.NotificationOnUnsubscription = m.NotificationOnUnsubscription;
        }


        /// <summary>
        ///     TableMeasurementDefinition nach ExMeasurementDefinition.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ExMeasurementDefinitionAssignment ToExMeasurementDefinitionAssignment(this TableMeasurementDefinitionAssignment t)
        {
            if (t == null!)
            {
                throw new ArgumentNullException($"[{nameof(ConverterDbMeasurementDefinitionAssignment)}]({nameof(ToExMeasurementDefinitionAssignment)}): {nameof(t)} is null");
            }

            var r = new ExMeasurementDefinitionAssignment
                    {
                        Id = t.Id,
                        //MeasurementDefinition = t.TblMeasurementDefinition.ToExMeasurementDefinition(),
                        AccessForResearchInstitutesGranted = t.AccessForResearchInstitutesGranted,
                        Type = t.Type,
                        SendNotifications = t.SendNotifications,
                        NotificationOnNewRating = t.NotificationOnNewRating,
                        NotificationOnSubscription = t.NotificationOnSubscription,
                        NotificationOnUnsubscription = t.NotificationOnUnsubscription
                    };

            r.Ratings = new ObservableCollection<ExRating>(t.TblRatings.Select(x => new ExRating
                                                                                    {
                                                                                        Id = x.Id,
                                                                                        Rating = x.Rating,
                                                                                        Description = x.Description,
                                                                                        TimeStamp = x.TimeStamp,
                                                                                        User = x.TblUser.ToExUser(),
                                                                                        //MeasurementDefinitionAssignment = r,
                                                                                    }));


            var exMes = new ExMeasurementDefinition
                        {
                            Id = t.TblMeasurementDefinition.Id,
                            AdditionalConfiguration = t.TblMeasurementDefinition.AdditionalConfiguration,
                            Information = t.TblMeasurementDefinition.Information.ToExInformation(),
                            ValueType = t.TblMeasurementDefinition.ValueType,
                            DownstreamType = t.TblMeasurementDefinition.DownstreamType,
                            MeasurementInterval = t.TblMeasurementDefinition.MeasurementInterval,
                            AdditionalProperties = t.TblMeasurementDefinition.AdditionalProperties,
                            //IotDeviceId = t.TblMeasurementDefinition.TblIotDeviceId,
                            //CompanyId = t.TblMeasurementDefinition.TblIoTDevice!.TblGateway!.TblCompany.Id
                        };

            r.MeasurementDefinition = exMes;

            return r;
        }
    }
}