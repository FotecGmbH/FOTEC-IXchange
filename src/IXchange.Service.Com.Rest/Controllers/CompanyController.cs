// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using IXchangeDatabase;
using Microsoft.AspNetCore.Mvc;

namespace IXchange.Service.Com.Rest.Controllers;

/// <summary>
///     <para>Firmen</para>
/// Klasse CompanyController.cs (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
/// </summary>
public class CompanyController : Controller
{
    /// <summary>
    ///     Datenbank Context
    /// </summary>
    // ReSharper disable once NotAccessedField.Local
    private readonly Db _db;

    /// <summary>
    ///     ctor
    /// </summary>
    /// <param name="db">Database</param>
    public CompanyController(Db db)
    {
        _db = db;
    }
}