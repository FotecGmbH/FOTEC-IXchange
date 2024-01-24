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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BaseApp.Connectivity;
using BaseApp.Helper;
using Biss.Apps.Interfaces;
using Biss.Apps.ViewModel;
using Biss.Collections;
using Biss.Common;
using Biss.Serialize;
using Exchange;
using BDA.Common.Exchange.Model.ConfigApp;
using Exchange.Enum;
using Exchange.Extensions;
using Exchange.Model.ConfigApp;
using Exchange.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BaseApp.ViewModel.Infrastructure;

namespace BaseApp.View.Xamarin.Controls
{
    /// <summary>
    ///     Measurement Definition Details.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MeasurementDetailsComponent
    {
        /// <summary>
        ///     Measurement Definition.
        /// </summary>
        public static readonly BindableProperty MeasurementDefinitionProperty = BindableProperty.Create(
            nameof(MeasurementDefinition),
            typeof(ExMeasurementDefinition),
            typeof(MeasurementDetailsComponent)
        );

        /// <summary>
        ///     Ist selektierbar
        /// </summary>
        public static readonly BindableProperty IsSelectableProperty = BindableProperty.Create(
            nameof(IsSelectable),
            typeof(bool),
            typeof(MeasurementDetailsComponent)
        );

        /// <summary>
        ///     Ist editierbar.
        /// </summary>
        public static readonly BindableProperty IsEditableProperty = BindableProperty.Create(
            nameof(IsEditable),
            typeof(bool),
            typeof(MeasurementDetailsComponent)
        );

        /// <summary>
        /// Messwerte
        /// </summary>
        public ObservableCollection<ExMeasurement> CachedMeasurements
        {
            get;
        } = new ObservableCollection<ExMeasurement>();

        /// <summary>
        /// Current Page
        /// </summary>
        // ReSharper disable once RedundantDefaultMemberInitializer
        public int CurrentPage { get; set; } = 0;

        /// <summary>
        /// Pages
        /// </summary>
        // ReSharper disable once RedundantDefaultMemberInitializer
        public int Pages { get; set; } = 0;

        /// <summary>
        /// MeasurementDefinitionAssignment
        /// </summary>
        public ExMeasurementDefinitionAssignment MeasurementDefinitionAssignment
        {
            get
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (MeasurementDefinition != null)
                {
                    return ExMesDefAssignHelper.GetAssignment(MeasurementDefinition, VmProjectBase.GetVmBaseStatic.Dc).Data;
                }
                else
                {
                    return new ExMeasurementDefinitionAssignment();
                }
                
            }
        }


        /// <summary>
        /// Messwerte
        /// </summary>
        public ObservableCollection<ExMeasurement> Measurements
        {
            get;
        } = new ObservableCollection<ExMeasurement>();

        /// <summary>
        /// ForceMeasurementsUpdate
        /// </summary>
        public bool ForceMeasurementsUpdate { get; set; } = true;


        /// <summary>
        /// PickerTimePeriod
        /// </summary>
        public VmPicker<EnumTimePeriod> PickerTimePeriod { get; set; } = new VmPicker<EnumTimePeriod>(nameof(PickerTimePeriod));

        /// <summary>
        ///  Measurement Definition Details.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MeasurementDetailsComponent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

            InitializeComponent();

            foreach (var value in EnumUtil.GetValues<EnumTimePeriod>())
            {
                PickerTimePeriod.AddKey(value, value.ToString());
            }

            PickerTimePeriod.SelectKey(EnumTimePeriod.Day);

