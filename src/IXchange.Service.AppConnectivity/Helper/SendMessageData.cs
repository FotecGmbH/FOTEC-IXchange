﻿// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.Generic;
using Database.Tables;

namespace IXchange.Service.AppConnectivity.Helper
{
    /// <summary>
    ///     <para>Daten fürs Nachrichten versenden.</para>
    /// Klasse SendMessageData. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class SendMessageData
    {
        #region Properties

        /// <summary>
        ///     An welche Geräte Nachricht gesendet werden soll.
        /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
        public List<TableDevice> Devices { get; set; } = new List<TableDevice>();
#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        ///     Wie Nachricht gesendet werden sollen
        /// </summary>
        public SendViaEnum SendVia { get; set; }

        /// <summary>
        ///     An welchen User Email gesendet werden soll. Wird nur verwendet wenn <see cref="SendVia" /> property auf Email
        ///     gesetzt ist./>
        /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
        public List<TableUser> Users { get; set; } = new List<TableUser>();
#pragma warning restore CA2227 // Collection properties should be read only

        #endregion
    }

    /// <summary>
    ///     Enum wie Nachricht versendet werden soll.
    /// </summary>
    public enum SendViaEnum
    {
        /// <summary>
        ///     Via Dc senden.
        /// </summary>
        Dc,

        /// <summary>
        ///     Via Push senden.
        /// </summary>
        Push,

        /// <summary>
        ///     Via Email senden.
        /// </summary>
        Email
    }
}