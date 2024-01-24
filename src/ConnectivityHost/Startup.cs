// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Biss.Apps;
using Biss.Apps.Base.Connectivity.Interface;
using Biss.Apps.Blazor;
using Biss.Apps.Blazor.Extensions;
using Biss.Apps.Service.Connectivity;
using Biss.Dc.Core;
using Biss.Dc.Transport.Server.SignalR;
using Biss.Log.Producer;
using ConnectivityHost.BaseApp;
using ConnectivityHost.CacheConverter;
using ConnectivityHost.Controllers;
using ConnectivityHost.Helper;
using ConnectivityHost.Services;
using Database.Tables;
using Exchange;
using Exchange.Model.Gateway;
using Exchange.Resources;
using IXchange.GatewayService;
using IXchange.GatewayService.Interfaces;
using IXchange.Service.AppConnectivity.DataConnector;
using IXchange.Service.Com.Base.Helpers;
using IXchange.Service.Com.GRPC.Services;
using IXchange.Service.TriggerAgent;
using IXchangeDatabase;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using NetTopologySuite.Geometries;
using WebExchange.Interfaces;

namespace ConnectivityHost
{
    /// <summary>
    ///     <para>Startup</para>
    /// Klasse Startup. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class Startup
    {
        /// <summary>
        ///     Startup
        /// </summary>
        /// <param name="configuration">Config</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Logging.Log.LogTrace($"[{nameof(Startup)}]({nameof(Startup)}): [ServerApp] Launch App ...");
        }

        #region Properties

        /// <summary>
        ///     Config
        /// </summary>
        public IConfiguration Configuration { get; }

        #endregion

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        ///     For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services">Services</param>
        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddBissLog(l => l.AddDebug().AddConsole().SetMinimumLevel(LogLevel.Information));

            services.InitBissApps(AppSettings.Current());

            //BISS Apps
            if (!VmBase.DisableConnectivityBuildInApp)
            {
                Logging.Log.LogInfo("Blazor: Init started");

                AppSettings.Current().DefaultViewNamespace = "ConnectivityHost.Pages.";
                AppSettings.Current().DefaultViewAssembly = "ConnectivityHost";

                var init = new BissInitializer();
                init.Initialize(AppSettings.Current(), new Language());

                VmProjectBase.InitializeApp().ConfigureAwait(true);

                Logging.Log.LogInfo("Blazor: Init finished");
            }

            services.AddCors(options =>
                options.AddDefaultPolicy(builder =>
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()));
            services.AddRazorPages();
            services.AddServerSideBlazor();

            //Datenbank
            services.AddDbContext<Db>();

            //Connectivity
            services.AddDcSignalRCore<IServerRemoteCalls>(typeof(ServerRemoteCalls))
                .AddHubOptions<DcCoreHub<IServerRemoteCalls>>(o =>
                {
                    o.MaximumReceiveMessageSize = null;
                    o.EnableDetailedErrors = true;
                    o.MaximumParallelInvocationsPerClient = 5;
                });

            // configure DI for application services
            services.AddScoped<IAuthUserService, AuthUserService>();


            // Hintergrund-Service
            services.AddHostedService<BackgroundService<IServerRemoteCalls>>();

            // Authentication
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            //services.AddScoped<ILocalStorageService, LocalStorageService>();

            services.AddSingleton<IDbProjectCacheConverter>(new DbProjectCacheConverter());

            services.AddMvc().AddControllersAsServices();

            services.AddScoped<CancellationTokenSource>();

            services.AddScoped<BackgroundIxiesWorker>();
            services.AddScoped<BackgroundTriggerWorker>();

            services.AddControllers(o => { o.Conventions.Add(new ActionHidingConvention()); }).AddOData(oDataOptions =>
            {
                oDataOptions
                    .Select().Filter().OrderBy().Expand().Count().SetMaxTop(null)
                    .AddRouteComponents("api/odata", GetEdmModel());
            });

            services.AddRestAccess(new RestAccessService());

            services.AddGrpc();

            //Gatway Daten
            services.AddSingleton<IGatewayConnectedClientsManager, GatewayConnectedClientsManager>();

            //TriggerAgent
            services.AddSingleton<ITriggerAgent, TriggerAgent>();

            try
            {
                services.AddSwaggerGen(c =>
                {
                    try
                    {
                        c.SwaggerDoc("v1", new OpenApiInfo {Title = "IXchange API", Version = "v1", Description = "API für den Zugriff der gesammelten Daten von IXchange. <br> Die API ist zugriffsgeschüzt. Durch Klick auf den Authorize Button erscheint ein Fenster, in diesem ein Token angegeben werden kann. Der Token kann in der IXchange - Demo Applikation - im Bereich 'ICH' erzeugt werden.<br>"});
                        // using System.Reflection;
                        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "IXchange.Service.Com.Rest.xml"));
                        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "IXchange.Service.Com.Base.xml"));
                        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Exchange.xml"));
                        //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Exchange.xml"));

