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
using IXchangeDatabase.Tables;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace IXchangeDatabase;

/// <summary>
///     <para>Helper für <see cref="TblMeasurementDefinitions" /> </para>
/// Klasse HelperDbMeasurementDefinition. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
/// </summary>
public partial class Db
{
    /// <summary>
    ///     Alle Messwert-Definitionen
    /// </summary>
    /// <returns></returns>
    public IQueryable<TableMeasurementDefinitionAssignment> GetMeasurementDefinitionAssignments()
    {
        var r = TblMeasurementDefinitionAssignments
            .AsNoTracking()
            //.Include(i => i.TblMeasurementDefinition)
            //.ThenInclude(md=>md.TblMeasurements)
            .Include(i => i.TblMeasurementDefinition)
            .ThenInclude(md => md.Information)
            //.Include(i => i.TblMeasurementDefinition)
            //.ThenInclude(md=>md.TblIoTDevice)
            //.ThenInclude(md=>md.TblGateway)
            //.ThenInclude(md=>md.TblCompany)
            .Include(t => t.TblRatings)
            .ThenInclude(r => r.TblUser)
            .Include(t => t.TblNotifications)
            .Include(x => x.TblAbos);
        //.Select(s => new TableMeasurementDefinitionAssignment()
        //{
        //    Id = s.Id,
        //    Type = s.Type,
        //    TblMeasurementDefinition = s.TblMeasurementDefinition,
        //    TblMeasurementDefinitionId = s.TblMeasurementDefinitionId,
        //    NotificationOnNewRating = s.NotificationOnNewRating,
        //    NotificationOnSubscription = s.NotificationOnSubscription,
        //    NotificationOnUnsubscription = s.NotificationOnUnsubscription,
        //    AccessForResearchInstitutesGranted = s.AccessForResearchInstitutesGranted
        //});

        return r;
    }
}