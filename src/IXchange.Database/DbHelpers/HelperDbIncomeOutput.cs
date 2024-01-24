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
using IXchangeDatabase.Tables;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace IXchangeDatabase;

/// <summary>
///     <para>DESCRIPTION</para>
/// Klasse HelperDbIncomeOutput. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
/// </summary>
public partial class Db
{
    /// <summary>
    /// AddIncomeOutput
    /// </summary>
    /// <param name="ixiesValue">ixiesValue</param>
    /// <param name="description">description</param>
    /// <param name="tblUserId">tblUserId</param>
    /// <param name="dateTime">dateTime</param>
    /// <param name="saveChanges">saveChanges</param>
    public async Task AddIncomeOutput(int ixiesValue, string description, long tblUserId, DateTime? dateTime = null, bool saveChanges = true)
    {
        var time = dateTime ?? DateTime.UtcNow;

        var allItems = await TblIncomeOutputs
            .Where(x => x.TblUserId == tblUserId)
            .ToListAsync()
            .ConfigureAwait(false);

        var previousItems = allItems.Where(x => x.TimeStamp <= time).OrderByDescending(x => x.TimeStamp);
        // ReSharper disable once UnusedVariable
        var laterItems = allItems.Where(x => x.TimeStamp > time);

        var currentTotalIxies = previousItems.FirstOrDefault()?.CurrentTotalIxies ?? 0;

        var newItem = new TableIncomeOutput
                      {
                          Ixies = ixiesValue,
                          //Description = description,
                          TblUserId = tblUserId,
                          TimeStamp = time,
                          CurrentTotalIxies = currentTotalIxies + ixiesValue
                      };

        await TblIncomeOutputs.AddAsync(newItem).ConfigureAwait(false);

        if (saveChanges)
        {
            await SaveChangesAsync().ConfigureAwait(false);
        }
    }
}