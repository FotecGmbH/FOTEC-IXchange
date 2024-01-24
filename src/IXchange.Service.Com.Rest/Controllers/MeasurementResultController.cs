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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BDA.Common.Exchange.Enum;
using BDA.Common.Exchange.Model;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Log.Producer;
using IXchange.Service.Com.Base;
using IXchange.Service.Com.Base.Helpers;
using IXchange.Service.Com.Rest.Enums;
using IXchange.Service.Com.Rest.Helpers;
using IXchangeDatabase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebExchange.Interfaces;


namespace IXchange.Service.Com.Rest.Controllers;

/// <summary>
/// <para>Abfragen für Messwerte</para>
/// Klasse MeasurementResultController.cs (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
/// </summary>
public class MeasurementResultController : Controller
{
    /// <summary>
    /// Datenbank Context
    /// </summary>
    private readonly Db _db;

    private readonly ITriggerAgent _triggerAgent;


    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="db">Datenbank</param>
    /// <param name="triggerAgent"></param>
    public MeasurementResultController(Db db, ITriggerAgent triggerAgent)
    {
        _db = db;
        _triggerAgent = triggerAgent ?? throw new ArgumentNullException(nameof(triggerAgent));
    }

    /// <summary>
    /// Details zu einem Messergebnis
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>Messergebnis</returns>
    [HttpGet("/api/measurementresult/{id}")]
    [IXChangeAuthorize]
    public virtual async Task<IActionResult> MeasurementResultGet(long id)
    {
        if (id <= 0)
        {
            return Ok(false);
        }

        if (!await UserAccessControl.HasMeasurmentResultPermission((ExUser) HttpContext.Items["User"]!, _db, id).ConfigureAwait(true))
        {
            return UserAccessControl.Unauthorized();
        }

        var mr = await _db.TblMeasurementResults.AsNoTracking().Select(aa => new ExRestMeasurementResult
                                                                             {
                                                                                 Location = new ExPosition
                                                                                            {
                                                                                                Altitude = aa.Location.Altitude,
                                                                                                Latitude = aa.Location.Latitude,
                                                                                                Longitude = aa.Location.Longitude,
                                                                                                TimeStamp = aa.Location.TimeStamp,
                                                                                                Presision = aa.Location.Precision,
                                                                                                Source = aa.Location.Source
                                                                                            },
                                                                                 AdditionalProperties = aa.AdditionalProperties,
                                                                                 Value = CommonMethodsHelper.GetValueOfMeasurementResult(aa),
                                                                                 ValueType = aa.ValueType,
                                                                                 TimeStamp = aa.TimeStamp,
                                                                                 Id = aa.Id
                                                                             }).FirstOrDefaultAsync(a => a.Id == id).ConfigureAwait(true);

        return Ok(mr);
    }

