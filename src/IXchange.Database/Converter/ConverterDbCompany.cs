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
using BDA.Common.Exchange.Model.ConfigApp;
using Database.Converter;
using Database.Tables;

// ReSharper disable once CheckNamespace
namespace IXchangeDatabase.Converter;

/// <summary>
///     <para>DB Company Hilfsmethoden</para>
/// Klasse ConverterDbCompany. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
/// </summary>
public static class ConverterDbCompany
{
    /// <summary>
    ///     Companies, Ex zu tbl
    /// </summary>
    /// <param name="co"></param>
    /// <param name="t"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void ToTableCompany(this ExCompany co, TableCompany t)
    {
        if (co == null!)
        {
            throw new ArgumentNullException(nameof(co));
        }

        if (t == null!)
        {
            throw new ArgumentNullException(nameof(t));
        }

        t.CompanyType = co.CompanyType;
        co.Information.ToDbInformation(t.Information);
        t.TblGateways = new List<TableGateway>();
        t.TblCompanyGlobalConfigurations = new List<TableCompanyGlobalConfig>();
        t.TblPermissions = new List<TablePermission>();
        t.TblProjects = new List<TableProject>();
    }
}