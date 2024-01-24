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
    /// <para>DbRating => ExRating</para>
    /// Klasse ConverterDbRating. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class ConverterDbRating
    {
        /// <summary>
        /// ToExRating
        /// </summary>
        /// <param name="tableRating">tableRating</param>
        /// <returns>ExRating</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ExRating ToExRating(this TableRating tableRating)
        {
            if (tableRating == null!)
            {
                throw new ArgumentNullException($"[{nameof(ConverterDbRating)}]({nameof(ToExRating)}): {nameof(tableRating)} is null");
            }

            return new ExRating
                   {
                       Id = tableRating.Id,
                       Rating = tableRating.Rating,
                       Description = tableRating.Description,
                       TimeStamp = tableRating.TimeStamp,
                       User = tableRating.TblUser.ToExUser(),
                       // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                       MeasurementDefinitionAssignment = tableRating.TblMeasurementDefinitionAssignment?.ToExMeasurementDefinitionAssignment()!,
                   };
        }

        /// <summary>
        /// UpdateTableRating
        /// </summary>
        /// <param name="tableRating">tableRating</param>
        /// <param name="exRating">exRating</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void UpdateTableRating(this TableRating tableRating, ExRating exRating)
        {
            if (tableRating == null)
            {
                throw new ArgumentNullException($"[{nameof(ConverterDbRating)}]({nameof(UpdateTableRating)}): {nameof(tableRating)}");
            }

            if (exRating == null!)
            {
                throw new ArgumentNullException($"[{nameof(ConverterDbRating)}]({nameof(UpdateTableRating)}): {nameof(exRating)}");
            }

            tableRating.Rating = exRating.Rating;
            tableRating.Description = exRating.Description;
            tableRating.TimeStamp = DateTime.UtcNow; //Todo: check ob immer auf aktuelles Datum upgedated werden soll (oder soll erstes Datum drin bleiben? (CreationDate))
        }
    }
}