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
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace IXchangeDatabase;

/// <summary>
///     <para>DESCRIPTION</para>
/// Klasse HelperDbCompany. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
/// </summary>
public partial class Db
{
    /// <summary>
    ///     Ist ein bestimmter user Super-Admin
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public bool IsUserAdmin(long userId)
    {
        if (userId <= 0)
        {
            return false;
        }

        return TblUsers.Where(f => f.Id == userId).Select(s => s.IsAdmin).FirstOrDefault();
    }

    /// <summary>
    ///     Einen bestimmten Benutzer mit dessen Abhängigkeiten laden
    /// </summary>
    /// <param name="userId">Id des Users</param>
    /// <returns></returns>
    public TableUser? GetUserWithdependences(long userId)
    {
        return TblUsers.Where(i => i.Id == userId).Include(x => x.TblUserImage).Include(y => y.TblDevices).Include(i => i.TblPermissions).Include(i => i.TblAccessToken).FirstOrDefault();
    }
}