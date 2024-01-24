// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using Database.Converter;
using Exchange.Model.ConfigApp;
using IXchangeDatabase.Tables;

// ReSharper disable once CheckNamespace
namespace IXchangeDatabase.Converter
{
    /// <summary>
    /// <para>TableNotification => ExNotification</para>
    /// Klasse ConverterDbNotification. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class ConverterDbNotification
    {
        /// <summary>
        /// ToExNotification
        /// </summary>
        /// <param name="tableNotification">tableNotification</param>
        /// <returns>ExNotification</returns>
        public static ExNotification ToExNotification(this TableNotification tableNotification)
        {
            return new ExNotification
                   {
                       Id = tableNotification.Id,
                       Description = tableNotification.Description,
                       NotificationType = tableNotification.NotificationType,
                       TimeStamp = tableNotification.TimeStamp,
                       User = tableNotification.TblUser.ToExUser(),
#pragma warning disable CA1062
                       MeasurementDefinitionAssignment = tableNotification.TblMeasurementDefinitionAssignment.ToExMeasurementDefinitionAssignment()
#pragma warning restore CA1062
                   };
        }

        /// <summary>
        /// UpdateTableNotification
        /// </summary>
        /// <param name="tableNotification">tableNotification</param>
        /// <param name="exNotification">exNotification</param>
        public static void UpdateTableNotification(this TableNotification tableNotification, ExNotification exNotification)
        {
            tableNotification.TimeStamp = exNotification.TimeStamp;
            tableNotification.Description = exNotification.Description;
            tableNotification.NotificationType = exNotification.NotificationType;
        }
    }
}