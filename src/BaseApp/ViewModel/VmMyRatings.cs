// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Linq;
using System.Threading.Tasks;
using BaseApp.Connectivity;
using BaseApp.ViewModel.ModalVM;
using Biss.Apps.Attributes;
using Biss.Apps.Collections;
using Biss.Apps.Enum;
using Biss.Apps.Interfaces;
using Biss.Common;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    /// <para>Bewertungen des Users</para>
    /// Klasse VmMyRatings. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewMyRatings")]
    public class VmMyRatings : VmProjectBase
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmMyRatings.DesignInstance}"
        /// </summary>
        public static VmMyRatings DesignInstance = new VmMyRatings();

        /// <summary>
        ///     VmMyRatings
        /// </summary>
        public VmMyRatings() : base(ResViewMyRatings.LblPageTitle)
        {
            SetViewProperties();
        }

        #region Overrides

        /// <summary>
        ///     OnActivated (2) für View geladen noch nicht sichtbar
        ///     Nur einmal
        /// </summary>
        public override async Task OnActivated(object? args = null)
        {
            //nur eigene Bewertungen rausfiltern
#pragma warning disable CS0618 // Type or member is obsolete
            await Dc.DcExRatings.WaitDataFromServerAsync(reload: true) /*.Sync()*/.ConfigureAwait(true);
#pragma warning restore CS0618 // Type or member is obsolete

            //////  Workaround duplicates in dc list  but not in db /////////////
            var duplicates = Dc.DcExRatings.GroupBy(r => r.Id).SelectMany(r => r.Skip(1));
            foreach (var dcListTypeRating in duplicates)
            {
                Dc.DcExRatings.Remove(dcListTypeRating);
            }

            Dc.DcExRatings.FilterList(x => x.Data.User.Id == Dc.DcExUser.Data.Id);

            await base.OnActivated(args).ConfigureAwait(true);
        }

        /// <summary>
        ///     OnLoaded (3) für View geladen
        ///     Jedes Mal wenn View wieder sichtbar
        /// </summary>
        public override Task OnLoaded()
        {
            AttachDetachEvents(true);
            return base.OnLoaded();
        }

        /// <summary>
        ///     OnDisappearing (4) wenn View unsichtbar / beendet wird
        ///     Nur einmal
        /// </summary>
        public override Task OnDisappearing(IView view)
        {
            AttachDetachEvents(false);
            return base.OnDisappearing(view);
        }

        #endregion

        #region Commands

        /// <summary>
        /// An/Abhaengen der events
        /// </summary>
        /// <param name="attach">Ob An oder Abhaengen</param>
        private void AttachDetachEvents(bool attach)
        {
            if (attach)
            {
                Dc.DcExRatings.CollectionEvent += DcExRatings_CollectionEvent;
            }
            else
            {
                Dc.DcExRatings.CollectionEvent -= DcExRatings_CollectionEvent;
            }
        }

        /// <summary>
        /// Kommentar collection aenderung
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        private async Task DcExRatings_CollectionEvent(object sender, CollectionEventArgs<DcListTypeRating> e)
        {
            switch (e.TypeOfEvent)
            {
                case EnumCollectionEventType.EditRequest:
                {
                    if (DeviceInfo.Plattform == EnumPlattform.Web)
                    {
                        Dc.DcExRatings.SelectedItem = e.Item;
                    }
                    else
                    {
                        View.DetailView.ShowVmDetail(typeof(VmEditRating), e.Item);
                        await Dc.DcExRatings.Sync().ConfigureAwait(true);
                    }

                    break;
                }


                case EnumCollectionEventType.DeleteRequest:
                {
                    // ReSharper disable once RedundantSuppressNullableWarningExpression
                    var answer = await MsgBox!.Show(ResViewMyRatings.MsgConfirmDelete, button: VmMessageBoxButton.YesNo, icon: VmMessageBoxImage.Warning).ConfigureAwait(true);

                    if (answer != VmMessageBoxResult.Yes)
                    {
                        return;
                    }

                    Dc.DcExRatings.Remove(e.Item);

                    View.BusySet($"{ResCommon.MsgPleaseWait}...");
                    var deleteResult = await Dc.DcExRatings.StoreAll().ConfigureAwait(true);
                    View.BusyClear(true);

                    if (!deleteResult.DataOk)
                    {
                        var task = Dc.DcExRatings.Sync().ConfigureAwait(true);

                        await MsgBox.Show($"{ResViewMyRatings.MsgFailedToDeleteRating}<br>{deleteResult.ServerExceptionText}", button: VmMessageBoxButton.Ok, icon: VmMessageBoxImage.Error).ConfigureAwait(true);

                        await task;
                    }
                    else
                    {
                        await MsgBox.Show($"{ResViewMyRatings.MsgRatingDeleted}", button: VmMessageBoxButton.Ok, icon: VmMessageBoxImage.Information).ConfigureAwait(true);
                    }

                    break;
                }
            }
        }

        #endregion
    }
}