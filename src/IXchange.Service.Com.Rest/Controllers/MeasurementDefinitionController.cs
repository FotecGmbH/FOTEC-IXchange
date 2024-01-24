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
using BDA.Common.Exchange.Model;
using BDA.Common.Exchange.Model.ConfigApp;
using Database.Converter;
using IXchange.Service.Com.Base.Extensions;
using IXchange.Service.Com.Base.Helpers;
using IXchange.Service.Com.Rest.Mapper;
using IXchangeDatabase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;


namespace IXchange.Service.Com.Rest.Controllers;

/// <summary>
/// <para>Abfragen für Messdefinitionen</para>
/// Klasse MeasurementDefinitionController.cs (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
/// </summary>
public class MeasurementDefinitionController : Controller
{
    /// <summary>
    /// Datenbank Context
    /// </summary>
    private readonly Db _db;

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="db">DB</param>
    public MeasurementDefinitionController(Db db)
    {
        _db = db;
    }

    /// <summary>
    /// Informationen aller Messdefinitionen abfragen, die der Benutzer besitzt oder aboniert hat
    /// </summary>
    /// <returns>Informationen der Messdefinition</returns>
    [HttpGet("/api/measurementdefinition/list")]
    [EnableQuery]
    [IXChangeAuthorize]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public virtual async Task<IActionResult> MeasurementDefinitionsHttpGet()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        var isValidToken = HttpContext.TryGetExUserFromHttpContext(out var user);

        if (!isValidToken)
        {
            return UserAccessControl.Unauthorized();
        }

        IEnumerable<ExMeasurementDefinition> own = new List<ExMeasurementDefinition>();
        IEnumerable<ExMeasurementDefinition> abos = new List<ExMeasurementDefinition>();

        if (user == null)
        {
            return BadRequest("Couldn't find User");
        }

        try
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            own = _db.TblPermissions
                .Include(p => p.TblCompany)
                .ThenInclude(c => c.TblGateways)
                .ThenInclude(g => g.TblIotDevices)
                .ThenInclude(d => d.TblMeasurementDefinitions)
                .AsNoTracking()
                .FirstOrDefault(p => p.TblUserId == user.Id)
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                .TblCompany
                .TblGateways.FirstOrDefault()
                .TblIotDevices.SelectMany(dev => dev.TblMeasurementDefinitions.Select(m => m.ToExMeasurementDefinition( /*false*/)));
        }
        catch (Exception )
        {
            // ignored
        }

        try
        {
            abos = _db.TblAbos
                .Include(a => a.TblMeasurementDefinitionAssignment)
                .ThenInclude(md => md.TblMeasurementDefinition)
                .ThenInclude(md => md.TblIoTDevice)
                .ThenInclude(device => device.TblGateway)
                .Include(a => a.TblMeasurementDefinitionAssignment)
                .ThenInclude(md => md.TblMeasurementDefinition)
                .ThenInclude(md => md.Information)
                .Where(a => a.TblUserId == user.Id)
                .Select(a => a.TblMeasurementDefinitionAssignment.TblMeasurementDefinition.ToExMeasurementDefinition( /*false*/));
        }
        catch (Exception)
        {
            // ignored
        }

        var result = own.Concat(abos).DistinctBy(r => r.Id).AsQueryable();

        return Ok(result);
    }

    /// <summary>
    /// Informationen über die Messdefinition abfragen
    /// </summary>
    /// <param name="id">ID der Messdefinition</param>
    /// <returns>Informationen der Messdefinition</returns>
    [HttpGet("/api/measurementdefinition/{id}")]
    //[EnableQuery]
    [IXChangeAuthorize]
    public virtual async Task<IActionResult> MeasurementDefinitionHttpGet(long id)
    {
        if (id <= 0)
        {
            return Ok(false);
        }

        if (!await UserAccessControl.HasMeasurmentDefinitionPermission((ExUser) HttpContext.Items["User"]!, _db, id).ConfigureAwait(true))
        {
            return UserAccessControl.Unauthorized();
        }

        var tmp = _db.TblMeasurementDefinitions.FirstOrDefault(a => a.Id == id);
        if (tmp == null)
        {
            return Ok();
        }

        return Ok(tmp.ToExRestMeasurmentDefinition());
        //var md = await _db.TblMeasurementDefinitions.AsNoTracking().Select(a => a.ToExRestMeasurmentDefinition()).FirstOrDefaultAsync(a => a.Id == id).ConfigureAwait(true);


        //return Ok(md);
    }
}