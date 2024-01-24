// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;

// ReSharper disable once CheckNamespace
namespace ConnectivityHost.CacheConverter
{
    /// <summary>
    /// <para>DbProjectCacheConverter</para>
    /// Klasse DbProjectCacheConverter. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class DbProjectCacheConverter : IDbProjectCacheConverter
    {
        #region Properties

        /// <summary>
        /// Cache Converter für Bewertungen
        /// </summary>
        public DbCacheConverterRating DbCacheRatings { get; } = new();

        /// <summary>
        /// DbCacheIncomeOutputs
        /// </summary>
        public DbCacheConverterIncomeOutputs DbCacheIncomeOutputs { get; } = new();

        /// <summary>
        /// DbCacheAbos
        /// </summary>
        public DbCacheConverterAbos DbCacheAbos { get; } = new();

        #endregion
    }
}