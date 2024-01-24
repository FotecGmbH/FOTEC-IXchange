// (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 05.10.2023 14:51
// Entwickler      Roman Jahn (RJa)
// Projekt         IXchange

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace BaseApp.View.Xamarin.View
{
    public partial class ViewIncomeOutput
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public ViewIncomeOutput() : this (null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
        }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public ViewIncomeOutput(object args = null) : base(args)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
            InitializeComponent();
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member