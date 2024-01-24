// (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 16.10.2021 15:28
// Entwickler      Michael Kollegger
// Projekt         IXchange

using BaseApp.ViewModel;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace BaseApp.View.Xamarin.View
{
    public partial class ViewSettings
    {
        public ViewSettings() : this(null)
        {
            ViewModel.SwitchTheme += ViewModelOnSwitchTheme;
        }

        public ViewSettings(object? args = null) : base(args)
        {
            InitializeComponent();
            ViewModel.SwitchTheme += ViewModelOnSwitchTheme;
        }

        private void ViewModelOnSwitchTheme(object sender, EventArg<bool> e)
        {
            App.SwitchTheme(e.Data);
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member