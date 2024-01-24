using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDA.Common.Exchange.Configs.GlobalConfigs;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BaseApp.View.Xamarin.Controls
{
    /// <summary>
    /// View für Global Config TTN
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GcTtnView : ContentView
    {

        /// <summary>
        /// 
        /// </summary>
        public static BindableProperty GlobalConfigProperty = BindableProperty
            .Create(nameof(GlobalConfig),
                typeof(GcTtn),
                typeof(GcTtnView),
                null!);


        /// <summary>
        /// Konstruktor
        /// </summary>
        public GcTtnView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Globale Konfig fürs Binding
        /// </summary>
        public GcTtn GlobalConfig
        {
            get => (GcTtn) GetValue(GlobalConfigProperty);
            set => SetValue(GlobalConfigProperty, value);
        }
    }
}