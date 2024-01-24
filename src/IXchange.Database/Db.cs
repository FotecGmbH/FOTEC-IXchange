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
using BDA.Common.Exchange.Configs.Enums;
using BDA.Common.Exchange.Enum;
using Biss.Apps.Base;
using Biss.Common;
using Biss.Log.Producer;
using Database.Common;
using Database.Tables;
using Exchange.Enum;
using IXchangeDatabase.Tables;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using WebExchange;

// ReSharper disable once CheckNamespace
namespace IXchangeDatabase
{
    /// <summary>
    ///     <para>Projektweite Datenbank - Entity Framework Core</para>
    /// Klasse Db. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public partial class Db : DbContext
    {
        /// <summary>
        /// Ob Postgres-DB verwendet wird (sonst SQL-Server-DB)
        /// </summary>
        public static bool UsePostgres = false;

        /// <summary>
        ///     Db Context initialisieren - für SQL Server
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder != null!)
            {
                if (UsePostgres)
                {
                    optionsBuilder.UseNpgsql(WebConstants.ConnectionStringPostGres, x => x.UseNetTopologySuite().MigrationsAssembly("Database.Postgres.Migrations") /*.SetPostgresVersion(15, 3)*/);
                }
                else
                {
                    optionsBuilder.UseSqlServer(WebConstants.ConnectionString, x => x.UseNetTopologySuite());
                }
            }
        }

        #region Erzeugen, Neu Erzeugen, Löschen

        /// <summary>
        /// Datenbank löschen
        /// </summary>
        /// <returns>Erfolg</returns>
        public static bool DeleteDatabase()
        {
            Logging.Log.LogTrace($"[{nameof(Db)}]({nameof(DeleteDatabase)}): Delete Database");
            using var db = new Db();
            try
            {
                db.Database.EnsureDeleted();
            }
            catch (Exception e)
            {
                Logging.Log.LogWarning($"[{nameof(Db)}]({nameof(DeleteDatabase)}): Error deleting database: {e}");
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Datenbank wird bei Aufruf erzugt bzw. gelöscht und neu erzeugt
        /// </summary>
        /// <returns>Erfolg</returns>
        public static bool CreateAndFillUp()
        {
            using var db = new Db();

            var createDb = true;

            if (createDb)
            {
                //DB anlegen

                var connstrBldr = new SqlConnectionStringBuilder(db.Database.GetConnectionString())
                                  {
                                      InitialCatalog = "master"
                                  };

                Logging.Log.LogTrace($"[{nameof(Db)}]({nameof(CreateAndFillUp)}): Create Database");

                using (var conn = new SqlConnection(connstrBldr.ConnectionString))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandTimeout = 360;
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                    cmd.CommandText = $"CREATE DATABASE [{db.Database.GetDbConnection().Database}] (EDITION = 'basic')";
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
                    cmd.ExecuteNonQuery();
                }

                Logging.Log.LogTrace($"[{nameof(Db)}]({nameof(CreateAndFillUp)}): Create tables");

                db.Database.EnsureCreated();

                Logging.Log.LogTrace($"[{nameof(Db)}]({nameof(CreateAndFillUp)}): Initially fill up the database");

                #region TableUser

                var admin = new TableUser
                            {
                                LoginName = "biss@fotec.at",
                                FirstName = "Biss",
                                LastName = "Admin",
                                Locked = false,
                                PasswordHash = AppCrypt.CumputeHash("1234"),
                                DefaultLanguage = "de",
                                LoginConfirmed = true,
                                IsAdmin = true,
                                AgbVersion = "1.0.0",
                                CreatedAtUtc = DateTime.UtcNow,
                                RefreshToken = AppCrypt.GeneratePassword(),
                                JwtToken = AppCrypt.GeneratePassword()
                            };
                db.TblUsers.Add(admin);

                #endregion

                #region TableSetting

                var settings = EnumUtil.GetValues<EnumDbSettings>();
                foreach (var setting in settings)
                {
                    var v = new Version(1, 0);
                    switch (setting)
                    {
                        case EnumDbSettings.Agb:
                            db.TblSettings.Add(new TableSetting {Key = setting, Value = v.ToString()});
                            break;
                        case EnumDbSettings.CurrentAppVersion:
                            db.TblSettings.Add(new TableSetting {Key = setting, Value = v.ToString()});
                            break;
                        case EnumDbSettings.MinAppVersion:
                            db.TblSettings.Add(new TableSetting {Key = setting, Value = v.ToString()});
                            break;
                        case EnumDbSettings.CommonMessage:
                            db.TblSettings.Add(new TableSetting {Key = setting, Value = ""});
                            break;
                        case EnumDbSettings.ConfigAppWindows:
                            db.TblSettings.Add(new TableSetting {Key = setting, Value = ""});
                            break;
                        case EnumDbSettings.GatewayAppWindows:
                            db.TblSettings.Add(new TableSetting {Key = setting, Value = ""});
                            break;
                        case EnumDbSettings.GatewayAppLinux:
                            db.TblSettings.Add(new TableSetting {Key = setting, Value = ""});
                            break;
                        case EnumDbSettings.SensorTemplateFipiTtn:
                            db.TblSettings.Add(new TableSetting {Key = setting, Value = ""});
                            break;
                        case EnumDbSettings.InterfaceGrpc:
                            db.TblSettings.Add(new TableSetting {Key = setting, Value = ""});
                            break;
                        case EnumDbSettings.ConfigAppWeb:
                            db.TblSettings.Add(new TableSetting {Key = setting, Value = ""});
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                #endregion

                #region TableCompany

                var noCompany = new TableCompany
                                {
                                    Information = new()
                                                  {
                                                      Name = "NoCompany",
                                                      Description = "Neue Gateways tauchen hier auf und können dann einer Firma zugewiesen werden",
                                                      CreatedDate = DateTime.UtcNow
                                                  },
                                    CompanyType = EnumCompanyTypes.NoCompany
                                };
                var publicCompany = new TableCompany
                                    {
                                        Information = new()
                                                      {
                                                          Name = "Öffentliche Firma",
                                                          Description = "Alle Daten dieser Firma sind öffentlich",
                                                          CreatedDate = DateTime.UtcNow
                                                      },
                                        CompanyType = EnumCompanyTypes.PublicCompany
                                    };

                var fotecCompany = new TableCompany
                                   {
                                       Information = new()
                                                     {
                                                         Name = "FOTEC",
                                                         Description = "FOTEC Forschungs- und Technologietransfer GmbH",
                                                         CreatedDate = DateTime.UtcNow
                                                     },
                                       CompanyType = EnumCompanyTypes.Company
                                   };

                var bissCompany = new TableCompany
                                  {
                                      Information = new()
                                                    {
                                                        Name = "biss@fotec.at",
                                                        Description = "Company von biss@fotec.at",
                                                        CreatedDate = DateTime.UtcNow
                                                    },
                                      CompanyType = EnumCompanyTypes.Company
                                  };

                db.TblCompanies.Add(noCompany);
                db.TblCompanies.Add(publicCompany);
                db.TblCompanies.Add(fotecCompany);
                db.TblCompanies.Add(bissCompany);

                #endregion


                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Logging.Log.LogWarning($"[{nameof(Db)}]({nameof(CreateAndFillUp)}): Error creating database: {e}");
                    return false;
                }

                var bissC = db.TblCompanies.Include(c => c.Information).FirstOrDefault(c => c.Information.Name == "biss@fotec.at");

                if (bissC != null)
                {
                    var bissGateway = new TableGateway
                                      {
                                          Information = new DbInformation
                                                        {
                                                            Name = "biss@fotec.at",
                                                            Description = "Gateway des Users mit der Email Addresse biss@fotec.at",
                                                            CreatedDate = DateTime.UtcNow
                                                        },
                                          TblCompanyId = bissC.Id
                                      };
                    db.TblGateways.Add(bissGateway);
                    db.SaveChanges();
                }


                AddGatewayIotDeviceMeasurementDefAboTest(db);
            }
            else
            {
                return false;
            }

            return true;
        }


        private static void AddGatewayIotDeviceMeasurementDefAboTest(Db db)
        {
            #region TableIoTDevice_MeasurementDef_Abo

            var myGateway = new TableGateway {TblCompanyId = db.TblCompanies.FirstOrDefault()!.Id, Information = new DbInformation {Name = "TestGateway"}};
            try
            {
                db.TblGateways.Add(myGateway);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Logging.Log.LogWarning($"[{nameof(Db)}]({nameof(AddGatewayIotDeviceMeasurementDefAboTest)}): Error adding {e}");
                return;
            }

            var myIoTDevice = new TableIotDevice {MeasurementInterval = 100, Plattform = EnumIotDevicePlattforms.DotNet, Upstream = EnumIotDeviceUpstreamTypes.Tcp, TransmissionType = EnumTransmission.Instantly, TblGatewayId = db.TblGateways.FirstOrDefault()!.Id, Information = new DbInformation {Name = "TestIoTDevice"}};
            try
            {
                db.TblIotDevices.Add(myIoTDevice);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Logging.Log.LogWarning($"[{nameof(Db)}]({nameof(AddGatewayIotDeviceMeasurementDefAboTest)}): Error adding {e}");
                return;
            }

            var myMessuar = new TableMeasurementDefinition {DownstreamType = EnumIotDeviceDownstreamTypes.DotNet, MeasurementInterval = 100, ValueType = EnumValueTypes.Text, TblIotDeviceId = db.TblIotDevices.FirstOrDefault()!.Id, Information = new DbInformation {Name = "TestMeasurementDefinition"}};
            try
            {
                db.TblMeasurementDefinitions.Add(myMessuar);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Logging.Log.LogWarning($"[{nameof(Db)}]({nameof(AddGatewayIotDeviceMeasurementDefAboTest)}): Error adding {e}");
                return;
            }

            var myAbo = new TableAbo {TblUserId = db.TblUsers.FirstOrDefault()!.Id, TblMeasurementDefinitionAssignmentId = db.TblMeasurementDefinitions.FirstOrDefault()!.Id, ExceedNotify = true, ExceedNotifyValue = 10};
            try
            {
                db.TblAbos.Add(myAbo);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Logging.Log.LogWarning($"[{nameof(Db)}]({nameof(AddGatewayIotDeviceMeasurementDefAboTest)}): Error adding {e}");
                return;
            }

            var myRating = new TableRating {TblUserId = db.TblUsers.FirstOrDefault()!.Id, TblMeasurementDefinitionAssignmentId = db.TblMeasurementDefinitions.FirstOrDefault()!.Id, Rating = 2, TimeStamp = DateTime.Now, Description = "Test Rating"};
            try
            {
                db.TblRatings.Add(myRating);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Logging.Log.LogWarning($"[{nameof(Db)}]({nameof(AddGatewayIotDeviceMeasurementDefAboTest)}): Error adding {e}");
                return;
            }

            var myIncome = new TableIncomeOutput {TblUserId = db.TblUsers.FirstOrDefault()!.Id, TimeStamp = DateTime.UtcNow /*, Description = "Test Income"*/, TblMeasurementDefinitonId = null, Option = EnumIncomeOutputOption.Transfer, Ixies = 50};
            try
            {
                db.TblIncomeOutputs.Add(myIncome);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Logging.Log.LogWarning($"[{nameof(Db)}]({nameof(AddGatewayIotDeviceMeasurementDefAboTest)}): Error adding {e}");
                return;
            }

            var myNotification = new TableNotification {TblUserId = db.TblUsers.FirstOrDefault()!.Id, TblMeasurementDefinitionAssignmentId = db.TblMeasurementDefinitions.FirstOrDefault()!.Id, Description = "Test Notification"};
            try
            {
                db.TblNotifications.Add(myNotification);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Logging.Log.LogWarning($"[{nameof(Db)}]({nameof(AddGatewayIotDeviceMeasurementDefAboTest)}): Error adding {e}");
            }

            #endregion
        }

        // ReSharper disable once UnusedMember.Local
        /// <summary>
        /// AddIotDeviceMeasurementDefTest
        /// </summary>
        /// <param name="db">db</param>
        private static void AddIotDeviceMeasurementDefTest(Db db)
        {
            #region TableIoTDevice_MeasurementDef_Abo

            //47.838060, 16.252546
            var r = new Random();
            var lat = 47.5 + (r.NextDouble() / 2);
            var longi = 16.0 + (r.NextDouble() / 2);

            //db.TblIotDevices.FirstOrDefault().FallbackPosition = new DbPosition() {Altitude = 0, Latitude = 47.848985, Longitude = 16.241863, TimeStamp = DateTime.Now, Source = EnumPositionSource.Pc};

            var ioTDeviceRand = r.Next(2, 500);
            var myIoTDevice = new TableIotDevice {FallbackPosition = new DbPosition {Altitude = 0, Latitude = lat, Longitude = longi}, Information = new DbInformation {Description = "Description of TestIoTDevice:" + ioTDeviceRand, Name = "TestIoTDevice:" + ioTDeviceRand}, MeasurementInterval = 100, Plattform = EnumIotDevicePlattforms.DotNet, Upstream = EnumIotDeviceUpstreamTypes.Tcp, TransmissionType = EnumTransmission.Instantly, TblGatewayId = db.TblGateways.FirstOrDefault()!.Id};

            for (var i = 0; i < r.Next(2, 5); i++)
            {
                var measdefRand = r.Next(2, 500);
                var measDef = new TableMeasurementDefinition {Information = new DbInformation {Description = "Description of MeasurementDefinition:" + measdefRand, Name = "TestMeasurementDefinition:" + measdefRand}, DownstreamType = EnumIotDeviceDownstreamTypes.DotNet, MeasurementInterval = 100, ValueType = EnumValueTypes.Number};

                for (var j = 0; j < r.Next(2, 20); j++)
                {
                    measDef.TblMeasurements.Add(new TableMeasurementResult {TimeStamp = DateTime.Now - TimeSpan.FromDays(r.Next(1, 10)), ValueType = EnumValueTypes.Number, Value = new DbValue {Number = r.NextDouble() * 10}, Location = new DbPosition {Altitude = 0, Latitude = 0, Longitude = 0}, SpatialPoint = new Point(longi, lat) {SRID = 4326}});
                }

                myIoTDevice.TblMeasurementDefinitions.Add(measDef);
            }

            try
            {
                db.TblIotDevices.Add(myIoTDevice);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Logging.Log.LogWarning($"[{nameof(Db)}]({nameof(AddGatewayIotDeviceMeasurementDefAboTest)}): Error adding {e}");
            }

            #endregion
        }

        #endregion

        #region Tabellen

        /// <summary>
        ///     Tabelle AccessToken
        /// </summary>
        public virtual DbSet<TableAccessToken> TblAccessToken { get; set; } = null!;

        /// <summary>
        ///     Tabelle Firma
        /// </summary>
        public virtual DbSet<TableCompany> TblCompanies { get; set; } = null!;

        /// <summary>
        ///     Tabelle Firmenkonfiguration
        /// </summary>
        public virtual DbSet<TableCompanyGlobalConfig> TblCompanyGlobalConfigs { get; set; } = null!;

        /// <summary>
        ///     Tabelle Gateway
        /// </summary>
        public virtual DbSet<TableGateway> TblGateways { get; set; } = null!;

        /// <summary>
        ///     Tabelle IoT Geräte
        /// </summary>
        public virtual DbSet<TableIotDevice> TblIotDevices { get; set; } = null!;

        /// <summary>
        /// Tabelle mit allen Parsern
        /// </summary>
        public virtual DbSet<TableDataconverter> TblDataconverters { get; set; } = null!;

        /// <summary>
        /// Tabelle mit Measurementdefinition Templates
        /// </summary>
        public virtual DbSet<TableMeasurementDefinitionTemplate> TblMeasurementDefinitionTemplates { get; set; } = null!;

        /// <summary>
        ///     Tabelle Messungen
        /// </summary>
        public virtual DbSet<TableMeasurementResult> TblMeasurementResults { get; set; } = null!;

        /// <summary>
        ///     Tabelle Berechtigung
        /// </summary>
        public virtual DbSet<TablePermission> TblPermissions { get; set; } = null!;

        /// <summary>
        ///     Tabelle Projekte
        /// </summary>
        public virtual DbSet<TableProject> TblProjects { get; set; } = null!;

        /// <summary>
        ///     Tabelle Sensoren
        /// </summary>
        public virtual DbSet<TableMeasurementDefinition> TblMeasurementDefinitions { get; set; } = null!;

        /// <summary>
        ///     Tabelle Sensoren
        /// </summary>
        public virtual DbSet<TableMeasurementDefinitionAssignment> TblMeasurementDefinitionAssignments { get; set; } = null!;

        /// <summary>
        ///     Tabelle Zurodnung Projekt zu Messdefinition
        /// </summary>
        public virtual DbSet<TableMeasurementDefinitionToProjectAssignment> TblMeasurementDefinitionToProjectAssignments { get; set; } = null!;

        /// <summary>
        ///     Tabelle User
        /// </summary>
        public virtual DbSet<TableUser> TblUsers { get; set; } = null!;

        /// <summary>
        ///     Tabelle Settings
        /// </summary>
        public virtual DbSet<TableSetting> TblSettings { get; set; } = null!;

        /// <summary>
        ///     Tabelle Devices
        /// </summary>
        public virtual DbSet<TableDevice> TblDevices { get; set; } = null!;

        /// <summary>
        ///     Tabelle Files z.B. Userbild
        /// </summary>
        public virtual DbSet<TableFile> TblFiles { get; set; } = null!;

        /// <summary>
        ///     Tabelle Abos
        /// </summary>
        public virtual DbSet<TableAbo> TblAbos { get; set; } = null!;

        /// <summary>
        ///     Tabelle Abos
        /// </summary>
        public virtual DbSet<TableRating> TblRatings { get; set; } = null!;

        /// <summary>
        ///     Tabelle Abos
        /// </summary>
        public virtual DbSet<TableIncomeOutput> TblIncomeOutputs { get; set; } = null!;

        /// <summary>
        ///     Tabelle Abos
        /// </summary>
        public virtual DbSet<TableNotification> TblNotifications { get; set; } = null!;

        /// <summary>
        ///     Tabelle Zugriff für Forschungseinrichtungen auf OData-Schnittstellen
        /// </summary>
        public virtual DbSet<TableResearchInstitutesAccess> TblResearchInstitutesAccess { get; set; } = null!;

        #endregion
    }
}