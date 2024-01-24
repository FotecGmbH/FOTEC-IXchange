// (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 18.08.2022 10:11
// Entwickler      Georg Wernitznig
// Projekt         BISS.Apps.Core

using Biss.Apps.ViewModel;
using System;
using System.ComponentModel;
using Biss.Apps.XF.Controls.DataTemplates;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BaseApp.View.Xamarin.Controls.DataTemplates
{
    /// <summary>
    /// Template für WPF mit Swipe
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    // ReSharper disable once RedundantExtendsListEntry
    public partial class TemplateWpfGatewaySwipe : CollectionViewTemplate
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public TemplateWpfGatewaySwipe()
        {
            InitializeCommands();
            InitializeComponent();
          
            AttachViews.Add(new AttachableSelectView(GridLeftMargin, Container));
        }


        /// <summary>
        /// WpfSwipe
        /// </summary>
        public VmCommand CmdWpfSwipe { get; private set; } = null!;

        private void InitializeCommands()
        {
            CmdWpfSwipe = new VmCommand(string.Empty, (param) =>
            {
                if (!(param is global::Xamarin.Forms.View view))
                    throw new InvalidOperationException();

                if (view.IsVisible)
                {
                    view.FadeTo(0);
                    view.IsVisible = false;
                }
                else
                {
                    view.IsVisible = true;
                    view.FadeTo(1);
                }


            }, glyph: "\uEE12");
        }
    }
}