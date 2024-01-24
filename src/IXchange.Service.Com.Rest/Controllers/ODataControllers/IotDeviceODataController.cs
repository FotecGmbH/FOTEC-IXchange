// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System.Linq;
using Database.Tables;
using IXchange.Service.Com.Base.Helpers;
using IXchangeDatabase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace IXchange.Service.Com.Rest.Controllers.ODataControllers
{
    /// <summary>
    /// IotDeviceODataController
    /// </summary>
    public class IotDeviceODataController : ODataController
    {
        private readonly Db _db;

        /// <summary>
        /// IotDeviceODataController
        /// </summary>
        /// <param name="db">db</param>
        public IotDeviceODataController(Db db)
        {
            _db = db;
        }

        /*MaxExpansionDepth=1 weil man sonst mit "?$expand=tblmeasurementdefinitions($expand=tblmeasurements)" auch auf Messwerte
            von MesswertDefinitionen ohne "Freigabe für Forschungseinrichtungen" kommt
        */
        /// <summary>
        /// Get
        /// </summary>
        /// <returns>table iot device</returns>
        [EnableQuery(MaxExpansionDepth = 1)]
        [HttpGet("api/odata/Iotdevices")]
        [IXChangeODataAuthorize]
        public IQueryable<TableIotDevice> Get()
        {
            var result = _db.TblMeasurementDefinitionAssignments
                .AsNoTracking()
                .Include(x => x.TblMeasurementDefinition)
                .ThenInclude(x => x.TblIoTDevice)
                .Where(x => x.AccessForResearchInstitutesGranted)
                .Select(x => x.TblMeasurementDefinition.TblIoTDevice)
                .Distinct();
            return result;
        }
    }
}