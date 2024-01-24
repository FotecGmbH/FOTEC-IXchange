// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System.ComponentModel.DataAnnotations;
using Exchange.Resources;

namespace Exchange.Enum
{
    /// <summary>
    /// <para>Type of the measurement</para>
    /// Klasse EnumMeasurementType. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public enum EnumMeasurementType
    {
        /// <summary>
        /// Feuchtigkeit
        /// </summary>
        [Display(Name = "EnumMeasurementType_Humidity", ResourceType = typeof(ResEnumTranslations))]
        Humidity,

        /// <summary>
        /// Temperatur
        /// </summary>
        [Display(Name = "EnumMeasurementType_Temperature", ResourceType = typeof(ResEnumTranslations))]
        Temperature,

        /// <summary>
        /// Luftdruck
        /// </summary>
        [Display(Name = "EnumMeasurementType_AirPressure", ResourceType = typeof(ResEnumTranslations))]
        AirPreasure,

        /// <summary>
        /// Lichtstaerke
        /// </summary>
        [Display(Name = "EnumMeasurementType_LightIntensity", ResourceType = typeof(ResEnumTranslations))]
        LightIntensity,

        /// <summary>
        /// Wind
        /// </summary>
        [Display(Name = "EnumMeasurementType_Wind", ResourceType = typeof(ResEnumTranslations))]
        Wind,

        /// <summary>
        /// Sonstige, keiner kategorie zugehoerige
        /// </summary>
        [Display(Name = "EnumMeasurementType_Other", ResourceType = typeof(ResEnumTranslations))]
        Other
    }
}