// (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 18.10.2023 11:56
// Entwickler      Benjamin Moser (BMo)
// Projekt         IXchange

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using BaseApp.Connectivity;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Apps;
using Biss.Apps.Interfaces;
using Biss.Apps.ViewModel;
using Exchange.Model.ConfigApp;
using Exchange.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BaseApp.View.Xamarin.Controls
{
    /// <summary>
    ///     Measurement Component.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MeasurementsComponent
    {
        /// <summary>
        ///     Measurement Definitions.
        /// </summary>
        public static readonly BindableProperty MeasurementDefinitionsProperty = BindableProperty.Create(
            nameof(MeasurementDefinitions),
            typeof(ICollection<ExMeasurementDefinition>),
            typeof(MeasurementsComponent),
            defaultValue: new List<ExMeasurementDefinition>(),
            propertyChanged: Collection_PropertyChanged
        );

        /// <summary>
        ///     Kann abbonieren.
        /// </summary>
        public static readonly BindableProperty CanSubscribeProperty = BindableProperty.Create(
            nameof(CanSubscribe),
            typeof(bool),
            typeof(MeasurementsComponent)
        );

        /// <summary>
        ///     Measurement Component.
        /// </summary>
        public MeasurementsComponent()
        {
            InitializeComponent();
            CalculateCosts();
        }

        #region Properties

        /// <summary>
        ///     Measurements.
        /// </summary>
        public bool CollectionAny { get; private set; }

        /// <summary>
        ///     Keine Measurements.
        /// </summary>
        public bool CollectionNone { get; private set; }

        /// <summary>
        ///     Measurement Definitions.
        /// </summary>
        public ICollection<ExMeasurementDefinition> MeasurementDefinitions
        {
            get => (ICollection<ExMeasurementDefinition>) GetValue(MeasurementDefinitionsProperty);
            set => SetValue(MeasurementDefinitionsProperty, value);
        }

        /// <summary>
        ///     DESCRIPTION
        /// </summary>
        public VmCommand CmdSubscribe { get; } = new VmCommand(ResViewEditAbo.TxtSubscribe, async arg =>
        {
            IEnumerable<ExMeasurementDefinition> measurementDefinitions = new List<ExMeasurementDefinition>();

            if (arg is IEnumerable<ExMeasurementDefinition> measurementDefinitionsEnumerable)
            {
                measurementDefinitions = measurementDefinitionsEnumerable;
            }

            if (arg is ExMeasurementDefinition measurementDefinition)
            {
                measurementDefinitions = new List<ExMeasurementDefinition> {measurementDefinition};
            }

            var changed = false;
            foreach (var exMeasurementDefinition in measurementDefinitions)
            {
                var mesDefAssignment = VmProjectBase.GetVmBaseStatic.Dc.DcExMeasurementDefinitionAssignments.FirstOrDefault(mA => mA.Data.MeasurementDefinition.Id == exMeasurementDefinition.Id);
                var datapoint = VmProjectBase.GetVmBaseStatic.Dc.DcExAbos.FirstOrDefault(x => x.Data.User.Id == VmProjectBase.GetVmBaseStatic.Dc.DcExUser.Data.Id && x.Data.MeasurementDefinitionAssignment.MeasurementDefinition.Id == mesDefAssignment.Id);

                if (exMeasurementDefinition.IsSelected)
                {
                    //abonnieren wenn noch nicht abonniert
                    if (datapoint is null)
                    {
                        datapoint = new DcListTypeAbo(new ExAbo
                                                      {
                                                          MeasurementDefinitionAssignment = mesDefAssignment.Data,
                                                          User = VmProjectBase.GetVmBaseStatic.Dc.DcExUser.Data,
                                                      });

                        VmProjectBase.GetVmBaseStatic.Dc.DcExAbos.Add(datapoint);
                        changed = true;
                    }
                }
                else
                {
                    //deabonnieren falls abonniert
                    if (datapoint != null)
                    {
                        VmProjectBase.GetVmBaseStatic.Dc.DcExAbos.Remove(datapoint);
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                VmProjectBase.GetVmBaseStatic.View.BusySet(ResCommon.MsgPleaseWait, 0);
                await VmProjectBase.GetVmBaseStatic.Dc.DcExAbos.StoreAll().ConfigureAwait(true);
                await VmProjectBase.GetVmBaseStatic.Dc.DcExAbos.Sync().ConfigureAwait(true);
                VmProjectBase.GetVmBaseStatic.View.BusyClear(true);
            }
            else
            {
                await VmBase.MsgBox.Show(ResViewMain.MsgBoxNoChangesMade, icon: VmMessageBoxImage.Information).ConfigureAwait(true);
            }
        });

        /// <summary>
        ///     Kann abbonieren.
        /// </summary>
        public bool CanSubscribe
        {
            get => (bool) GetValue(CanSubscribeProperty);
            set => SetValue(CanSubscribeProperty, value);
        }

        /// <summary>
        ///     Kosten einer Measurement.
        /// </summary>
        public string MeasurementCosts { get; set; } = string.Empty;

        #endregion

        private static void Collection_PropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is MeasurementsComponent component)
            {
                if (newvalue is ICollection<ExMeasurementDefinition> newcoll)
                {
                    foreach (var exMeasurementDefinition in newcoll)
                    {
                        exMeasurementDefinition.PropertyChanged += component.ExMeasurementDefinition_PropertyChanged;
                    }

                    component.CollectionAny = newcoll.Count > 0;
                    component.CollectionNone = newcoll.Count == 0;
                }
            }
        }

        private void ExMeasurementDefinition_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExMeasurementDefinition.IsSelected))
            {
                CalculateCosts();
            }
        }

        private void CalculateCosts()
        {
            double costs = 0;

            //int counter = 0;

            //while (VmProjectBase.GetVmBaseStatic.Dc.DcExMeasurementDefinitionAssignments.Count == 0 && counter++ <10)
            //{
            //    VmProjectBase.GetVmBaseStatic.Dc.DcExMeasurementDefinitionAssignments.WaitDataFromServerAsync(reload:true, timeout:5000);
            //    Thread.Sleep(1000);
            //}

            //if (VmProjectBase.GetVmBaseStatic.Dc.DcExMeasurementDefinitionAssignments.Count == 0)
            //{
            //    MeasurementCosts = "keine Daten";
            //    return;
            //}
                

            foreach (var exMeasurementDefinition in MeasurementDefinitions.Where(x => x.IsSelected))
            {
                try
                {
                    var mesDefAssignment = VmProjectBase.GetVmBaseStatic.Dc.DcExMeasurementDefinitionAssignments.FirstOrDefault(mA => mA.Data.MeasurementDefinition.Id == exMeasurementDefinition.Id);
                    costs = costs + mesDefAssignment.Data.Costs;
                }
                catch (Exception e)
                {
                }
                
            }

            MeasurementCosts = $"{costs} {ResCommon.TxtIxiesPerDay}";
        }

        private void CheckBox_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (MeasurementDefinitions  == null!)
            {
                return;
            }

            foreach (var dataMeasurementDefinition in MeasurementDefinitions)
            {
                dataMeasurementDefinition.IsSelected = e.Value;
            }
        }
    }
}