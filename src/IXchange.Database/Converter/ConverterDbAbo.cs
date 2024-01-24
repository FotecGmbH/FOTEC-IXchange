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
    /// <para>TableAbo => ExAbo</para>
    /// Klasse ConverterDbAbo. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class ConverterDbAbo
    {
        
        /// <summary>
        /// ToExAbo
        /// </summary>
        /// <param name="tableAbo">tableAbo</param>
        /// <returns>ExAbo</returns>
        public static ExAbo ToExAbo(this TableAbo tableAbo)
        {
            return new ExAbo
                   {
                       Id = tableAbo.Id,
                       ExceedNotify = tableAbo.ExceedNotify,
                       ExceedNotifyValue = tableAbo.ExceedNotifyValue,
                       FailureForMinutesNotify = tableAbo.FailureForMinutesNotify,
                       FailureForMinutesNotifyValue = tableAbo.FailureForMinutesNotifyValue,
                       MovingAverageNotify = tableAbo.MovingAverageNotify,
                       MovingAverageNotifyValue = tableAbo.MovingAverageNotifyValue,
                       UndercutNotify = tableAbo.UndercutNotify,
                       UndercutNotifyValue = tableAbo.UndercutNotifyValue,
                       MeasurementDefinitionAssignment = tableAbo.TblMeasurementDefinitionAssignment.ToExMeasurementDefinitionAssignment(), //tableAbo.TblMeasurementDefinitionAssignment.ToExMeasurementDefinition(),
                       User = tableAbo.TblUser.ToExUser()
                   };
        }

        /// <summary>
        /// UpdateTableAbo
        /// </summary>
        /// <param name="tableAbo">tableAbo</param>
        /// <param name="exAbo">exAbo</param>
        public static void UpdateTableAbo(this TableAbo tableAbo, ExAbo exAbo)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (tableAbo is null)
            {
                return;
            }

            tableAbo.TblMeasurementDefinitionAssignmentId = exAbo.MeasurementDefinitionAssignment.Id;
            tableAbo.ExceedNotify = exAbo.ExceedNotify;
            tableAbo.ExceedNotifyValue = exAbo.ExceedNotifyValue;
            tableAbo.FailureForMinutesNotify = exAbo.FailureForMinutesNotify;
            tableAbo.FailureForMinutesNotifyValue = exAbo.FailureForMinutesNotifyValue;
            tableAbo.MovingAverageNotify = exAbo.MovingAverageNotify;
            tableAbo.MovingAverageNotifyValue = exAbo.MovingAverageNotifyValue;
            tableAbo.UndercutNotify = exAbo.UndercutNotify;
            tableAbo.UndercutNotifyValue = exAbo.UndercutNotifyValue;
        }
    }
}