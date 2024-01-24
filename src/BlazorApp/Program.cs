// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 23.1.2024 10:37
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using BaseApp;
using BaseApp.Connectivity;
using Biss.Apps.Blazor;
using Biss.Apps.Blazor.Extensions;
using Biss.Apps.Connectivity.Blazor;
using Biss.Apps.Connectivity.Blazor.Extensions;
using Biss.Apps.Map.Blazor.Extensions;
using Biss.Apps.Map.Blazor.Interfaces;
using Biss.Apps.Map.Blazor.OpenStreetMap;
using Biss.Log.Producer;
using Exchange;
using Exchange.Resources;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Radzen;

namespace BlazorApp
{
    /// <summary>
    /// Program Klasse
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main Method
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            //await Task.Delay(10000);

            //if(Debugger.IsAttached)
            //    Debugger.Break();

            var indexed = true;
            var sw = new Stopwatch();
            sw.Start();

            Logging.Init(l => l.AddDebug().SetMinimumLevel(LogLevel.Information));

            //Initialisierung
            Logging.Log.LogInfo("Blazor: Init started");

            _ = Constants.AppConfiguration;
            AppSettings.Current().DefaultViewNamespace = "BlazorApp.Pages.";
            AppSettings.Current().DefaultViewAssembly = "BlazorApp";

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddBissLog(l => l.AddDebug().SetMinimumLevel(LogLevel.Information));

            builder.Services.AddSingleton(s => (s.GetRequiredService<IJSRuntime>() as IJSInProcessRuntime)!);

            builder.Services.InitBissApps(AppSettings.Current());
            builder.Services.BissUseDc(AppSettings.Current(), new DcProjectBase(), indexed);
            builder.Services.BissUseSa(AppSettings.Current(), new SaProjectBase());
            builder.Services.InitBissAppsConnectivity(AppSettings.Current(), builder.HostEnvironment.BaseAddress, indexed);
            builder.Services.AddBissMap(AppSettings.Current(), new List<IBlazorMapProvider> {new OpenStreetMapProvider()}, "/content/Pin.png", "content/LocationPin.png");
            builder.Services.AddScoped<DialogService>();

            var init = new BissInitializer();
            init.Initialize(AppSettings.Current(), new Language());

            await VmProjectBase.InitializeApp().ConfigureAwait(true);

            sw.Stop();
            Debug.WriteLine($"[{nameof(Program)}]({nameof(Main)}): Finish - {sw.ElapsedMilliseconds} ms");

            await builder.Build().RunAsync().ConfigureAwait(true);
        }
    }
}