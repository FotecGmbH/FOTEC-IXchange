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
using System.Threading.Tasks;
using BDA.Common.Exchange.Enum;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Dc.Core;
using Database.Tables;
using IXchangeDatabase;
using IXchangeDatabase.Converter;
using SendGrid.Helpers.Errors.Model;
using ConverterDbCompany = Database.Converter.ConverterDbCompany;
using EnumReloadDcList = Exchange.Enum.EnumReloadDcList;


namespace IXchange.Service.AppConnectivity.DataConnector;

/// <summary>
///     <para>Datenaustausch für DcExCompanies</para>
/// Klasse DcExCompanies. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
/// </summary>
public partial class ServerRemoteCalls
{
    #region Interface Implementations

    /// <summary>
    ///     Device fordert Listen Daten für DcCompanies
    /// </summary>
    /// <param name="deviceId">Id des Gerätes</param>
    /// <param name="userId">Id des Benutzers oder -1 wenn nicht angemeldet</param>
    /// <param name="startIndex">Lesen ab Index (-1 für Start)</param>
    /// <param name="elementsToRead">Anzahl der Elemente welche maximal gelesen werden sollen (-1 für alle verfügbaren Daten)</param>
    /// <param name="secondId">
    ///     Optionale 2te Id um schnellen Wechsel zwischen Listen zu ermöglichen bzw. dynamische Listen. Zb.
    ///     für Chats
    /// </param>
    /// <param name="filter">Optionaler Filter für die Daten</param>
    /// <returns>Daten oder eine Exception auslösen</returns>
    public async Task<List<DcServerListItem<ExCompany>>> GetDcExCompanies(long deviceId, long userId, long startIndex, long elementsToRead, long secondId, string filter)
    {
        var returnList = new List<DcServerListItem<ExCompany>>();

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var db = new Db();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task


        //var tt = db.TblCompanies.Include(c => c.TblPermissions);


        var companies = db.GetTableCompanyForUser(userId);

        foreach (var tableCompany in companies)
        {
            returnList.Add(new DcServerListItem<ExCompany>
                           {
                               Index = tableCompany.Id,
                               SortIndex = tableCompany.Id,
                               // ReSharper disable once RedundantCast
                               Data = (ExCompany) ConverterDbCompany.ToExCompany(tableCompany)
                           });
        }

        return returnList;
    }

    /// <summary>
    ///     Device will Listen Daten für DcCompanies sichern
    /// </summary>
    /// <param name="deviceId">Id des Gerätes</param>
    /// <param name="userId">Id des Benutzers oder -1 wenn nicht angemeldet</param>
    /// <param name="data">Eingetliche Daten</param>
    /// <param name="secondId">
    ///     Optionale 2te Id um schnellen Wechsel zwischen Listen zu ermöglichen bzw. dynamische Listen. Zb.
    ///     für Chats
    /// </param>
    /// <returns>Ergebnis (bzw. Infos zum Fehler)</returns>
    public async Task<DcListStoreResult> StoreDcExCompanies(long deviceId, long userId, List<DcStoreListItem<ExCompany>> data, long secondId)
    {
        if (data == null!)
        {
            throw new ArgumentNullException(nameof(data));
        }

        var r = new DcListStoreResult
                {
                    SecondId = secondId,
                    StoreResult = new(),
                    ElementsStored = new()
                };

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var db = new Db();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        TableCompany p = null!;
        TablePermission pp = null!;
#pragma warning disable CS0219
        var anyDelete = false;
#pragma warning restore CS0219

        var tmp = new DcListStoreResultIndexAndData();


        foreach (var d in data)
        {
            switch (d.State)
            {
                case EnumDcListElementState.New:
                    p = new TableCompany();
                    d.Data.ToTableCompany(p);
                    r.ElementsStored++;
                    pp = new TablePermission();
                    break;
                case EnumDcListElementState.Modified:
                    throw new MethodNotAllowedException();
                case EnumDcListElementState.Deleted:
                    throw new MethodNotAllowedException();
                case EnumDcListElementState.None:
                    continue;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (d.State == EnumDcListElementState.New)
            {
                db.TblCompanies.Add(p);
            }

            await db.SaveChangesAsync().ConfigureAwait(true);

            if (d.State == EnumDcListElementState.New)
            {
                tmp.NewIndex = p.Id;
                tmp.NewSortIndex = p.Id;
                r.NewIndex.Add(tmp);

                pp.TblCompanyId = p.Id;
                pp.TblUserId = userId;
                pp.UserRight = EnumUserRight.ReadWrite;
                pp.UserRole = EnumUserRole.User;

                db.TblPermissions.Add(pp);
                await db.SaveChangesAsync().ConfigureAwait(true);

                await SendReloadList(EnumReloadDcList.ExCompanyUsers).ConfigureAwait(false);
            }
        }

        return r;
    }

    /// <summary>
    /// Daten Synchronisieren für DcExCompanies
    /// </summary>
    /// <param name="deviceId">Gerät</param>
    /// <param name="userId">User Id oder -1 wenn nicht angemeldet</param>
    /// <param name="current">Aktuelle Datensätze am Gerät</param>
    /// <param name="props">Zusätzliche Optionen</param>
    /// <returns>Neuer, aktualisierte und gelöschte Datensätze</returns>
    public Task<DcListSyncResultData<ExCompany>> SyncDcExCompanies(long deviceId, long userId, DcListSyncData current, DcListSyncProperties props)
    {
        throw new NotImplementedException();
    }

    #endregion
}