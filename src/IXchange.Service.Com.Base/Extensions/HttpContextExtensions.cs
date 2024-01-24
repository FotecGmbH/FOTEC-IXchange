// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using BDA.Common.Exchange.Model;
using Biss.Log.Producer;
using Exchange.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IXchange.Service.Com.Base.Extensions
{
    /// <summary>
    /// <para>This class contains extension methods for the HttpContext class.</para>
    /// Klasse HttpContextExtensions. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool TryGetExUserFromHttpContext(this HttpContext context, out ExUser? user)
        {
            try
            {
                // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                if (context?.Items["User"] is ExUser exUser)
                {
                    user = exUser;
                    return true;
                }

                user = null;
                return false;
            }
            catch (InvalidCastException e)
            {
                Logging.Log.LogError($"{e}");
                user = null;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="researchInstitute"></param>
        /// <returns></returns>
        public static bool TryGetExResearchInstituteFromHttpContext(this HttpContext context, out ExResearchInstitute? researchInstitute)
        {
            try
            {
                // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                if (context?.Items["ResearchInstitute"] is ExResearchInstitute exResearchInstitute)
                {
                    researchInstitute = exResearchInstitute;
                    return true;
                }

                researchInstitute = null;
                return false;
            }
            catch (InvalidCastException e)
            {
                Logging.Log.LogError($"{e}");
                researchInstitute = null;
                return false;
            }
        }
    }
}