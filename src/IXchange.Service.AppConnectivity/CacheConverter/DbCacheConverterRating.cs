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
    /// <para>CacheConverterRating</para>
    /// Klasse DbCacheConverterRating. (C) 2023 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class DbCacheConverterRating : DbCacheConverterBase<Db, TableRating, ExRating>
    {
        /// <summary>
        /// DbCacheConverterRating
        /// </summary>
        public DbCacheConverterRating() : base(nameof(Db.TblRatings), EnumDbCacheConverterModes.Off)
        {
        }

        /// <summary>
        ///     Einzelnen Db Datensatz konvertieren
        /// </summary>
        /// <param name="db">Aktueller Db Kontext falls benötigt</param>
        /// <param name="dbData">Einzelner Db-Datensatz</param>
        /// <returns></returns>
        protected override Task<DcServerListItem<ExRating>> ConvertToModel(Db db, TableRating dbData)
        {
            return Task.FromResult(new DcServerListItem<ExRating>
                                   {
                                       Data = dbData.ToExRating(),
                                       DataVersion = dbData.DataVersion,
                                       Index = dbData.Id,
                                       SortIndex = dbData.Id
                                   });
        }

        /// <summary>
        /// Ein einzelnen Element in der Datenbank sichern. Add/Remove/SaveChanges usw. passiert automatisch und muss in dieser
        /// Methode nicht aufgerufen werden.
        /// </summary>
        /// <param name="db">Aktueller Datenbankkontext für das Bearbeiten</param>
        /// <param name="deviceId">Geräte Id des Bearbeiters</param>
        /// <param name="userId">User Id - welcher bearbeitet</param>
        /// <param name="newOrModifiedData">Neue bzw. modifizierte Daten</param>
        /// <param name="dbData">
        /// Db Datensatz - one "Includes" - wenn "mehr" gebraucht wird dann mit der dbId und db Kontext
        /// arbeiten. Immer den richtigen Db-Datensatz dann zurückliefern für das Add/Remove/SaveChanges usw.
        /// </param>
        /// <param name="dbId">Id des Datensatzes (bei "New" immer 0)</param>
        /// <param name="operation">New/Edit/Delete</param>
        /// <param name="deleteFileAction">
        /// Falls "Dateien aus DB und aus einer Storage gelöscht werden müssen dann hier die Methode
        /// angeben
        /// </param>
        /// <returns>Db-Datensatz</returns>
        protected override async Task<TableRating> StoreElement(Db db, long deviceId, long userId, ExRating newOrModifiedData, TableRating dbData, long dbId, EnumDbCacheElementOperations operation, Func<Db, long, Task> deleteFileAction)
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
                p = await db.TblRatings.Where(f => f.Id == dbId)
                    .FirstAsync().ConfigureAwait(false);
            }

            if (operation == EnumDbCacheElementOperations.Deleted)
            {
                return p;
            }

            p.Description = newOrModifiedData.Description;
            // ReSharper disable once RedundantSuppressNullableWarningExpression
            p.TblUserId = newOrModifiedData.User!.Id;
            p.Rating = newOrModifiedData.Rating;
            p.TblMeasurementDefinitionAssignmentId = newOrModifiedData.MeasurementDefinitionAssignment.Id;
            p.TimeStamp = newOrModifiedData.TimeStamp;

            return p;
        }
    }
}