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
using BDA.Common.Exchange.Enum;
using Database.Tables;
using IXchange.Service.Com.Base;
using IXchange.Service.Com.Rest.Enums;
using IXchangeDatabase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;


namespace IXchange.Service.Com.Rest.Controllers;

/// <summary>
/// <para>Geoabfragen</para>
/// Klasse GeoController.cs (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
/// </summary>
public class GeoController : Controller
{
    /// <summary>
    /// Datenbank Context
    /// </summary>
    // ReSharper disable once NotAccessedField.Local
    private readonly Db _db;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="db">Database</param>
    public GeoController(Db db)
    {
        _db = db;
    }

    /// <summary>
    /// GetGeoResults
    /// </summary>
    /// <param name="db">db</param>
    /// <param name="lat">latitude</param>
    /// <param name="lng">langitude</param>
    /// <param name="radius">radius</param>
    /// <param name="altMin">altMin</param>
    /// <param name="altMax">altMax</param>
    /// <param name="take">take</param>
    /// <param name="skip">skip</param>
    /// <param name="valueType">valueType</param>
    /// <param name="filterAdditionalProperties">filterAdditionalProperties</param>
    /// <param name="timeStampFrom">timeStampFrom</param>
    /// <param name="timeStampTo">timeStampTo</param>
    /// <returns>ExRestGeoResult</returns>
    public static async Task<List<ExRestGeoResult>> GetGeoResults(Db db, double lat = 47.83768085082139, double lng = 16.251903664907417, double radius = 500, double altMin = 0, double altMax = 0, int take = 20, int skip = 0, EnumQueryValueTypes valueType = EnumQueryValueTypes.All, string filterAdditionalProperties = "", DateTime? timeStampFrom = null, DateTime? timeStampTo = null)
    {
        var sourceSpatialPoint = GetSpatialPoint(lat, lng);

        return await GetGeoResults(db, sourceSpatialPoint, radius, altMin, altMax, take, skip, valueType, filterAdditionalProperties, timeStampFrom, timeStampTo).ConfigureAwait(false);
    }

    /// <summary>
    /// GetGeoResults
    /// </summary>
    /// <param name="db">db</param>
    /// <param name="sourceSpatialPoint"></param>
    /// <param name="radius">radius</param>
    /// <param name="altMin">altMin</param>
    /// <param name="altMax">altMax</param>
    /// <param name="take">take</param>
    /// <param name="skip">skip</param>
    /// <param name="valueType">valueType</param>
    /// <param name="filterAdditionalProperties">filterAdditionalProperties</param>
    /// <param name="timeStampFrom">timeStampFrom</param>
    /// <param name="timeStampTo">timeStampTo</param>
    /// <returns>ExRestGeoResult</returns>
    public static async Task<List<ExRestGeoResult>> GetGeoResults(Db db, Point sourceSpatialPoint, double radius = 500, double altMin = 0, double altMax = 0, int take = 20, int skip = 0, EnumQueryValueTypes valueType = EnumQueryValueTypes.All, string filterAdditionalProperties = "", DateTime? timeStampFrom = null, DateTime? timeStampTo = null)
    {
        var queryAbleMeasurments = GetGeoResultsBaseQueryable(db, sourceSpatialPoint, radius);

        //Einschränkung auf Wertetyp
        if (valueType != EnumQueryValueTypes.All)
        {
            queryAbleMeasurments = queryAbleMeasurments.Where(a => a.ValueType == (EnumValueTypes) valueType);
        }

        //Filter auf zusätzliche Properties
        if (!String.IsNullOrEmpty(filterAdditionalProperties))
        {
            queryAbleMeasurments = queryAbleMeasurments.Where(a => a.AdditionalProperties.Contains(filterAdditionalProperties));
        }

        //Filter Zeitstempel
        if (timeStampFrom != null)
        {
            queryAbleMeasurments = queryAbleMeasurments.Where(a => a.TimeStamp >= timeStampFrom.Value);
        }

        if (timeStampTo != null)
        {
            queryAbleMeasurments = queryAbleMeasurments.Where(a => a.TimeStamp <= timeStampTo.Value);
        }

        //Filter Höhe
        if (altMin != 0 && altMax != 0 && altMin <= altMax)
        {
            queryAbleMeasurments = queryAbleMeasurments.Where(a => a.Location.Altitude <= altMax && a.Location.Altitude >= altMin);
        }

        queryAbleMeasurments = queryAbleMeasurments.Skip(skip).Take(take);

        var lstResult = await GetRestGeoResults(queryAbleMeasurments, sourceSpatialPoint).ConfigureAwait(false);

        //lstResult = lstResult.OrderBy(a => a.Distance).ToList();

        return lstResult;
    }

