// (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 18.10.2023 11:56
// Entwickler      Benjamin Moser (BMo)
// Projekt         IXchange

using System;
using System.Collections.ObjectModel;
using Biss.Apps.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BaseApp.View.Xamarin.Controls
{
    /// <summary>
    ///     Daten Modell eines Ratings.
    /// </summary>
    public class Rating
    {
        /// <summary>
        ///     Daten Modell eines Ratings.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="filled"></param>
        public Rating(int index, bool filled)
        {
            Index = index;
            Filled = filled;
        }

        #region Properties

        /// <summary>
        ///     Index des Symbols.
        /// </summary>
        public int Index { get; }

        /// <summary>
        ///     Ist ausgefüllt.
        /// </summary>
        public bool Filled { get; }

        #endregion
    }

    /// <summary>
    ///     Ix Change Rating Component.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IxChangeRating
    {
        /// <summary>
        ///     Anzahl der Sterne zum Anzeigen.
        /// </summary>
        public static readonly BindableProperty StarCountProperty = BindableProperty.Create(
            nameof(StarCount),
            typeof(int),
            typeof(IxChangeRating),
            5,
            propertyChanged: Value_PropertyChanged
        );

        /// <summary>
        ///     Anzahl der ausgefüllten Sterne.
        /// </summary>
        public static readonly BindableProperty StarValueProperty = BindableProperty.Create(
            nameof(StarValue),
            typeof(int),
            typeof(IxChangeRating),
            propertyChanged: Value_PropertyChanged,
            defaultBindingMode: BindingMode.TwoWay
        );

        /// <summary>
        ///     Lesezugriff
        /// </summary>
        public static readonly BindableProperty ReadOnlyProperty = BindableProperty.Create(
            nameof(ReadOnly),
            typeof(bool),
            typeof(IxChangeRating)
        );

        /// <summary>
        ///     Ix Change Rating Component.
        /// </summary>
        public IxChangeRating()
        {
            InitializeComponent();
            FillList();
        }

        #region Properties

        /// <summary>
        ///     Anzahl der ausgefüllten Sterne.
        /// </summary>
        public int StarValue
        {
            get => (int) GetValue(StarValueProperty);
            set => SetValue(StarValueProperty, value);
        }

        /// <summary>
        ///     Lesezugriff
        /// </summary>
        public bool ReadOnly
        {
            get => (bool) GetValue(ReadOnlyProperty);
            set => SetValue(ReadOnlyProperty, value);
        }

        /// <summary>
        ///     Anzahl der Sterne zum Anzeigen.
        /// </summary>
        public int StarCount
        {
            get => (int) GetValue(StarCountProperty);
            set => SetValue(StarCountProperty, value);
        }

        /// <summary>
        ///     Liste an Sternen.
        /// </summary>
        public ObservableCollection<Rating> StarsList { get; } = new ObservableCollection<Rating>();

        /// <summary>
        ///     Set Rating.
        /// </summary>
        public VmCommand CmdSetRating => new VmCommand(string.Empty, obj =>
        {
            if (!ReadOnly && int.TryParse(obj.ToString(), out var starcount))
            {
                StarValue = starcount;
            }
        });

        #endregion

        /// <summary>
        ///     Property Changed der Liste.
        /// </summary>
        /// <param name="bindable"></param>
        /// <param name="oldvalue"></param>
        /// <param name="newvalue"></param>
        private static void Value_PropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is IxChangeRating rating && newvalue is int)
            {
                rating.FillList();
            }
        }

        private void FillList()
        {
            StarsList.Clear();
            for (var i = 1; i <= StarCount; i++)
            {
                StarsList.Add(new Rating(i, i <= StarValue));
            }
        }
    }
}