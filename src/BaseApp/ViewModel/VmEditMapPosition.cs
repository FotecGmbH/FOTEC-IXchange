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
using Biss.Apps.Attributes;
using Biss.Apps.Interfaces;
using Biss.Apps.Map.Base;
using Biss.Apps.Map.Component;
using Biss.Apps.Map.Helper;
using Biss.Apps.Map.Model;
using Biss.Apps.ViewModel;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    /// <para>Map Position bearbeiten</para>
    /// Klasse VmEditMapPosition. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewEditMapPosition")]
    public class VmEditMapPosition : VmProjectBase
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmEditMapPosition.DesignInstance}"
        /// </summary>
        public static VmEditMapPosition DesignInstance = new VmEditMapPosition();

        /// <summary>
        ///     VmEditMapPosition
        /// </summary>
        public VmEditMapPosition() : base(string.Empty)
        {
            SetViewProperties();
        }

        #region Properties

        /// <summary>
        ///     Wrapper BissMap
        /// </summary>
        public BissMap Map => this.BcBissMap()!.BissMap;

        /// <summary>
        /// Aktuelle Position
        /// </summary>
        public BissPosition Position { get; set; } = new BissPosition();

        #endregion

        /// <summary>
        /// Anhaengen / Abhaengen events
        /// </summary>
        /// <param name="attach">an oder abhaengen</param>
        private void AttachDetachVmEvents(bool attach)
        {
            if (attach)
            {
                Map.CoordinatesClicked += MapOnCoordinatesClicked;
            }
            else
            {
                Map.CoordinatesClicked -= MapOnCoordinatesClicked;
            }
        }

        /// <summary>
        /// Klick auf karte
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">event argumente</param>
        private void MapOnCoordinatesClicked(object sender, CoordinatesClickEventArgs e)
        {
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            Position = e?.Position ?? Position;

            UpdateMapItem();
        }

        /// <summary>
        /// Map Items updaten
        /// </summary>
        private void UpdateMapItem()
        {
            Map.MapItems.Clear();
            var point = new BmPoint("Position")
                        {
                            Position = Position,
                            IsVisible = true,
                            IsDraggable = true,
                            Index = 1,
                        };

            point.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(point.Position))
                {
                    Position = point.Position;
                }
            };

            Map.MapItems.Add(point);
        }


        #region Overrides

        /// <summary>
        ///     OnActivated (2) für View geladen noch nicht sichtbar
        ///     Nur einmal
        /// </summary>
        public override async Task OnActivated(object? args = null)
        {
            await base.OnActivated(args);

            if (args is BissPosition position)
            {
                Position = position;
            }
            //Todo: funktioniert noch nicht -> Getuserlocation returned nie
            //var loc = await this.BcBissMap()!.GetUserLocation().ConfigureAwait(true);
            //if (loc != null)
            //{
            //    Position = new BissPosition(loc.Latitude, loc.Longitude, loc.Altitude);
            //}
        }

        /// <summary>
        ///     OnLoaded (3) für View geladen
        ///     Jedes Mal wenn View wieder sichtbar
        /// </summary>
        public override async Task OnLoaded()
        {
            await base.OnLoaded().ConfigureAwait(true);

            UpdateMapItem();

            AttachDetachVmEvents(true);

            await Task.Delay(1).ConfigureAwait(true); //momentan notwendig da map sonst noch nicht fertig geladen
            Map.SetCenterAndZoom(Position, BmDistance.FromKilometers(5), false);
        }

        /// <summary>
        ///     OnDisappearing (4) wenn View unsichtbar / beendet wird
        ///     Nur einmal
        /// </summary>
        public override Task OnDisappearing(IView view)
        {
            AttachDetachVmEvents(false);
            return base.OnDisappearing(view);
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdOk = new VmCommand(ResCommon.CmdOk, async () =>
            {
                ViewResult = Position;

                await Nav.Back().ConfigureAwait(true);
            });

            CmdAbort = new VmCommand(ResCommon.CmdBack, async () =>
            {
                ViewResult = null;

                await Nav.Back().ConfigureAwait(true);
            });
        }

        /// <summary>
        ///     Ok Command
        /// </summary>
        public VmCommand CmdOk { get; set; } = null!;

        /// <summary>
        ///     Ok Command
        /// </summary>
        public VmCommand CmdAbort { get; set; } = null!;

        #endregion
    }
}