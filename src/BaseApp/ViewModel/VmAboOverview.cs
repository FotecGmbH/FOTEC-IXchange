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
using Biss.Apps.Attributes;
using Biss.Apps.Collections;
using Biss.Apps.Enum;
using Biss.Apps.Interfaces;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    /// <para>Viewmodel für ViewAboOverview</para>
    /// Klasse VmAboOverview. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewAboOverview")]
    public class VmAboOverview : VmProjectBase
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmAboOverview.DesignInstance}"
        /// </summary>
        public static VmAboOverview DesignInstance = new VmAboOverview();

        /// <summary>
        ///     VmAboOverview
        /// </summary>
        public VmAboOverview() : base(ResViewAboOverview.LblPageTitle)
        {
            SetViewProperties();
        }

        #region Overrides

        /// <summary>
        ///     OnLoaded (3) für View geladen
        ///     Jedes Mal wenn View wieder sichtbar
        /// </summary>
        public override async Task OnLoaded()
        {
            await Dc.DcExAbos.Sync().ConfigureAwait(true);
            //////  Workaround duplicates in dc list  but not in db /////////////
            var duplicates = Dc.DcExAbos.GroupBy(r => r.Id).SelectMany(r => r.Skip(1));
            foreach (var dcListTypeAbo in duplicates)
            {
                Dc.DcExAbos.Remove(dcListTypeAbo);
            }

            AttachDetachEvents(true);
            await base.OnLoaded().ConfigureAwait(true);
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
        /// Anhaengen/Abhaengen der events
        /// </summary>
        /// <param name="attach">an oder abhaengen</param>
        private void AttachDetachEvents(bool attach)
        {
            if (attach)
            {
                Dc.DcExAbos.CollectionEvent += DcExAbos_CollectionEvent;
            }
            else
            {
                Dc.DcExAbos.CollectionEvent -= DcExAbos_CollectionEvent;
            }
        }

        /// <summary>
        /// Collection Aenderung dc ex abos
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argumente</param>
        /// <returns></returns>
        private async Task DcExAbos_CollectionEvent(object sender, CollectionEventArgs<DcListTypeAbo> e)
        {
            switch (e.TypeOfEvent)
            {
                case EnumCollectionEventType.EditRequest:
                {
                    // ReSharper disable once UnusedVariable
                    var r = await Nav.ToViewWithResult(typeof(VmEditAbo), e.Item).ConfigureAwait(true);
                    await View.RefreshAsync().ConfigureAwait(true);
                    break;
                }

                case EnumCollectionEventType.DeleteRequest:
                {
                    var answer = await MsgBox.Show("Wirklich löschen", button: VmMessageBoxButton.YesNo, icon: VmMessageBoxImage.Warning).ConfigureAwait(true);

                    if (answer != VmMessageBoxResult.Yes)
                    {
                        return;
                    }

                    if (Dc.DcExAbos.Remove(e.Item))
                    {
                        View.BusySet("Wird gelöscht...", 0);
                        var result = await Dc.DcExAbos.StoreAll().ConfigureAwait(true);
                        View.BusyClear(true);

                        if (!result.DataOk)
                        {
                            Dc.DcExAbos.Add(e.Item);
                            await MsgBox.Show($"{result.ServerExceptionText}", "Fehler beim Löschen", icon: VmMessageBoxImage.Error).ConfigureAwait(true);
                        }
                    }

                    // ReSharper disable once RedundantSuppressNullableWarningExpression
                    await MsgBox!.Show("noch nicht implementiert").ConfigureAwait(true);
                    break;
                }
            }
        }

        #endregion
    }
}