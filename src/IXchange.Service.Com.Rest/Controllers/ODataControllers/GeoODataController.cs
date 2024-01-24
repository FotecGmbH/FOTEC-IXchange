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
using IXchange.Service.Com.Base;
using IXchange.Service.Com.Base.Helpers;
using IXchangeDatabase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace IXchange.Service.Com.Rest.Controllers.ODataControllers
{
    /// <summary>
    /// Geo data controller
    /// </summary>
    public class GeoODataController : ODataController
    {
        private readonly Db _db;

        /// <summary>
        /// Geo data controller
        /// </summary>
        /// <param name="db">db</param>
        public GeoODataController(Db db)
        {
            _db = db;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="lat">latitude</param>
        /// <param name="lng">langitude</param>
        /// <param name="radius">radius</param>
        /// <returns>geo result</returns>
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select | AllowedQueryOptions.Filter | AllowedQueryOptions.Count | AllowedQueryOptions.OrderBy)]
        [HttpGet("/api/odata/geo/area/{lat}/{lng}/{radius}")]
        [IXChangeODataAuthorize]
        public async Task<ActionResult<IEnumerable<ExRestGeoResult>>> Get(double lat = 47.83768085082139, double lng = 16.251903664907417, double radius = 500)
        {
            //if (!HttpContext.TryGetExUserFromHttpContext(out var user))
            //{
            //    return UserAccessControl.Unauthorized();
            //}

            var queryableMeasurements = GeoController.GetGeoResultsBaseQueryable(_db, lat, lng, radius);

            var res = await GeoController.GetRestGeoResults(queryableMeasurements, GeoController.GetSpatialPoint(lat, lng)).ConfigureAwait(false);

            var accessGrantedassignmentIds = await _db.TblMeasurementDefinitionAssignments.Where(x => x.AccessForResearchInstitutesGranted).Select(x => x.TblMeasurementDefinitionId).ToListAsync().ConfigureAwait(false);

            res = res.Where(x => accessGrantedassignmentIds.Contains(x.MeasurementDefinitionId)).ToList();

            return Ok(res);
        }
    }
}