            PickerTimePeriod.SelectedItemChanged += PickerTimePeriod_SelectedItemChanged; // garbage collector sollte sich um detach kuemmern

        }

        private void PickerTimePeriod_SelectedItemChanged(object sender, SelectedItemEventArgs<VmPickerElement<EnumTimePeriod>> e)
        {
            GetMeasurements().Wait();
        }



        #region Properties

        /// <summary>
        ///     Measurement Definition.
        /// </summary>
        public ExMeasurementDefinition MeasurementDefinition
        {
            get => (ExMeasurementDefinition)GetValue(MeasurementDefinitionProperty);
            set => SetValue(MeasurementDefinitionProperty, value);
        }

        /// <summary>
        ///     Ist selektierbar
        /// </summary>
        public bool IsSelectable
        {
            get => (bool)GetValue(IsSelectableProperty);
            set => SetValue(IsSelectableProperty, value);
        }

        /// <summary>
        ///     Ist editierbar.
        /// </summary>
        public bool IsEditable
        {
            get => (bool)GetValue(IsEditableProperty);
            set => SetValue(IsEditableProperty, value);
        }

        /// <summary>
        ///     Letztes Update.
        /// </summary>
        public string LastUpdate
        {
            get
            {
                // ReSharper disable once ConvertIfStatementToReturnStatement
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (MeasurementDefinition == null) // Wokraround Timing Measurementdefinition comes after it is requested
                    return "";
                else
                {
                    return $"{ResCommon.TxtLastValue}: {MeasurementDefinition.Information.UpdatedDate.ToString("dd.MM.yyyy hh:mm") ?? DateTime.MinValue.ToString("dd.MM.yyyy hh:mm")}";

                }

            }
        }

        /// <summary>
        ///     Editier Command.
        /// </summary>
        public VmCommand EditCommand => new VmCommand(VmProjectBase.GetVmBaseStatic.Dc.DcExMeasurementDefinition.CmdEditItem.DisplayName, async () =>
        {
            var listPoint = GetListPoint();

            if (listPoint is null)
            {
                return;
            }

            // ReSharper disable once UnusedVariable
            var r = await VmProjectBase.GetVmBaseStatic.Nav.ToViewWithResult(typeof(VmEditMeasurementDefinition), listPoint);
        }, glyph: VmProjectBase.GetVmBaseStatic.Dc.DcExMeasurementDefinition.CmdEditItem.Glyph);

        /// <summary>
        ///     Lösch Command.
        /// </summary>
        public VmCommand DeleteCommand => new VmCommand(VmProjectBase.GetVmBaseStatic.Dc.DcExMeasurementDefinition.CmdRemoveItem.DisplayName, () =>
        {
            var listPoint = GetListPoint();

            if (listPoint is null)
            {
                return;
            }

            VmProjectBase.GetVmBaseStatic.Dc.DcExMeasurementDefinition.CmdRemoveItem.Execute(listPoint);
        }, glyph: VmProjectBase.GetVmBaseStatic.Dc.DcExMeasurementDefinition.CmdRemoveItem.Glyph);

        /// <summary>
        ///     Zeige die Ratings.
        /// </summary>
        public bool ShowRatings { get; set; }

        /// <summary>
        ///     Zeige Ratings an.
        /// </summary>
        public VmCommand CmdShowRatings => new VmCommand(string.Empty, () => { ShowRatings = !ShowRatings; });

        /// <summary>
        /// CanSeeMore
        /// </summary>
        public bool CanSeeMore
        {
            get
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (MeasurementDefinition == null)
                    return false;
                else
                {
                    return IsEditable || MeasurementDefinition.IsSelected;
                }
            }
        }

        /// <summary>
        /// CmdNavigateLeft
        /// </summary>
        public VmCommand CmdNavigateLeft => new VmCommand("<", () =>
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdatePages();
            }
        });

        /// <summary>
        /// CmdNavigateRight
        /// </summary>
        public VmCommand CmdNavigateRight => new VmCommand(">", () =>
        {
            if (CurrentPage < Pages)
            {
                CurrentPage++;
                UpdatePages();
            }
        });

        #endregion

        private DcListTypeMeasurementDefinition? GetListPoint()
        {
            return VmProjectBase.GetVmBaseStatic.Dc.DcExMeasurementDefinition.FirstOrDefault(x => x.Data.Id == MeasurementDefinition.Id);
        }


        /// <summary>
        /// Text wenn keine Werte vorhanden oder Error bei Abruf
        /// </summary>
        public string NoValuesText { get; set; }

        private void UpdatePages()
        {
            Measurements.Clear();
            foreach (var exMeasurement in CachedMeasurements.Skip((CurrentPage - 1) * 20).Take(20))
            {
                Measurements.Add(exMeasurement);
            }

        }


        private async Task GetMeasurements()
        {
            DateTime fromDate;
            DateTime toDate;

            switch (PickerTimePeriod.SelectedItem!.Key)
            {
                case EnumTimePeriod.Day:
                    fromDate = DateTime.Today;
                    break;
                case EnumTimePeriod.Week:
                    fromDate = DateTime.Today.AddDays(-7);
                    break;
                case EnumTimePeriod.Month:
                    fromDate = DateTime.Today.AddMonths(-1);
                    break;
                case EnumTimePeriod.Year:
                    fromDate = DateTime.Today.AddYears(-1);
                    break;
                case EnumTimePeriod.All:
                    fromDate = DateTime.MinValue;
                    break;
                default:
                    return;

            }

            //Enddatum immer heute 23:59:59
            toDate = DateTime.Today.AddDays(1).AddSeconds(-1);

            string requestString = $"{AppSettings.Current().DcSignalHost}/api/measurementresult/timeperiod/{MeasurementDefinition.Id.ToString()}/{fromDate.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)}/{toDate.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)}";

            // ReSharper disable once RedundantAssignment
            List<ExMeasurement> newMeasurementsResult = new List<ExMeasurement>();

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Bearer", VmProjectBase.GetVmBaseStatic.Dc.DcExUser.Data.Tokens.FirstOrDefault().Token);
                    using (HttpResponseMessage HttpResponseMessage = await httpClient.GetAsync(requestString, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                    {
                        if (HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            using (HttpContent HttpContent = HttpResponseMessage.Content)
                            {
                                string response = await HttpContent.ReadAsStringAsync();
                                newMeasurementsResult = BissDeserialize.FromJson<List<ExMeasurement>>(response);

                                CachedMeasurements.Clear();

                                foreach (var exMeasurement in newMeasurementsResult)
                                {
                                    CachedMeasurements.Add(exMeasurement);
                                }

                                CurrentPage = 1;
                                UpdatePages();
                                // ReSharper disable once RedundantCast
                                Pages = (int)(CachedMeasurements.Count / 20);

                                NoValuesText = CachedMeasurements.Any() ? string.Empty : "Keine Werte vorhanden.";
                            }
                        }
                        else
                        {
                            NoValuesText = HttpResponseMessage.StatusCode.GetDisplayName();
                            // ReSharper disable once AccessToStaticMemberViaDerivedType
                            await VmProjectBase.MsgBox.Show(NoValuesText, "Fehler bei Abfrage der Messwerte", icon: VmMessageBoxImage.Error).ConfigureAwait(true);
                        }
                    }
                }
            }
            catch (Exception)
            {
                NoValuesText = "Fehler bei Abfrage";
                // ReSharper disable once AccessToStaticMemberViaDerivedType
                await VmProjectBase.MsgBox.Show(NoValuesText, "Fehler bei Abfrage der Messwerte", icon: VmMessageBoxImage.Error).ConfigureAwait(true);
            }
        }



        private void BindableObject_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DetailGridEnabled" && (!CachedMeasurements.Any() && ForceMeasurementsUpdate))
            {
                GetMeasurements().Wait();
                ForceMeasurementsUpdate = false;
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (MeasurementDefinition != null) // Wokraround Timing Measurementdefinition comes after it is requested
            {
                MeasurementDefinition.IsSelected = VmProjectBase.GetVmBaseStatic.Dc.DcExAbos.Any(x => x.Data.User.Id == VmProjectBase.GetVmBaseStatic.Dc.DcExUser.Data.Id && x.Data.MeasurementDefinitionAssignment.MeasurementDefinition.Id == MeasurementDefinition.Id);

                OnPropertyChanged(nameof(CanSeeMore));
                OnPropertyChanged(nameof(LastUpdate));
            }

        }


        private void CheckBox_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var mesDef = VmProjectBase.GetVmBaseStatic.Dc.DcExMeasurementDefinition.FirstOrDefault(x => x.Data.Id == MeasurementDefinition.Id);

            if (mesDef != null)
                VmProjectBase.GetVmBaseStatic.Dc.DcExMeasurementDefinition.StoreAll();

            MeasurementDefinition.IsSelected = e.Value;

            var curAbo = VmProjectBase.GetVmBaseStatic.Dc.DcExAbos.FirstOrDefault(x => x.Data.User.Id == VmProjectBase.GetVmBaseStatic.Dc.DcExUser.Data.Id && x.Data.MeasurementDefinitionAssignment.MeasurementDefinition.Id == MeasurementDefinition.Id);
            
            if (MeasurementDefinition.IsSelected && curAbo == null)
            {
                // ReSharper disable once UnusedVariable
                var currAbo = VmProjectBase.GetVmBaseStatic.Dc.DcExAbos.FirstOrDefault(x => x.Data.User.Id == VmProjectBase.GetVmBaseStatic.Dc.DcExUser.Data.Id && x.Data.MeasurementDefinitionAssignment.MeasurementDefinition.Id == MeasurementDefinition.Id);

                var currFiltered = VmProjectBase.GetVmBaseStatic.Dc.DcExIncomeOutput
                    .Where(x => x.Data.MeasurementDefinitionId == MeasurementDefinition.Id && x.Data.Option == EnumIncomeOutputOption.Abo).ToList();

                var curAssign = VmProjectBase.GetVmBaseStatic.Dc.DcExMeasurementDefinitionAssignments.FirstOrDefault(mA => mA.Data.MeasurementDefinition.Id == MeasurementDefinition.Id);

                if (!currFiltered.Any(c => DateTime.UtcNow - c.Data.TimeStamp < TimeSpan.FromDays(1)))
                {
                    VmProjectBase.GetVmBaseStatic.Dc.DcExIncomeOutput.Add(new DcListTypeIncomeOutput(
                        new ExIncomeOutput()
                        {
                            UserId = VmProjectBase.GetVmBaseStatic.Dc.DcExUser.Data.Id,
                            Ixies = -(int)(curAssign != null ? curAssign.Data.Costs : 0.0),   // eigener User Ausgabe
                            MeasurementDefinitionId = MeasurementDefinition.Id,
                            Option = EnumIncomeOutputOption.Abo
                        }));
                    VmProjectBase.GetVmBaseStatic.Dc.DcExIncomeOutput.StoreAll();
                }


                var newAbo = new ExAbo() { MeasurementDefinitionAssignment = curAssign!.Data, User = VmProjectBase.GetVmBaseStatic.Dc.DcExUser.Data };
                VmProjectBase.GetVmBaseStatic.Dc.DcExAbos.Add(new DcListTypeAbo(newAbo));
                VmProjectBase.GetVmBaseStatic.Dc.DcExAbos.StoreAll();
            }
            else if(!MeasurementDefinition.IsSelected && curAbo != null)
            {
                VmProjectBase.GetVmBaseStatic.Dc.DcExAbos.Remove(curAbo);
                VmProjectBase.GetVmBaseStatic.Dc.DcExAbos.StoreAll();
            }

            OnPropertyChanged(nameof(CanSeeMore));
        }
    }
}