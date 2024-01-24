// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Threading.Tasks;
using BaseApp.Connectivity;
using Biss.Apps.Interfaces;
using Biss.Apps.ViewModel;

namespace BaseApp.ViewModel.ModalVM
{
    /// <summary>
    ///     <para>Kommentar bearbeiten</para>
    /// Klasse VmEditRating. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class VmEditRating : VmProjectBase
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmEditRating.DesignInstance}"
        /// </summary>
        public static VmEditRating DesignInstance = new VmEditRating();

        /// <summary>
        ///     VmEditRating
        /// </summary>
        public VmEditRating() : base("Bewertung editieren")
        {
            SetViewProperties();
        }

        #region Properties

        /// <summary>
        ///     Bewertung.
        /// </summary>
        public DcListTypeRating Rating { get; set; } = null!;

        #endregion

        #region Overrides

        /// <summary>
        ///     OnActivated (2) für View geladen noch nicht sichtbar
        ///     Nur einmal
        /// </summary>
        public override Task OnActivated(object? args = null)
        {
            if (!(args is DcListTypeRating rating))
            {
                throw new ArgumentException("Falsches Argument");
            }

            Rating = rating;
            return base.OnActivated(args);
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdSaveRating = new VmCommand("Bestätigen", async () =>
            {
                if (Rating.Data.Id < 1)
                {
                    Dc.DcExRatings.Add(Rating);
                }

                var storeResult = await Rating.StoreData(true).ConfigureAwait(true);

                if (storeResult.DataOk)
                {
                    InvokeNavBackRequested();
                }
                else
                {
                    var answer = await MsgBox.Show($"{storeResult.ServerExceptionText}<br>Trotzdem schließen?", "Error", VmMessageBoxButton.YesNo, VmMessageBoxImage.Error).ConfigureAwait(true);

                    if (answer is VmMessageBoxResult.Yes)
                    {
                        InvokeNavBackRequested();
                    }
                }
            });
        }

        /// <summary>
        ///     Test Command
        /// </summary>
        public VmCommand CmdSaveRating { get; set; } = null!;

        #endregion
    }
}