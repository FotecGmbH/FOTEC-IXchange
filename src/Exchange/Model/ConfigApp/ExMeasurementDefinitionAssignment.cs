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
using System.ComponentModel;
using System.Linq;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Interfaces;
using Exchange.Enum;
using Exchange.Extensions;
using Exchange.Resources;
using Newtonsoft.Json;

namespace Exchange.Model.ConfigApp
{
    /// <summary>
    /// <para>
    /// Zwischentabelle / Zuweisung Measurementdefinition und ratings/notification, sowie fuer erweiterungen von BDA, wie
    /// Type
    /// </para>
    /// Klasse ExMeasurementDefinitionAssignment. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExMeasurementDefinitionAssignment : IBissModel
    {
        #region Properties

        /// <summary>
        /// Id aus DB
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Typ, z.b. feuchtigkeit, temperatur etc etc
        /// </summary>
        public EnumMeasurementType Type { get; set; }

        /// <summary>
        /// Typ als String
        /// </summary>
        public string TypeAsString => Type.GetDisplayName();

        /// <summary>
        /// Benachrichtigungen senden?
        /// </summary>
        public bool SendNotifications { get; set; }

        /// <summary>
        /// Benachrichtigung bei neuem Rating?
        /// </summary>
        public bool NotificationOnNewRating { get; set; }

        /// <summary>
        /// Benachrichtigung bei neuem Abo
        /// </summary>
        public bool NotificationOnSubscription { get; set; }

        /// <summary>
        /// Benachrichtigung bei "Deabonnierung"
        /// </summary>
        public bool NotificationOnUnsubscription { get; set; }

        /// <summary>
        /// Zugriff auf Daten für Forschungsinstitute gewährt
        /// </summary>
        public bool AccessForResearchInstitutesGranted { get; set; }

        /// <summary>
        /// Kosten (IXi's)
        /// </summary>
        [JsonIgnore]
        public double Costs
        {
            get
            {
                return TotalRatingInt switch
                {
                    <= 2 => 4,
                    <= 4 => 5,
                    _ => 6
                };
            }
        }

        /// <summary>
        /// Kosten (IXie's) als Textausgabe
        /// </summary>
        [JsonIgnore]
        public string CostsAsString => $"{Costs} {ResCommon.TxtIxiesPerDay}";

        /// <summary>
        /// Rezensionen
        /// </summary>
        public ObservableCollection<ExRating> Ratings { get; set; } = new();

        /// <summary>
        /// zugehoerige Messwertdefinition
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public virtual ExMeasurementDefinition MeasurementDefinition { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// Rating als Double (genau)
        /// </summary>
        [JsonIgnore]
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        public double TotalRatingDouble => !Ratings?.Any() ?? true ? 0 : Ratings.Average(x => x.Rating);

        /// <summary>
        /// Rating als integer (gerundet)
        /// </summary>
        [JsonIgnore]
        public int TotalRatingInt => (int) Math.Round(TotalRatingDouble, 0);

        /// <summary>
        /// Anzahl Bewertungen
        /// </summary>
        [JsonIgnore]
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        public int NumberOfRatings => Ratings?.Count ?? 0;

        /// <summary>
        /// Anzahl Bewertungen (Textausgabe)
        /// </summary>
        [JsonIgnore]
        public string NumberOfRatingsAsString => $"{NumberOfRatings} {(NumberOfRatings == 1 ? ResViewMyRatings.LblRating : ResViewMyRatings.LblRatings)}";

        #endregion

        #region Interface Implementations

#pragma warning disable CS0067
#pragma warning disable CS0414
        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged = null!;

#pragma warning restore CS0067
#pragma warning restore CS0414

        #endregion
    }
}