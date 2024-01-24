// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using Exchange.Model.ConfigApp;
using IXchangeDatabase.Tables;

// ReSharper disable once CheckNamespace
namespace IXchangeDatabase.Converter
{
    /// <summary>
    /// <para>DbIncomeOutput => ExIncomeOutput</para>
    /// Klasse ConverterDbIncomeOutput. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class ConverterDbIncomeOutput
    {
        /// <summary>
        /// ToExIncomeOutput
        /// </summary>
        /// <param name="tableIncomeOutput">tableIncomeOutput</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ExIncomeOutput ToExIncomeOutput(this TableIncomeOutput tableIncomeOutput)
        {
            if (tableIncomeOutput == null!)
            {
                throw new ArgumentNullException($"[{nameof(ConverterDbIncomeOutput)}]({nameof(ToExIncomeOutput)}): {nameof(tableIncomeOutput)} is null");
            }

            var name = "-";

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (tableIncomeOutput.TblMeasurementDefinition != null && tableIncomeOutput.TblMeasurementDefinition.Information != null)
            {
                name = tableIncomeOutput.TblMeasurementDefinition.Information.Name;
            }

            return new ExIncomeOutput
                   {
                       Id = tableIncomeOutput.Id,
                       UserId = tableIncomeOutput.TblUserId,
                       //Description = tableIncomeOutput.Description,
                       TimeStamp = tableIncomeOutput.TimeStamp,
                       MeasurementDefinitionId = tableIncomeOutput.TblMeasurementDefinitonId ?? 0,
                       MeasurementDefinitionName = name,
                       Option = tableIncomeOutput.Option,
                       Ixies = tableIncomeOutput.Ixies,
                       CurrentTotalIxies = tableIncomeOutput.CurrentTotalIxies
                   };
        }

        /// <summary>
        /// UpdateTableIncomeOutput
        /// </summary>
        /// <param name="tableIncomeOutput">tableIncomeOutput</param>
        /// <param name="exIncomeOutput">exIncomeOutput</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void UpdateTableIncomeOutput(this TableIncomeOutput tableIncomeOutput, ExIncomeOutput exIncomeOutput)
        {
            //Todo: check ob notwendig??
            if (tableIncomeOutput == null)
            {
                throw new ArgumentNullException($"[{nameof(ConverterDbIncomeOutput)}]({nameof(UpdateTableIncomeOutput)}): {nameof(tableIncomeOutput)}");
            }

            if (exIncomeOutput == null!)
            {
                throw new ArgumentNullException($"[{nameof(ConverterDbIncomeOutput)}]({nameof(UpdateTableIncomeOutput)}): {nameof(exIncomeOutput)}");
            }

            tableIncomeOutput.Ixies = exIncomeOutput.Ixies;
            tableIncomeOutput.TblUserId = exIncomeOutput.UserId;
            tableIncomeOutput.CurrentTotalIxies = exIncomeOutput.CurrentTotalIxies;
            if (exIncomeOutput.MeasurementDefinitionId == 0)
            {
                tableIncomeOutput.TblMeasurementDefinitonId = null!;
            }
            else
            {
                tableIncomeOutput.TblMeasurementDefinitonId = exIncomeOutput.MeasurementDefinitionId;
            }

            tableIncomeOutput.Option = exIncomeOutput.Option;
            //tableIncomeOutput.Description = exIncomeOutput.Description;
            tableIncomeOutput.TimeStamp = DateTime.UtcNow;
        }
    }
}