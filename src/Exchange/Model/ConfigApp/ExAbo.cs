// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.ComponentModel;
using BDA.Common.Exchange.Model;
using Biss.Interfaces;

namespace Exchange.Model.ConfigApp
{
    /// <summary>
    /// <para>ExAbo</para>
    /// Klasse ExAbo. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExAbo : IBissModel
    {
        #region Properties

        /// <summary>
        ///     Id aus DB
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Abweichung von Mittelwert trigger ja nein
        /// </summary>
        public bool MovingAverageNotify { get; set; }

        /// <summary>
        /// Abweichung von Mittelwert trigger(Wert=bei welchem unterschied von mittelwert soll benachrichtigung passieren?)
        /// </summary>
        public float MovingAverageNotifyValue { get; set; }

        /// <summary>
        /// Übersteigung wert trigger ja nein
        /// </summary>
        public bool ExceedNotify { get; set; }

        /// <summary>
        /// Überschreitung Wert trigger
        /// </summary>
        public float ExceedNotifyValue { get; set; }

        /// <summary>
        /// Unterschreitung Wert trigger ja nein
        /// </summary>
        public bool UndercutNotify { get; set; }

        /// <summary>
        /// Unterschreitung Wert trigger
        /// </summary>
        public float UndercutNotifyValue { get; set; }

        /// <summary>
        /// Ausfall Zeit in Minuten trigger ja nein
        /// </summary>
        public bool FailureForMinutesNotify { get; set; }

        /// <summary>
        /// Ausfall Zeit in Minuten trigger
        /// </summary>
        public float FailureForMinutesNotifyValue { get; set; }

        /// <summary>
        ///     User des Abos
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ExUser User { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


        /// <summary>
        ///     Abonierter Messwert
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ExMeasurementDefinitionAssignment MeasurementDefinitionAssignment { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        #endregion

        #region Interface Implementations

#pragma warning disable CS0067
#pragma warning disable CS0414
        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
#pragma warning restore CS0414

        #endregion
    }
}