// (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 12.01.2022 18:22
// Entwickler      Michael Kollegger
// Projekt         IXchange

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BaseApp.Connectivity;
using BaseApp.View.Xamarin.Styles;
using BaseApp.ViewModel;
using Biss.Apps;
using Biss.Apps.Base;
using Biss.Apps.Connectivity.Dc;
using Biss.Apps.Connectivity.XF;
using Biss.Apps.Interfaces;
using Biss.Apps.Map.XF.Extensions;
using Biss.Apps.XF;
using Biss.Apps.XF.Styles;
using Biss.Log.Producer;
using Exchange;
using Exchange.Resources;
using Xamarin.Forms;

namespace BaseApp.View.Xamarin
{
    /// <inheritdoc />
    public partial class App : Application
    {
        private CancellationTokenSource _sleepToken = new CancellationTokenSource();

        /// <summary>
        ///     Xamarin BISS MvvM initialisieren und starten IBissAppPlattform
        /// </summary>
        protected App(IBissAppPlattform plattform)
        {
            // KEIN InitializeComponent aufrufen
            LoadResources();
            if (this.UseBissXf(plattform, AppSettings.Current(), new Language(), typeof(ViewMenu)))
            {
#pragma warning disable CA2000 // Dispose objects before losing scope
                this.BissUseDc(AppSettings.Current(), new DcProjectBase());
#pragma warning restore CA2000 // Dispose objects before losing scope
                this.BissUseSa(AppSettings.Current(), new SaProjectBase());
                
                using var ms = new MemoryStream();
                Images.ReadImageAsStream(EnumEmbeddedImage.Pin_png).CopyTo(ms);
                this.BissUseMaps(AppSettings.Current(),ms.ToArray());

                VmProjectBase.InitializeApp().ConfigureAwait(true);
                Current.UserAppTheme = VmProjectBase.GetVmBaseStatic.Dc.DcExLocalAppData.Data.UseDarkTheme ? OSAppTheme.Dark : OSAppTheme.Light;
                VmProjectBase.LaunchFirstView();
            }
            else
            {
                throw new Exception();
            }
        }

        /// <summary>
        ///     Themes umschalten
        /// </summary>
        /// <param name="useDark"></param>
        public static void SwitchTheme(bool useDark)
        {
            Current.UserAppTheme = useDark ? OSAppTheme.Dark : OSAppTheme.Light;

            VmMain.DesignInstance.Nav.ClearCachedPages();
            // Brauchts nur mehr bei iOS
            if (Device.RuntimePlatform == Device.iOS)
            {
                VmMain.DesignInstance.Nav.ToView(VmBase.GetMainViewModelType(), showMenu: false);
                VmBase.ResetqModeMenuWorkAround();
                VmMain.DesignInstance.Nav.ToView(VmBase.GetMainViewModelType());
            }
        }

        private void LoadResources()
        {
            ICollection<ResourceDictionary> mergedDictionaries = Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null!)
            {
                if (mergedDictionaries.Count > 0)
                {
                    throw new InvalidOperationException("Kein InitializeComponent in der App.xaml.cs ausführen!");
                }

                mergedDictionaries.Clear();
                mergedDictionaries.Add(new StyColors());
                mergedDictionaries.Add(new StyCommon());
                mergedDictionaries.Add(new StyFonts());
                mergedDictionaries.Add(new StyConverterApp());
                mergedDictionaries.Add(new StyBase());
                mergedDictionaries.Add(new StyProject());
            }
        }

 

        #region OnStart/Sleep/Resume

        /// <inheritdoc />
        protected override void OnStart()
        {
            // Handle when your app starts
            base.OnStart();
            VmProjectBase.SubNotifications();
            Logging.Log.LogInfo("XamarinForms: OnStart");
        }

        /// <inheritdoc />
        protected override void OnSleep()
        {
            // Handle when your app sleeps
            base.OnSleep();
            Logging.Log.LogInfo("XamarinForms: OnSleep");

            //DC nach 20s trennen
            _sleepToken = new CancellationTokenSource();
            if (VmProjectBase.DcDoNotAutoDisconnect == false)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(20000, _sleepToken.Token).ConfigureAwait(true);
                    if (!_sleepToken.IsCancellationRequested)
                    {
                        Logging.Log.LogInfo("APP.xaml.cs: Auto Close DC Conntection");
                        var dc = BcDataConnectorExtensions.BcDataConnector(null!)?.GetDc<DcProjectBase>();
                        dc?.CloseConnection(true);
                    }
                });
            }

            VmProjectBase.UnsubNotifications();
        }

        /// <inheritdoc />
        protected override async void OnResume()
        {
            // Handle when your app resumes
            base.OnResume();
            Logging.Log.LogInfo("XamarinForms: OnResume");
            _sleepToken.Cancel();

            // DC bei Bedarf wieder starten
            var dc = BcDataConnectorExtensions.BcDataConnector(null!)?.GetDc<DcProjectBase>();
            if (dc != null)
            {
                if (dc.AutoConnect == false)
                {
                    await dc.OpenConnection(true).ConfigureAwait(true);
                }
            }

            VmProjectBase.SubNotifications();
        }

        #endregion
    }
}