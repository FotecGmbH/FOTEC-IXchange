// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Biss.Interfaces;
using Exchange.Enum;
using Exchange.Resources;
using Newtonsoft.Json;

namespace Exchange.Model.ConfigApp
{
    /// <summary>
    /// <para>Einnahme/Ausgabe</para>
    /// Klasse ExIncomeOutput. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExIncomeOutput : IBissModel
    {
        #region Properties

        /// <summary>
        ///     Id aus DB
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Wie viele IXies war diese Einnahme/Ausgabe
        /// </summary>
        public int Ixies { get; set; }

        /// <summary>
        /// "Kontostand" nach dieser Buchung
        /// </summary>
        public int CurrentTotalIxies { get; set; }

        /// <summary>
        /// Zeitpunkt der Einnahme/Ausgabe
        /// </summary>
        public DateTime TimeStamp { get; set; }


        /// <summary>
        /// UserId
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// MeasurementDefinition id
        /// </summary>
        public long MeasurementDefinitionId { get; set; }

        /// <summary>
        /// MeasurementDefinition Name
        /// </summary>
        public string MeasurementDefinitionName { get; set; } = string.Empty;

        /// <summary>
        /// Option/grund
        /// </summary>
        public EnumIncomeOutputOption Option { get; set; }

        #endregion

        /// <summary>
        /// Beschreibungstext der Option der Klasse
        /// </summary>
        /// <returns>Beschreibungstext</returns>
        /// <exception cref="NotImplementedException"></exception>
        private string GetDescriptionString()
        {
            var resultString = string.Empty;

            switch (Option)
            {
                case EnumIncomeOutputOption.Abo:
                    resultString = resultString + "Abo von Sensor: ";
                    break;
                case EnumIncomeOutputOption.CreateIoTDevice:
                    resultString = resultString + "Sensor erstellt: ";
                    break;
                case EnumIncomeOutputOption.Rating:
                    resultString = resultString + "Bewertung von Sensor: ";
                    break;
                case EnumIncomeOutputOption.Transfer:
                    resultString = resultString + "Sensor sendet Daten: ";
                    break;
                case EnumIncomeOutputOption.CreateAccount:
                    resultString = resultString + "Account erstellt ";
                    break;
                default:
                    throw new NotImplementedException();
            }

            resultString = resultString + MeasurementDefinitionName;

            return resultString;
        }

        #region Interface Implementations

#pragma warning disable CS0067
#pragma warning disable CS0414
        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
#pragma warning restore CS0414

        #endregion

        #region JsonIgnore

        /// <summary>
        /// z.B. "Abo Sensor XY"
        /// </summary>
        [JsonIgnore]
        public string Description => GetDescriptionString();

        /// <summary>
        /// Ist es eine Einnahme oder ausgabe?
        /// Income=>True
        /// Output=>False
        /// </summary>
        [JsonIgnore]
        public bool IncomeOutput => Ixies >= 0;

        /// <summary>
        /// Ixies als formatted string
        /// </summary>
        [JsonIgnore]
        public string IxiesFormattedString => $"{Ixies:N0}";

        /// <summary>
        /// Ixies als formatted string (inkl. "Ixies")
        /// </summary>
        [JsonIgnore]
        public string IxiesCompleteString => $"{IxiesFormattedString} {ResCommon.TxtIxies}";

        /// <summary>
        /// Aktueller Kontostand: Ixies als formatted string
        /// </summary>
        [JsonIgnore]
        public string CurrentTotalIxiesFormattedString => $"{CurrentTotalIxies:N0}";

        /// <summary>
        /// Aktueller Kontostand: Ixies als formatted string (inkl. "Ixies")
        /// </summary>
        [JsonIgnore]
        public string CurrentTotalIxiesCompleteString => $"{CurrentTotalIxiesFormattedString} {ResCommon.TxtIxies}";

        /// <summary>
        /// Ixies als string (inkl. "Ixies")
        /// </summary>
        [JsonIgnore]
        public string IncomeOutputText => Ixies == 0 ? string.Empty : IncomeOutput ? ResCommon.TxtIncome : ResCommon.TxtSpending;

        #endregion
    }

    /// <summary>
    /// Extension klasse fuer ExIncomeOutput
    /// </summary>
    public static class ExIncomeOutputExtensions
    {
        /// <summary>
        /// derzeitiger Ixie stand erhalten
        /// </summary>
        /// <param name="incomeOutput">ausgehend von welchem incomeoutput</param>
        /// <param name="allIncomeOutputs">liste aller income outputs</param>
        /// <returns>derzeitiger Ixie stand</returns>
        public static int GetCurrentIxies(this ExIncomeOutput incomeOutput, IEnumerable<ExIncomeOutput> allIncomeOutputs)
        {
            return allIncomeOutputs
                //alle früheren Einnahmen/Ausgaben (Timestamp) und die zeitgleichen mit niedrigerer Id (z.b. 2 gleiche wo nur Datum ohne Zeit (0 Uhr) angegeben) 
                .Where(x => x.TimeStamp < incomeOutput.TimeStamp || (x.TimeStamp == incomeOutput.TimeStamp && x.Id < incomeOutput.Id))
                .Select(x => x.Ixies)
                .DefaultIfEmpty()
                .Sum() + incomeOutput.Ixies;
        }
    }
}