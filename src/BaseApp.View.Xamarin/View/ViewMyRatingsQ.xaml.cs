// (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 09.10.2023 14:25
// Entwickler      Roman Jahn (RJa)
// Projekt         IXchange

using System;
using Xamarin.Forms.Xaml;

namespace BaseApp.View.Xamarin.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewMyRatingsQ
    {
    public ViewMyRatingsQ() : this (null)
    {
    }

    public ViewMyRatingsQ(object args = null) : base(args)
    {
        InitializeComponent();
    }
    }
}