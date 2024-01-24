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
using BDA.Common.Exchange.Model;
using Exchange.Enum;
using IXchangeDatabase;
using IXchangeDatabase.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace IXchange.Service.Com.Base.Helpers
{
    /// <summary>
    ///     <para>Zugrifssberechtigung</para>
    /// Klasse AccessControl. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class UserAccessControl
    {
        /// <summary>
        /// ID der öffentlichen Firma
        /// </summary>
        public static int PublicCompanyId = 2;

        /// <summary>
        ///     Berechtigungsabfrage
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="readAndWrite">Les- und Schreibrechte</param>
        /// <param name="user">Benutzer</param>
        /// <param name="db">DB Kontext</param>
        /// <returns>Berechtigt oder nicht berechtigt</returns>
        public static async Task<bool> HasMeasurmentResultPermission(ExUser user, Db db, long id, bool readAndWrite = false)
        {
            if (db == null || user == null)
            {
                throw new ArgumentNullException(nameof(db));
            }

            if (!user.IsAdmin)
            {
                var tblMeasuremetResult = await db.TblMeasurementResults.FirstOrDefaultAsync(a => a.Id == id).ConfigureAwait(true);
                if (tblMeasuremetResult == null)
                {
                    return false;
                }

                var measurmentDefinitionId = tblMeasuremetResult.TblMeasurementDefinitionId;

                return await HasMeasurmentDefinitionPermission(user, db, measurmentDefinitionId, readAndWrite).ConfigureAwait(true);
            }

            return true;
        }

        /// <summary>
        ///     Berechtigungsabfrage
        /// </summary>
        /// <param name="user">Benutzer</param>
        /// <param name="db">DB Kontext</param>
        /// <param name="id">Id</param>
        /// <param name="readAndWrite">Les- und Schreibrechte</param>
        /// <returns></returns>
        public static async Task<bool> HasMeasurmentDefinitionPermission(ExUser user, Db db, long id, bool readAndWrite = false)
        {
            if (db == null || user == null)
            {
                throw new ArgumentNullException(nameof(db));
            }

            if (!user.IsAdmin)
            {
                try
                {
                    var measurement = await db.TblMeasurementDefinitions.AsNoTracking()
                        .Include(m => m.TblIoTDevice)
                        .ThenInclude(io => io.TblGateway)
                        .ThenInclude(g => g!.TblCompany)
                        .ThenInclude(c => c.TblPermissions)
                        //.Include(m=>m.TblAbos)
                        .FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(true);

                    var curAssign = db.TblMeasurementDefinitionAssignments.FirstOrDefault(mA => mA.TblMeasurementDefinition.Id == measurement!.Id);

                    if (curAssign == null)
                    {
                        db.TblMeasurementDefinitionAssignments.Add(new TableMeasurementDefinitionAssignment {TblMeasurementDefinitionId = measurement!.Id, Type = EnumMeasurementType.Other});
                        await db.SaveChangesAsync().ConfigureAwait(true);
                        curAssign = db.TblMeasurementDefinitionAssignments.FirstOrDefault(mA => mA.TblMeasurementDefinition.Id == measurement.Id);
                    }


                    if (measurement!.TblIoTDevice.TblGateway!.TblCompany.TblPermissions.FirstOrDefault()!.TblUserId == user.Id)
                    {
                        return true;
                    }

                    // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
                    if (curAssign!.TblAbos.Any(a => a.TblUserId == user.Id))
                    {
                        return true;
                    }

                    return false;
                }
                catch (Exception)
                {
                    return false;
                }


                //var tblMtoPAssignment = await db.TblMeasurementDefinitionToProjectAssignments.FirstOrDefaultAsync(a => a.TblMeasurementDefinitionAssignmentId == id).ConfigureAwait(true);
                //if (tblMtoPAssignment == null)
                //{
                //    return false;
                //}

                //var projectId = tblMtoPAssignment.TblProjctId;

                //return await HasProjectPermission(user, db, projectId!.Value, readAndWrite).ConfigureAwait(true);
            }

            return true;
        }

        /// <summary>
        ///     Berechtigungsabfrage
        /// </summary>
        /// <param name="user">Benutzer</param>
        /// <param name="db">DB Kontext</param>
        /// <param name="projectId">Projekt ID</param>
        /// <param name="readAndWrite">Les- und Schreibrechte</param>
        /// <returns></returns>
        public static async Task<bool> HasProjectPermission(ExUser user, Db db, long projectId, bool readAndWrite = false)
        {
            if (db == null)
            {
                throw new ArgumentException(null, nameof(db));
            }

            if (user == null)
            {
                throw new ArgumentException(null, nameof(user));
            }

            if (!user.IsAdmin)
            {
                var tblProject = await db.TblProjects.AsNoTracking().FirstOrDefaultAsync(a => a.Id == projectId).ConfigureAwait(true);
                if (tblProject == null)
                {
                    return false;
                }

                var companyId = tblProject.TblCompanyId;

                if (readAndWrite)
                {
                    if (!user.IsAdmInCompany(companyId) && !user.CanUserWriteInCompany(companyId))
                    {
                        return false;
                    }
                }
                else
                {
                    //Zugriff auf öffentliche Firma
                    if (companyId == PublicCompanyId)
                    {
                        return true;
                    }

                    if (!user.IsAdmInCompany(companyId) && !user.CanUserReadInCompany(companyId) && !user.CanUserWriteInCompany(companyId))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Nicht berechtigt
        /// </summary>
        /// <returns>Nicht berechtigt</returns>
        public static JsonResult Unauthorized() => new(new {message = "Unauthorized"}) {StatusCode = StatusCodes.Status401Unauthorized};
    }
}