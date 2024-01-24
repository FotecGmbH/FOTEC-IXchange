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
using System.Linq;
using System.Threading.Tasks;
using BaseApp.Connectivity;
using BDA.Common.Exchange.Model.ConfigApp;
using Exchange.Enum;
using Exchange.Model.ConfigApp;

namespace BaseApp.Helper
{
    /// <summary>
    /// <para>Helper Klasse fuer Messwertdefinitionen und deren Zuweisung</para>
    /// Klasse ExMesDefAssignHelper. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class ExMesDefAssignHelper
    {
        /// <summary>
        /// Zuweisung einer Messwertdefinition erhalten
        /// </summary>
        /// <param name="exMeasurement">Messwertdefinition</param>
        /// <param name="dc">Data connector</param>
        /// <returns>Zuweisung</returns>
        public static DcListTypeMeasurementDefinitionAssignment GetAssignment(ExMeasurementDefinition exMeasurement, DcProjectBase dc)
        {
            // ReSharper disable once RedundantSuppressNullableWarningExpression
            var curr = dc!.DcExMeasurementDefinitionAssignments.FirstOrDefault(mA => mA.Data.MeasurementDefinition.Id == exMeasurement.Id && mA.Id != 0);

            if (curr == null)
            {
                dc.DcExMeasurementDefinitionAssignments.Add(new DcListTypeMeasurementDefinitionAssignment(new ExMeasurementDefinitionAssignment
                                                                                                          {
                                                                                                              MeasurementDefinition = exMeasurement,
                                                                                                              Type = EnumMeasurementType.Other
                                                                                                          }));
                dc.DcExMeasurementDefinitionAssignments.StoreAll();

                curr = dc.DcExMeasurementDefinitionAssignments.FirstOrDefault(mA => mA.Data.MeasurementDefinition.Id == exMeasurement.Id);
            }

            return curr;
        }

        /// <summary>
        /// Zuweisung einer Messwertdefinition asynchron erhalten
        /// </summary>
        /// <param name="exMeasurement">Messwertdefinition</param>
        /// <param name="dc">Data connector</param>
        /// <returns>Zuweisung</returns>
        public static async Task<DcListTypeMeasurementDefinitionAssignment> GetAssignmentAsync(ExMeasurementDefinition exMeasurement, DcProjectBase dc)
        {
            var curr = dc.DcExMeasurementDefinitionAssignments.FirstOrDefault(mA => mA.Data.MeasurementDefinition.Id == exMeasurement.Id);

            if (curr == null)
            {
                dc.DcExMeasurementDefinitionAssignments.Add(new DcListTypeMeasurementDefinitionAssignment(new ExMeasurementDefinitionAssignment
                                                                                                          {
                                                                                                              MeasurementDefinition = exMeasurement,
                                                                                                              Type = EnumMeasurementType.Other
                                                                                                          }));
                await dc.DcExMeasurementDefinitionAssignments.StoreAll().ConfigureAwait(true);

                curr = dc.DcExMeasurementDefinitionAssignments.FirstOrDefault(mA => mA.Data.MeasurementDefinition.Id == exMeasurement.Id);
            }

            var counter = 0;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            while ((curr.Data.Id == 0 || curr.Data.MeasurementDefinition == null || curr.Data.MeasurementDefinition.Id == 0) && counter < 10)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                await dc.DcExMeasurementDefinitionAssignments.WaitDataFromServerAsync(reload: true).ConfigureAwait(true);
#pragma warning restore CS0618 // Type or member is obsolete
                curr = dc.DcExMeasurementDefinitionAssignments.FirstOrDefault(mA => mA.Data.MeasurementDefinition.Id == exMeasurement.Id);
                counter++;
            }

            return curr;
        }

        /// <summary>
        /// Messwertdefinition einer Zuweisung erhalten
        /// </summary>
        /// <param name="exMeasurementAssignment">Messwertdefinition-Zuweisung</param>
        /// <param name="dc">Data connector</param>
        /// <returns>Messwertdefinition</returns>
        public static DcListTypeMeasurementDefinition GetAssignment(ExMeasurementDefinitionAssignment exMeasurementAssignment, DcProjectBase dc)
        {
            var curr = dc!.DcExMeasurementDefinition.FirstOrDefault(mA => mA.Data.Id == exMeasurementAssignment.MeasurementDefinition.Id);

            return curr;
        }

        /// <summary>
        /// Alle Zuweisungen eines IoT Geraetes erhalten
        /// </summary>
        /// <param name="exIotDevice">IoT-Geraet</param>
        /// <param name="dc">Data connector</param>
        /// <returns>Messwertdefinition</returns>
        public static Task<List<DcListTypeMeasurementDefinitionAssignment>> GetAssignments(ExIotDevice exIotDevice, DcProjectBase dc)
        {
            // ReSharper disable once UnusedVariable
            IEnumerable<DcListTypeMeasurementDefinitionAssignment> currItemsMeasDefAssignments = dc!.DcExMeasurementDefinitionAssignments;
            // ReSharper disable once UnusedVariable
            IEnumerable<DcListTypeIotDevice> currentItems = dc.DcExIotDevices;
            throw new NotImplementedException();
        }
    }
}