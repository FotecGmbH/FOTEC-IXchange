// (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 16.10.2021 15:28
// Entwickler      Michael Kollegger
// Projekt         IXchange

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace AndroidApp
{
    /// <summary>
    ///     <para>SplashActivity - Erste Aktivität bei Android</para>
    ///     Klasse SplashActivity. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Activity(MainLauncher = true, Theme = "@style/MainTheme.Splash", NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
    public class SplashActivity : Activity
    {
        /// <summary>
        /// Beim Erstellen
        /// </summary>
        /// <param name="bundle">bundle</param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var openMainActivity = new Intent(BaseContext, typeof(MainActivity));
            openMainActivity.SetFlags(ActivityFlags.ReorderToFront);
            if (bundle != null)
            {
                openMainActivity.PutExtras(bundle);
            }

            StartActivityIfNeeded(openMainActivity, 0);
            Finish();
        }
    }
}