// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 23.1.2024 10:37
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Apps.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Shared
{
    /// <summary>
    ///     Bx Device Infos.
    /// </summary>
    public partial class BxDeviceInfos
    {
        #region Properties

        /// <summary>
        ///     Ex Common Info.
        /// </summary>
        [Parameter]
        public ExCommonInfo ExCommonInfo { get; set; } = null !;

        /// <summary>
        ///     CmdDevHelper.
        /// </summary>
        [Parameter]
        public VmCommand CmdDevHelper { get; set; } = null !;

        #endregion
    }
}