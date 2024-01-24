// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Dc.Client;
using Biss.Dc.Core;
using Exchange.Model.ConfigApp;


namespace BaseApp.Connectivity
{
    /// <summary>
    /// Hilfsklasse für X:Datatype in xaml
    /// </summary>
    public class DcListTypeGateway : DcListDataPoint<ExGateway>
    {
        /// <summary>
        /// Listentyp für Orgusers konstruktor
        /// </summary>
        /// <param name="root">root</param>
        /// <param name="data">daten</param>
        /// <param name="pointId">id des punktes</param>
        /// <param name="source">source</param>
        /// <param name="index">index</param>
        /// <param name="state">status</param>
        /// <param name="sortIdex">sortier index</param>
        /// <param name="dataVersion">data version</param>
        public DcListTypeGateway(IDcDataRoot root, ExGateway data, string pointId, EnumDcDataSource source, long index, EnumDcListElementState state, long sortIdex, byte[] dataVersion) :
            base(root, data, pointId, source, index, state, sortIdex, dataVersion)
        {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="data"></param>
        public DcListTypeGateway(ExGateway data) : base(data)
        {
        }
    }


    /// <summary>
    /// Hilfsklasse für X:Datatype in xaml
    /// </summary>
    public class DcListTypeIotDevice : DcListDataPoint<ExIotDevice>
    {
        /// <summary>
        /// Listentyp für Orgusers konstruktor
        /// </summary>
        /// <param name="root">root</param>
        /// <param name="data">daten</param>
        /// <param name="pointId">id des punktes</param>
        /// <param name="source">source</param>
        /// <param name="index">index</param>
        /// <param name="state">status</param>
        /// <param name="sortIdex">sortier index</param>
        /// <param name="dataVersion">data version</param>
        public DcListTypeIotDevice(IDcDataRoot root, ExIotDevice data, string pointId, EnumDcDataSource source, long index, EnumDcListElementState state, long sortIdex, byte[] dataVersion) :
            base(root, data, pointId, source, index, state, sortIdex, dataVersion)
        {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="data"></param>
        public DcListTypeIotDevice(ExIotDevice data) : base(data)
        {
        }
    }

    /// <summary>
    /// Hilfsklasse für X:Datatype in xaml
    /// </summary>
    public class DcListTypeMeasurementDefinition : DcListDataPoint<ExMeasurementDefinition>
    {
        /// <summary>
        /// Listentyp für Orgusers konstruktor
        /// </summary>
        /// <param name="root">root</param>
        /// <param name="data">daten</param>
        /// <param name="pointId">id des punktes</param>
        /// <param name="source">source</param>
        /// <param name="index">index</param>
        /// <param name="state">status</param>
        /// <param name="sortIdex">sortier index</param>
        /// <param name="dataVersion">data version</param>
        public DcListTypeMeasurementDefinition(IDcDataRoot root, ExMeasurementDefinition data, string pointId, EnumDcDataSource source, long index, EnumDcListElementState state, long sortIdex, byte[] dataVersion) :
            base(root, data, pointId, source, index, state, sortIdex, dataVersion)
        {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="data">Daten des DC Listentyps</param>
        public DcListTypeMeasurementDefinition(ExMeasurementDefinition data) : base(data)
        {
        }
    }

    /// <summary>
    /// Hilfsklasse für X:Datatype in xaml
    /// </summary>
    public class DcListTypeCompany : DcListDataPoint<ExCompany>
    {
        /// <summary>
        /// Listentyp für Orgusers konstruktor
        /// </summary>
        /// <param name="root">root</param>
        /// <param name="data">daten</param>
        /// <param name="pointId">id des punktes</param>
        /// <param name="source">source</param>
        /// <param name="index">index</param>
        /// <param name="state">status</param>
        /// <param name="sortIdex">sortier index</param>
        /// <param name="dataVersion">data version</param>
        public DcListTypeCompany(IDcDataRoot root, ExCompany data, string pointId, EnumDcDataSource source, long index, EnumDcListElementState state, long sortIdex, byte[] dataVersion) :
            base(root, data, pointId, source, index, state, sortIdex, dataVersion)
        {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="data">Daten des DC Listentyps</param>
        public DcListTypeCompany(ExCompany data) : base(data)
        {
        }
    }

    /// <summary>
    ///     List Type Project.
    /// </summary>
    public class DcListTypeProject : DcListDataPoint<ExProject>
    {
        /// <summary>
        /// Listentyp für Orgusers konstruktor
        /// </summary>
        /// <param name="root">root</param>
        /// <param name="data">daten</param>
        /// <param name="pointId">id des punktes</param>
        /// <param name="source">source</param>
        /// <param name="index">index</param>
        /// <param name="state">status</param>
        /// <param name="sortIdex">sortier index</param>
        /// <param name="dataVersion">data version</param>
        public DcListTypeProject(IDcDataRoot root, ExProject data, string pointId, EnumDcDataSource source, long index, EnumDcListElementState state, long sortIdex, byte[] dataVersion) :
            base(root, data, pointId, source, index, state, sortIdex, dataVersion)
        {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="data">Daten des DC Listentyps</param>
        public DcListTypeProject(ExProject data) : base(data)
        {
        }
    }

