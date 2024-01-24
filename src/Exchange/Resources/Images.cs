// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.IO;
using System.Reflection;

// ReSharper disable InconsistentNaming
namespace Exchange.Resources
{
    /// <summary>
    ///     Bilder in den Resourcen
    /// </summary>
#pragma warning disable CS1591
    public enum EnumEmbeddedImage
    {
        Logo_png,
        DefaultUserImage_png,
        Fotec_png,
        Pin_png
    }
#pragma warning restore CS1591

    /// <summary>
    ///     <para>Bilder laden (Projektweit)</para>
    /// Klasse Images. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class Images
    {
        /// <summary>
        ///     Bild als lokalen Stream Laden
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public static Stream ReadImageAsStream(EnumEmbeddedImage imageName)
        {
#pragma warning disable CA1307 // Specify StringComparison for clarity
            var image = $"Exchange.Resources.Images.{imageName.ToString().Replace("_", ".")}";
#pragma warning restore CA1307 // Specify StringComparison for clarity
            var assembly = Assembly.Load(new AssemblyName("Exchange"));
            var imageStream = assembly.GetManifestResourceStream(image);
            return imageStream;
        }
    }
}