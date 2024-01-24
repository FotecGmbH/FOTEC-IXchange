// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using IXchange.Service.Com.Base.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IXchange.Service.Com.Base.Helpers
{
    /// <summary>
    ///     Klasse AuthorizeAttribute.cs
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class IXChangeAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        #region Interface Implementations

        /// <summary>
        ///     Authorize Attribut
        /// </summary>
        /// <param name="context">Kontext</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentException(null, nameof(context));
            }

            if (!context.HttpContext.TryGetExUserFromHttpContext(out _))
            {
                // not logged in
                context.Result = UserAccessControl.Unauthorized();
            }
        }

        #endregion
    }
}