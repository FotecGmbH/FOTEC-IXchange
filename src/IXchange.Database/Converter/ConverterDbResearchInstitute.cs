// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using Exchange.Model;
using IXchangeDatabase.Tables;

// ReSharper disable once CheckNamespace
namespace IXchangeDatabase.Converter
{
    /// <summary>
    /// <para>Converter Forschungseinrichtungen</para>
    /// Klasse ConverterDbResearchInstitute. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class ConverterDbResearchInstitute
    {
        /// <summary>
        /// ToExResearchInstitute
        /// </summary>
        /// <param name="researchInstitutesAccess">researchInstitutesAccess</param>
        /// <returns>ExResearchInstitute</returns>
        public static ExResearchInstitute ToExResearchInstitute(this TableResearchInstitutesAccess researchInstitutesAccess)
        {
            return new ExResearchInstitute
                   {
                       Id = researchInstitutesAccess.Id,
                       Name = researchInstitutesAccess.ResearchInstituteName,
                       AdditionalData = researchInstitutesAccess.AdditionalData,
                       AccessToken = researchInstitutesAccess.AccessToken,
                       AccessValidFrom = researchInstitutesAccess.ValidFrom,
                       AccessValidUntil = researchInstitutesAccess.ValidUntil
                   };
        }
    }
}