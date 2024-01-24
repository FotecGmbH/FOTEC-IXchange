// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Linq;
using System.Threading.Tasks;
using Biss.Dc.Core;
using Exchange.Model.ConfigApp;
using IXchangeDatabase;
using IXchangeDatabase.Converter;
using IXchangeDatabase.Tables;
using Microsoft.EntityFrameworkCore;


// ReSharper disable once CheckNamespace
namespace ConnectivityHost.CacheConverter
{
    /// <summary>
    /// <para>CacheConverter Abos</para>
    /// Klasse DbCacheConverterAbos. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class DbCacheConverterAbos : DbCacheConverterBase<Db, TableAbo, ExAbo>
    {
        /// <summary>
        /// DbCacheConverterAbos
        /// </summary>
        public DbCacheConverterAbos() : base(nameof(Db.TblAbos), EnumDbCacheConverterModes.Off)
        {
        }

        /// <summary>
        /// Convert to model
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="dbData">tableabo</param>
        /// <returns>Abo</returns>
        protected override Task<DcServerListItem<ExAbo>> ConvertToModel(Db db, TableAbo dbData)
        {
            return Task.FromResult(new DcServerListItem<ExAbo>
                                   {
                                       Data = dbData!.ToExAbo(),
                                       DataVersion = dbData.DataVersion,
                                       Index = dbData.Id,
                                       SortIndex = dbData.Id
                                   });
        }

        /// <summary>
        /// Store element
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="deviceId">device id</param>
        /// <param name="userId">user id</param>
        /// <param name="newOrModifiedData">data</param>
        /// <param name="dbData">db data</param>
        /// <param name="dbId">db id</param>
        /// <param name="operation">operation</param>
        /// <param name="deleteFileAction">action</param>
        /// <returns>abo</returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected override async Task<TableAbo> StoreElement(Db db, long deviceId, long userId, ExAbo newOrModifiedData, TableAbo dbData, long dbId, EnumDbCacheElementOperations operation, Func<Db, long, Task> deleteFileAction)
        {
            if (db == null!)
            {
                throw new ArgumentNullException($"[{GetType().Name}]({nameof(StoreElement)}): {nameof(db)}");
            }

            if (newOrModifiedData == null!)
            {
                throw new ArgumentNullException($"[{GetType().Name}]({nameof(StoreElement)}): {nameof(newOrModifiedData)}");
            }

            if (dbData == null!)
            {
                throw new ArgumentNullException($"[{GetType().Name}]({nameof(StoreElement)}): {nameof(dbData)}");
            }

            if (deleteFileAction == null!)
            {
                throw new ArgumentNullException($"[{GetType().Name}]({nameof(StoreElement)}): {nameof(deleteFileAction)} - wird benötigt!");
            }

            var p = dbData;

            if (operation == EnumDbCacheElementOperations.Modified || operation == EnumDbCacheElementOperations.Deleted)
            {
                p = await db.TblAbos.Where(f => f.Id == dbId)
                    .FirstAsync().ConfigureAwait(false);
            }

            if (operation == EnumDbCacheElementOperations.Deleted)
            {
                return p;
            }

            p.ExceedNotify = newOrModifiedData.ExceedNotify;
            p.ExceedNotifyValue = newOrModifiedData.ExceedNotifyValue;
            p.UndercutNotify = newOrModifiedData.UndercutNotify;
            p.UndercutNotifyValue = newOrModifiedData.UndercutNotifyValue;
            p.FailureForMinutesNotify = newOrModifiedData.FailureForMinutesNotify;
            p.FailureForMinutesNotifyValue = newOrModifiedData.FailureForMinutesNotifyValue;
            p.MovingAverageNotify = newOrModifiedData.MovingAverageNotify;
            p.MovingAverageNotifyValue = newOrModifiedData.MovingAverageNotifyValue;
            p.TblUserId = newOrModifiedData.User.Id;
            p.TblMeasurementDefinitionAssignmentId = newOrModifiedData.MeasurementDefinitionAssignment.Id;

            return p;
        }
    }
}