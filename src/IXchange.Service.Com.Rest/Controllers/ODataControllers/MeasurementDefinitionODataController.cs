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
    /// MeasurementDefinitionODataController
    /// </summary>
    public class MeasurementDefinitionODataController : ODataController
    {
        private readonly Db _db;

        /// <summary>
        /// MeasurementDefinitionODataController
        /// </summary>
        /// <param name="db">db</param>
        public MeasurementDefinitionODataController(Db db)
        {
            _db = db;
        }

        /*MaxExpansionDepth=1 weil man sonst mit "?$expand=tblIotDevice($expand=tblmeasurementdefinitions)" auch MesswertDefinitionen
            ohne "Freigabe für Forschungseinrichtungen" kommt
        */
        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        [EnableQuery(MaxExpansionDepth = 1)]
        [HttpGet("api/odata/measurementdefinitions")]
        [IXChangeODataAuthorize]
        public IQueryable<TableMeasurementDefinition> Get()
        {
            var result = _db.TblMeasurementDefinitionAssignments
                .AsNoTracking()
                .Include(x => x.TblMeasurementDefinition)
                .Where(x => x.AccessForResearchInstitutesGranted)
                .Select(x => x.TblMeasurementDefinition);

            return result;
        }
    }
}