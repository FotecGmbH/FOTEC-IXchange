// (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 09.10.2023 14:17
// Entwickler      Roman Jahn (RJa)
// Projekt         IXchange


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace BaseApp.View.Xamarin.View
{
    /// <summary>
    /// ViewMyRatings
    /// </summary>
    public partial class ViewMyRatings
    {
        /// <summary>
        /// ViewMyRatings
        /// </summary>
        public ViewMyRatings() : this (null!)
        {
        }

        /// <summary>
        /// ViewMyRatings
        /// </summary>
        /// <param name="args"></param>
        public ViewMyRatings(object args = null!) : base(args)
        {
            InitializeComponent();
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member