                        //Security
                        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                                                          {
                                                              Name = "Authorization",
                                                              Type = SecuritySchemeType.ApiKey,
                                                              Scheme = "Bearer",
                                                              BearerFormat = "Token",
                                                              In = ParameterLocation.Header,
                                                              Description = "Zugriffstoken hier einfügen - Wird vom Konfigurationstool zur Verfügung gestellt"
                                                          });
                        c.AddSecurityRequirement(new OpenApiSecurityRequirement
                                                 {
                                                     {
                                                         new OpenApiSecurityScheme
                                                         {
                                                             Reference = new OpenApiReference
                                                                         {
                                                                             Type = ReferenceType.SecurityScheme,
                                                                             Id = "Bearer"
                                                                         }
                                                         },
                                                         new string[] { }
                                                     }
                                                 });
                    }
                    catch (Exception e)
                    {
                        Logging.Log.LogError($"[{nameof(Startup)}]({nameof(ConfigureServices)}): {e}");
                    }
                });
            }
            catch (Exception e)
            {
                Logging.Log.LogError($"[{nameof(Startup)}]({nameof(ConfigureServices)}): {e}");
            }
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 v1"));

            app.UseCors();
            app.UseStaticFiles();

            app.UseRouting();

            //Authorization & Authentication
            app.UseAuthentication();
            app.UseAuthorization();

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapControllers();
                endpoints.MapGrpcService<CompanyService>();
                endpoints.MapGrpcService<MeasurementResultService>();
                endpoints.MapGrpcService<MeasurementDefinitionService>();
                endpoints.MapGrpcService<ProjectService>();
                endpoints.MapGrpcService<GeoService>();
            });

            //DC Connectivity
            app.UseEndpoints(endpoints => { endpoints.MapHub<DcCoreHub<IServerRemoteCalls>>(DcHelper.DefaultHubRoute); });

            //Gateway Hub
            app.UseEndpoints(endpoints => { endpoints.MapHub<GatewayHub>($"/{GatewayConstants.HubName}"); });
            //Db.DeleteDatabase();
            //Db.CreateAndFillUp(); //IM ECHTBETRIEB AUSKOMMENTIEREN
            //Db.SetupTemplates();
        }

        private IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            builder.Ignore(typeof(Point));

            builder.EntitySet<TableIotDevice>(nameof(TableIotDevice));
            builder.EntitySet<TableMeasurementDefinition>(nameof(TableMeasurementDefinition));
            builder.EntitySet<TableMeasurementResult>(nameof(TableMeasurementResult)).EntityType.Ignore(x => x.SpatialPoint);

            var measurementResults = builder.EntitySet<TableMeasurementResult>(nameof(TableMeasurementResult));
            measurementResults.EntityType.Ignore(x => x.SpatialPoint);

            builder.EntityType<TableMeasurementResult>().Ignore(x => x.SpatialPoint);

            //builder.EntityType<TableMeasurementResult>().Property(x => x.SpatialPoint).IsNotExpandable().IsNotNavigable();

            //builder.AddComplexType(typeof(Point));

            //var measurementResult = builder.EntityType<TableMeasurementResult>();
            //measurementResult.Ignore(x => x.SpatialPoint);
            //measurementResult.Property(x => x.SpatialPoint.X);
            //measurementResult.ComplexProperty(x => x.SpatialPoint);

            //var point = builder.ComplexType<Point>();
            //point.Ignore(p => p.Coordinates);
            //point.Ignore(p => p.CoordinateSequence);
            //point.Ignore(p => p.Coordinate);
            //point.Ignore(p => p.Z);
            //point.Ignore(p => p.M);
            //point.Ignore(p => p.SRID);
            //point.Ignore(p => p.Boundary);
            //point.Ignore(p => p.BoundaryDimension);
            //point.Ignore(p => p.Dimension);
            //point.Ignore(p => p.GeometryType);
            //point.Ignore(p => p.OgcGeometryType);
            //point.Ignore(p => p.PointOnSurface);
            //point.Ignore(p => p.Area);
            //point.Ignore(p => p.Centroid);
            //point.Ignore(p => p.Envelope);

            //var coordinateSequence = builder.ComplexType<CoordinateSequence>();
            //coordinateSequence.Property(c => c.Dimension);
            //coordinateSequence.Property(c => c.Measures);
            //coordinateSequence.Property(c => c.Spatial);
            ////coordinateSequence.Property(c => c.Ordinates);
            //coordinateSequence.Property(c => c.HasZ);
            //coordinateSequence.Property(c => c.HasM);
            //coordinateSequence.Property(c => c.ZOrdinateIndex);
            //coordinateSequence.Property(c => c.MOrdinateIndex);
            //coordinateSequence.Property(c => c.Count);

            return builder.GetEdmModel();
        }
    }

    /// <summary>
    /// Kontroller auf Swagger UI verbergen
    /// </summary>
    public class ActionHidingConvention : IActionModelConvention
    {
        #region Interface Implementations

        /// <summary>
        /// Durchführen
        /// </summary>
        /// <param name="action">Action</param>
        public void Apply(ActionModel action)
        {
            //Liste der Controller, die versteckt werden sollen
            List<string> lstHideControllers = new();
            lstHideControllers.Add("Authentication");
            lstHideControllers.Add("Info");
            lstHideControllers.Add("WebLinks");

            // Controller verstecken
            if (lstHideControllers.Contains(action.Controller.ControllerName))
            {
                action.ApiExplorer.IsVisible = false;
            }
        }

        #endregion
    }
}