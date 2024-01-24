// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.Common.Exchange.Model.ConfigApp;
using IXchange.Service.Com.Base.Extensions;
using IXchange.Service.Com.Base.Helpers;
using IXchange.Service.Com.Rest.Helpers;
using IXchangeDatabase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IXchange.Service.Com.Rest.Controllers
{
    /// <summary>
    /// IoTDeviceController
    /// </summary>
    public class IoTDeviceController : Controller
    {
        /// <summary>
        ///     Datenbank Context
        /// </summary>
        private readonly Db _db;

        /// <summary>
        ///     ctor
        /// </summary>
        /// <param name="db">Database</param>
        public IoTDeviceController(Db db)
        {
            _db = db;
        }

        /// <summary>
        ///     Listet alle IoT Geräte die der Benutzer besitzt oder von dessen der Benutzer eine Messwerdefinition aboniert hat
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/iotdevice/list")]
        [IXChangeAuthorize]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> IotDeviceGet()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var isValidToken = HttpContext.TryGetExUserFromHttpContext(out var user);

            if (!isValidToken)
            {
                return UserAccessControl.Unauthorized();
            }

            if (user!.IsAdmin)
            {
                var iotDevices = new List<ExIotDevice>();
                foreach (var ioTDevice in _db.TblIotDevices)
                {
                    iotDevices.Add(ioTDevice.GetTableIotDevice());
                }

                return Ok(iotDevices);
            }

            var companyId = _db.TblPermissions.Where(a => a.TblUserId == user.Id).Select(a => a.TblCompanyId).FirstOrDefault();
            var permission = _db.TblGateways.Where(a => a.TblCompanyId == companyId).Select(a => a.TblIotDevices).FirstOrDefault();

            var fromAbos = _db.TblAbos
                .Include(a => a.TblMeasurementDefinitionAssignment)
                .ThenInclude(md => md.TblMeasurementDefinition)
                .ThenInclude(md => md.TblIoTDevice)
                .AsNoTracking()
                .Where(a => a.TblUserId == user.Id)
                .Select(a => a.TblMeasurementDefinitionAssignment.TblMeasurementDefinition.TblIoTDevice).ToList();

            foreach (var tableIotDevice in fromAbos)
            {
                tableIotDevice.AdditionalConfiguration = "nicht sichtbar fuer Abonennten";
            }

            permission = permission!.Concat(fromAbos).DistinctBy(p => p.Id).ToList();

            if (!permission.Any())
            {
                return UserAccessControl.Unauthorized();
            }

            return Ok(permission);
        }
    }
}