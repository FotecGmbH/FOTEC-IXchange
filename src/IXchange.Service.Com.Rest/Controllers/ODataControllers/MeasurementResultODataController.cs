// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System.Linq;
using IXchange.Service.Com.Base.Helpers;
using IXchangeDatabase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace IXchange.Service.Com.Rest.Controllers.ODataControllers
{
    /// <summary>
    /// MeasurementResultODataController
    /// </summary>
    public class MeasurementResultODataController : ODataController
    {
        private readonly Db _db;

        /// <summary>
        /// MeasurementResultODataController
        /// </summary>
        /// <param name="db">db</param>
        public MeasurementResultODataController(Db db)
        {
            _db = db;
        }

        /*MaxExpansionDepth=2 weil man sonst mit "?$expand=tblmeasurementdefinition($expand=tbliotdevice($expand=tblmeasurementdefinitions))" auch auf Messwerte
            von MesswertDefinitionen ohne "Freigabe für Forschungseinrichtungen" kommt
        */
        /// <summary>
        /// Get
        /// </summary>
        /// <returns>queryable</returns>
        [EnableQuery(MaxExpansionDepth = 2)]
        [HttpGet("api/odata/measurementresults")]
        [IXChangeODataAuthorize]
        public IQueryable /*<TableMeasurementResult>*/ Get()
        {
            var result = _db.TblMeasurementDefinitionAssignments
                .AsNoTracking()
                .Include(x => x.TblMeasurementDefinition)
                .ThenInclude(x => x.TblMeasurements)
                .Where(x => x.AccessForResearchInstitutesGranted)
                .Select(x => x.TblMeasurementDefinition)
                .SelectMany(x => x.TblMeasurements)
                //folgende Zeile derzeit notwendig, da SpatialPoint-Property beim Json-Serialisieren Error wirft 
                //Der Versuch das Property zu ignorieren (siehe Startup.cs) war nicht erfolgreich
                .Select(x => new {x.Id, x.TblMeasurementDefinitionId, x.Location, x.Value, x.AdditionalConfiguration, x.AdditionalProperties, x.TimeStamp, x.ValueType, x.TblMeasurementDefinition});

            return result;
        }
    }
}