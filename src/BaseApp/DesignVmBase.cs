// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

namespace BaseApp
{
    /// <summary>
    ///     <para>Hilfsklasse für XAML Binding zur Entwurfszeit</para>
    /// Klasse DesignVmBase. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class DesignVmBase : VmProjectBase
    {
        /// <summary>
        ///     Basis View Model für alle ViewModel
        /// </summary>
        /// <param name="pageTitle"></param>
        /// <param name="args"></param>
        /// <param name="subTitle"></param>
        public DesignVmBase(string pageTitle, object? args = null, string subTitle = "") : base(pageTitle, args, subTitle)
        {
            
        }

        #region Properties

        /// <summary>
        ///     DesignInstance
        /// </summary>
        public static DesignVmBase DesignInstance => new DesignVmBase("");

        #endregion
    }
}