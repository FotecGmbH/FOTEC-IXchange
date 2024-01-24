// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using IXchange.Service.Com.Base.Helpers;
using IXchange.Service.Com.GRPC.Services;
using IXchangeDatabase;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<Db>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<CompanyService>();
app.MapGrpcService<MeasurementResultService>();
app.MapGrpcService<MeasurementDefinitionService>();
app.MapGrpcService<ProjectService>();
app.MapGrpcService<GeoService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");


app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();

app.Run();