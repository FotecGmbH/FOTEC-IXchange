// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using Google.Protobuf.WellKnownTypes;

namespace IXchange.Service.Com.GRPC.Extensions
{
    /// <summary>
    /// <para>This class contains extension methods for the DateTime</para>
    /// Klasse DateTimeExtensions. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class DateTimeExtensions
    {
        public static Timestamp ConvertToUtcGoogleTimestamp(this DateTime date)
        {
            var utcDate = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            return Timestamp.FromDateTime(utcDate);
        }
    }
}