    /// <summary>
    /// Hilfsklasse für X:Datatype in xaml
    /// </summary>
    public class DcListTypeRating : DcListDataPoint<ExRating>
    {
        /// <summary>
        /// Listentyp für Orgusers konstruktor
        /// </summary>
        /// <param name="root">root</param>
        /// <param name="data">daten</param>
        /// <param name="pointId">id des punktes</param>
        /// <param name="source">source</param>
        /// <param name="index">index</param>
        /// <param name="state">status</param>
        /// <param name="sortIdex">sortier index</param>
        /// <param name="dataVersion">data version</param>
        public DcListTypeRating(IDcDataRoot root, ExRating data, string pointId, EnumDcDataSource source, long index, EnumDcListElementState state, long sortIdex, byte[] dataVersion) :
            base(root, data, pointId, source, index, state, sortIdex, dataVersion)
        {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="data">Daten des DC Listentyps</param>
        public DcListTypeRating(ExRating data) : base(data)
        {
        }
    }

    /// <summary>
    /// Hilfsklasse für X:Datatype in xaml
    /// </summary>
    public class DcListTypeIncomeOutput : DcListDataPoint<ExIncomeOutput>
    {
        /// <summary>
        /// Listentyp für Orgusers konstruktor
        /// </summary>
        /// <param name="root">root</param>
        /// <param name="data">daten</param>
        /// <param name="pointId">id des punktes</param>
        /// <param name="source">source</param>
        /// <param name="index">index</param>
        /// <param name="state">status</param>
        /// <param name="sortIdex">sortier index</param>
        /// <param name="dataVersion">data version</param>
        public DcListTypeIncomeOutput(IDcDataRoot root, ExIncomeOutput data, string pointId, EnumDcDataSource source, long index, EnumDcListElementState state, long sortIdex, byte[] dataVersion) :
            base(root, data, pointId, source, index, state, sortIdex, dataVersion)
        {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="data">Daten des DC Listentyps</param>
        public DcListTypeIncomeOutput(ExIncomeOutput data) : base(data)
        {
        }
    }

    /// <summary>
    /// Hilfsklasse für X:Datatype in xaml
    /// </summary>
    public class DcListTypeAbo : DcListDataPoint<ExAbo>
    {
        /// <summary>
        /// Listentyp für Orgusers konstruktor
        /// </summary>
        /// <param name="root">root</param>
        /// <param name="data">daten</param>
        /// <param name="pointId">id des punktes</param>
        /// <param name="source">source</param>
        /// <param name="index">index</param>
        /// <param name="state">status</param>
        /// <param name="sortIdex">sortier index</param>
        /// <param name="dataVersion">data version</param>
        public DcListTypeAbo(IDcDataRoot root, ExAbo data, string pointId, EnumDcDataSource source, long index, EnumDcListElementState state, long sortIdex, byte[] dataVersion) :
            base(root, data, pointId, source, index, state, sortIdex, dataVersion)
        {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="data">Daten des DC Listentyps</param>
        public DcListTypeAbo(ExAbo data) : base(data)
        {
        }
    }

    /// <summary>
    /// Hilfsklasse für X:Datatype in xaml
    /// </summary>
    public class DcListTypeNotification : DcListDataPoint<ExNotification>
    {
        /// <summary>
        /// Listentyp für Orgusers konstruktor
        /// </summary>
        /// <param name="root">root</param>
        /// <param name="data">daten</param>
        /// <param name="pointId">id des punktes</param>
        /// <param name="source">source</param>
        /// <param name="index">index</param>
        /// <param name="state">status</param>
        /// <param name="sortIdex">sortier index</param>
        /// <param name="dataVersion">data version</param>
        public DcListTypeNotification(IDcDataRoot root, ExNotification data, string pointId, EnumDcDataSource source, long index, EnumDcListElementState state, long sortIdex, byte[] dataVersion) :
            base(root, data, pointId, source, index, state, sortIdex, dataVersion)
        {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="data">Daten des DC Listentyps</param>
        public DcListTypeNotification(ExNotification data) : base(data)
        {
        }
    }

    /// <summary>
    /// Hilfsklasse für X:Datatype in xaml
    /// </summary>
    public class DcListTypeMeasurementDefinitionAssignment : DcListDataPoint<ExMeasurementDefinitionAssignment>
    {
        /// <summary>
        /// Listentyp für Orgusers konstruktor
        /// </summary>
        /// <param name="root">root</param>
        /// <param name="data">daten</param>
        /// <param name="pointId">id des punktes</param>
        /// <param name="source">source</param>
        /// <param name="index">index</param>
        /// <param name="state">status</param>
        /// <param name="sortIdex">sortier index</param>
        /// <param name="dataVersion">data version</param>
        public DcListTypeMeasurementDefinitionAssignment(IDcDataRoot root, ExMeasurementDefinitionAssignment data, string pointId, EnumDcDataSource source, long index, EnumDcListElementState state, long sortIdex, byte[] dataVersion) :
            base(root, data, pointId, source, index, state, sortIdex, dataVersion)
        {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="data">Daten des DC Listentyps</param>
        public DcListTypeMeasurementDefinitionAssignment(ExMeasurementDefinitionAssignment data) : base(data)
        {
        }
    }
}