    /// <summary>
    /// GetGeoResultsBaseQueryable
    /// </summary>
    /// <param name="db">db</param>
    /// <param name="lat">latitude</param>
    /// <param name="lng">langitude</param>
    /// <param name="radius">radius</param>
    /// <returns>TableMeasurementResult</returns>
    public static IQueryable<TableMeasurementResult> GetGeoResultsBaseQueryable(Db db, double lat = 47.83768085082139, double lng = 16.251903664907417, double radius = 500)
    {
        var sourceSpatialPoint = GetSpatialPoint(lat, lng);

        return GetGeoResultsBaseQueryable(db, sourceSpatialPoint, radius);
    }

    /// <summary>
    /// GetGeoResultsBaseQueryable
    /// </summary>
    /// <param name="db">db</param>
    /// <param name="sourceSpatialPoint"></param>
    /// <param name="radius">radius</param>
    /// <returns>TableMeasurementResult</returns>
    public static IQueryable<TableMeasurementResult> GetGeoResultsBaseQueryable(Db db, Point sourceSpatialPoint, double radius = 500)
    {
        //Queryalbe -> Hat den Vorteil, dass man nicht alles verschachteln muss
        var queryAbleMeasurments = db.TblMeasurementResults.AsQueryable().AsNoTracking();

        queryAbleMeasurments = queryAbleMeasurments.Include(x => x.TblMeasurementDefinition);

        if (radius > 0) //Filter mit Radius          groeßte einschraenkung
        {
            queryAbleMeasurments = queryAbleMeasurments.Where(a => a.SpatialPoint.Distance(sourceSpatialPoint) <= radius);
        }

        //Filter auf Projekt
        //if (projectId != -1)
        //{
        //    var tblMeasurmentDefinitionIds = db.TblMeasurementDefinitionToProjectAssignments
        //        .Where(aaa => aaa.TblProjctId == projectId).Select(aa => aa.TblMeasurementDefinitionAssignmentId);

        //    queryAbleMeasurments = queryAbleMeasurments.Where(a => tblMeasurmentDefinitionIds.Contains(a.TblMeasurementDefinitionAssignmentId));
        //}

        return queryAbleMeasurments;
    }

    /// <summary>
    /// GetRestGeoResults
    /// </summary>
    /// <param name="queryAbleMeasurments">queryAbleMeasurments</param>
    /// <param name="sourceSpatialPoint">sourceSpatialPoint</param>
    /// <returns>List ExRestGeoResult </returns>
    public static async Task<List<ExRestGeoResult>> GetRestGeoResults(IQueryable<TableMeasurementResult> queryAbleMeasurments, Point sourceSpatialPoint)
    {
        var lstResult = await queryAbleMeasurments
            .Select(a => new ExRestGeoResult(
                a.Id,
                a.TblMeasurementDefinitionId,
                a.TimeStamp,
                a.SpatialPoint.Distance(sourceSpatialPoint),
                a.Value.Binary,
                a.Value.Text,
                a.Value.Number,
                a.Value.Bit,
                a.ValueType,
                a.Location.Latitude,
                a.Location.Longitude,
                a.Location.Altitude,
                a.AdditionalProperties
            ))
            .ToListAsync().ConfigureAwait(false);

        return lstResult;
    }

    /// <summary>
    /// GetSpatialPoint
    /// </summary>
    /// <param name="lat">latitude</param>
    /// <param name="lng">longitude</param>
    /// <returns>Point</returns>
    public static Point GetSpatialPoint(double lat, double lng)
    {
        //SRID = 4326
        return new Point(lng, lat) {SRID = 4326};
    }
}