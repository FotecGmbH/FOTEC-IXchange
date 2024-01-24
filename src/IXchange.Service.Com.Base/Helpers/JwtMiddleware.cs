// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Linq;
using System.Threading.Tasks;
using Database.Converter;
using IXchangeDatabase;
using IXchangeDatabase.Converter;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace IXchange.Service.Com.Base.Helpers
{
    /// <summary>
    /// Jwt Middleware
    /// </summary>
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Creates JwtMiddleware
        /// </summary>
        /// <param name="next"></param>
        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Aufruf von Framework
        /// </summary>
        /// <param name="context"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, Db db)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await AttachUserToContext(context, db, token).ConfigureAwait(false);
                await AttachResearchInstituteToContext(context, db, token).ConfigureAwait(false);
            }

            await _next(context).ConfigureAwait(false);
        }

        private async Task AttachUserToContext(HttpContext context, Db db, string token)
        {
            try
            {
                // attach user to context on successful jwt validation
                var accessToken = await db.TblAccessToken.FirstOrDefaultAsync(a => a.Token == token).ConfigureAwait(false);
                if (accessToken != null)
                {
                    var user = await db.TblUsers.Include(a => a.TblAccessToken)
                        .Include(a => a.TblDevices)
                        .Include(a => a.TblPermissions)
                        .FirstOrDefaultAsync(a => a.Id == accessToken.TblUserId).ConfigureAwait(false);

                    if (user != null)
                    {
                        context.Items["User"] = user.ToExUser();
                    }
                }
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }

        private async Task AttachResearchInstituteToContext(HttpContext context, Db db, string token)
        {
            try
            {
                var tableResearchInstitutesAccess = await db.TblResearchInstitutesAccess.FirstOrDefaultAsync(a => a.AccessToken == token).ConfigureAwait(false);
                if (tableResearchInstitutesAccess != null)
                {
                    context.Items["ResearchInstitute"] = tableResearchInstitutesAccess.ToExResearchInstitute();
                }
            }
            catch
            {
                // do nothing if token not found
                // researchInstitute is not attached to context so request won't have access to secure routes
            }
        }
    }
}