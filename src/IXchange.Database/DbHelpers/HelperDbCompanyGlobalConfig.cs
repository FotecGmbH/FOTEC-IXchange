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
using BDA.Common.Exchange.Enum;
using Database.Tables;

// ReSharper disable once CheckNamespace
namespace IXchangeDatabase;

/// <summary>
///     <para>Hilfklassen für CompanyGlobalConfig</para>
/// Klasse HelperDbCompanyGlobalConfig. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
/// </summary>
public partial class Db
{
    /// <summary>
    ///     Alle globalen Configs für einen bestimmten Benutzer
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public IQueryable<TableCompanyGlobalConfig> GetTableCompanyGlobalConfigForUser(long userId)
    {
        var r = TblCompanyGlobalConfigs.Where(c =>
            c.TblCompany.CompanyType == EnumCompanyTypes.PublicCompany ||
            (c.TblCompany.TblPermissions.Any(a => a.TblUserId == userId) ||
             TblUsers.Any(a => a.Id == userId && a.IsAdmin)
            ));

        return r;
    }
}