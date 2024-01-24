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
    /// <para>Interface für CacheConverter</para>
    /// Interface IDbProjectCacheConverter. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public interface IDbProjectCacheConverter
    {
        #region Properties

        /// <summary>
        ///     Bewertungen.
        /// </summary>
        DbCacheConverterRating DbCacheRatings { get; }

        /// <summary>
        /// DbCacheIncomeOutputs
        /// </summary>
        DbCacheConverterIncomeOutputs DbCacheIncomeOutputs { get; }

        /// <summary>
        /// DbCaheAbos
        /// </summary>
        DbCacheConverterAbos DbCacheAbos { get; }

        #endregion
    }
}