    /// <summary>
    /// Abfrage für Messungen
    /// </summary>
    /// <param name="id">ID der Messdefinition</param>
    /// <param name="take">Anzahl der Daten die abgefragt werden</param>
    /// <param name="skip">Anzahl der Daten die übersprungen werden</param>
    /// <param name="valueType">Datentyp</param>
    /// <param name="filterAdditionalProperty">Filter für zusätzliche Eigenschaften (contains)</param>
    /// <returns>Liste an Messergebnissen</returns>
    [HttpGet("/api/measurementresult/query/{id}/{take}/{skip}")]
    [EnableQuery]
    [IXChangeAuthorize]
    public virtual async Task<IActionResult> MeasurementResultQuery(long id = -1, int take = 20, int skip = 0, [FromQuery] EnumQueryValueTypes valueType = EnumQueryValueTypes.All, [FromQuery] string filterAdditionalProperty = "" /*, [FromQuery] string orderby = "timestamp desc"*/)
    {
        if (take > 5000)
        {
            return BadRequest("Take darf nicht > 5000 sein");
        }

        var user = (ExUser) HttpContext.Items["User"]!;

        if (id == -1 && !user.IsAdmin)
        {
            if (!await UserAccessControl.HasMeasurmentResultPermission((ExUser) HttpContext.Items["User"]!, _db, id).ConfigureAwait(true))
            {
                return UserAccessControl.Unauthorized();
            }
        }

        var result = new ExRestMeasurementQueryResult();

        //QueryAble -> Hat den Vorteile mehrere Filter zu setzen, vermeidet verschachtelungen

        var queryAble = _db.TblMeasurementResults.AsQueryable().AsNoTracking();

        queryAble = queryAble.Where(a => a.TblMeasurementDefinitionId == id || id == -1);

        //queryAble = queryAble.ApplyOrderbyOnMeasurementResults(orderby);

        if (valueType != EnumQueryValueTypes.All)
        {
            queryAble = queryAble.Where(a => a.ValueType == (EnumValueTypes) valueType);
        }

        if (!String.IsNullOrEmpty(filterAdditionalProperty))
        {
            queryAble = queryAble.Where(a => a.AdditionalProperties.Contains(filterAdditionalProperty));
        }

        var xx = queryAble
            .Select(aa => new ExRestMeasurementResult
                          {
                              Location = new ExPosition
                                         {
                                             Altitude = aa.Location.Altitude,
                                             Latitude = aa.Location.Latitude,
                                             Longitude = aa.Location.Longitude,
                                             TimeStamp = aa.Location.TimeStamp,
                                             Presision = aa.Location.Precision,
                                             Source = aa.Location.Source
                                         },
                              AdditionalProperties = aa.AdditionalProperties,
                              Value = CommonMethodsHelper.GetValueOfMeasurementResult(aa),
                              ValueType = aa.ValueType,
                              TimeStamp = aa.TimeStamp,
                              Id = aa.Id,
                          })
            .Skip(skip)
            .Take(take);
        result.MeasurementResults = xx;

        result.Count = _db.TblMeasurementResults.Count(a => a.TblMeasurementDefinitionId == id || id == -1);
        return Ok(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">ID der Messdefinition</param>
    /// <param name="fromDate">Format yyyyMMdd</param>
    /// <param name="toDate">Format yyyyMMdd</param>
    /// <returns></returns>
    [HttpGet("/api/measurementresult/timeperiod/{id}/{fromDate}/{toDate}")]
    [EnableQuery]
    //[IXChangeAuthorize] //TODO MMa wieder reinkommentieren, => SaProjectBase Wap.Get hat keinen Bearer Token derzeit
    public virtual IActionResult MeasurementResultTimePeriod(long id, string fromDate, string toDate)
    {
        var user = (ExUser) HttpContext.Items["User"]!;

        if (id == -1 && !user.IsAdmin)
        {
            return UserAccessControl.Unauthorized();
        }

        var datesucess1 = DateTime.TryParseExact(fromDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateFrom);
        var datesucess2 = DateTime.TryParseExact(toDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTo);
        if (!datesucess1 || !datesucess2)
        {
            return BadRequest("Falsches Datumsformat");
        }

        //QueryAble -> Hat den Vorteile mehrere Filter zu setzen, vermeidet verschachtelungen

        var queryAble = _db.TblMeasurementResults.AsQueryable().AsNoTracking();

        queryAble = queryAble
            .Where(a => (id == -1 || a.TblMeasurementDefinitionId == id) && a.TimeStamp >= dateFrom && a.TimeStamp <= dateTo);

        var result = queryAble
            .Select(aa => new ExMeasurement
                          {
                              Location = new ExPosition
                                         {
                                             Altitude = aa.Location.Altitude,
                                             Latitude = aa.Location.Latitude,
                                             Longitude = aa.Location.Longitude,
                                             TimeStamp = aa.Location.TimeStamp,
                                             Presision = aa.Location.Precision,
                                             Source = aa.Location.Source
                                         },
                              Value = CommonMethodsHelper.GetValueOfMeasurementResult(aa),
                              TimeStamp = aa.TimeStamp,
                              Id = aa.Id,
                          });

        //test-values Todo: löschen
        //var list = result.ToList();
        //if (!list.Any())
        //{
        //    var numberOfElements = 20;
        //    for (var i = 1; i <= numberOfElements; i++)
        //    {
        //        list.Add(new ExMeasurement()
        //                           {
        //                               Id = i,
        //                               Value = Random.Shared.Next(20).ToString(),
        //                               ValueCounter = numberOfElements,
        //                               TimeStamp = DateTime.UtcNow.AddDays(numberOfElements * -1 + i),
        //                           });
        //    }

        //    return Ok(list.OrderByDescending(x => x.TimeStamp));
        //}

        return Ok(result.OrderBy(x => x.TimeStamp));
    }

    /// <summary>
    ///     Messergebnis hinzufügen
    /// </summary>
    /// <param name="definitionId">ID Messdefinition</param>
    /// <param name="measurementResult">Messresultat</param>
    /// <returns></returns>
    [HttpPost("/api/measurementresult/create/{definitionId}")]
    public virtual async Task<IActionResult> MeasurementResultCreate(long definitionId, ExRestMeasurementResult measurementResult)
    {
        if (definitionId <= 0)
        {
            return BadRequest();
        }

        if (!await UserAccessControl.HasMeasurmentDefinitionPermission((ExUser) HttpContext.Items["User"]!, _db, definitionId, true).ConfigureAwait(true))
        {
            return UserAccessControl.Unauthorized();
        }

        var md = await _db.TblMeasurementDefinitions.Include(def => def.TblIoTDevice).FirstOrDefaultAsync(a => a.Id == definitionId).ConfigureAwait(true);

        if (md == null)
        {
            return BadRequest();
        }

        try
        {
            var model = _db.TblMeasurementResults.Add(measurementResult.GetTableModel(md.Id));


            await _db.SaveChangesAsync().ConfigureAwait(true);

            if (md.TblIoTDevice.TblGatewayId != null)
            {
                _triggerAgent.NewMeasurementsFromGateway(md.TblIoTDevice.TblGatewayId.Value, new List<long> {md.Id});
            }


            return Ok(model.Entity.Id);
        }
        catch (Exception e)
        {
            Logging.Log.LogError($"{e}");
            return BadRequest();
        }
    }

    /// <summary>
    ///     Messergebnisse löschen bis bestimmten Datum
    /// </summary>
    /// <param name="definitionId">ID Measurement Definition</param>
    /// <param name="dateTime">Zeitpunkt</param>
    /// <returns></returns>
    [HttpDelete("/api/measurementresult/delete/{definitionId}")]
    [IXChangeAuthorize]
    public virtual async Task<IActionResult> MeasurementResultDelete(long definitionId, string dateTime)
    {
        // ReSharper disable once RedundantAssignment
        var date = DateTime.MinValue;
        var cultureInfo = new CultureInfo("de-DE");
        date = DateTime.Parse(dateTime, cultureInfo);
        if (definitionId <= 0)
        {
            return BadRequest();
        }

        if (!await UserAccessControl.HasMeasurmentResultPermission((ExUser) HttpContext.Items["User"]!, _db, definitionId, true).ConfigureAwait(true))
        {
            return UserAccessControl.Unauthorized();
        }

        var md = await _db.TblMeasurementDefinitions.Include(def => def.TblIoTDevice).FirstOrDefaultAsync(a => a.Id == definitionId).ConfigureAwait(true);
        if (md == null)
        {
            return BadRequest();
        }

        foreach (var result in _db.TblMeasurementResults.Where(a => a.TblMeasurementDefinitionId == definitionId))
        {
            if (result.TimeStamp < date)

            {
                _db.TblMeasurementResults.Remove(result);
            }
        }

        await _db.SaveChangesAsync();

        return Ok();
    }
}