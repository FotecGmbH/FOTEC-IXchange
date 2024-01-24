using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class _03_TableResearchInstitutesAccessAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AccessForResearchInstitutesGranted",
                table: "MeasurementDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationOnNewRating",
                table: "MeasurementDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationOnSubscription",
                table: "MeasurementDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationOnUnsubscription",
                table: "MeasurementDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SendNotifications",
                table: "MeasurementDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TableResearchInstitutesAccess",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResearchInstituteName = table.Column<string>(type: "text", nullable: false),
                    AdditionalData = table.Column<string>(type: "text", nullable: false),
                    AccessToken = table.Column<string>(type: "text", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableResearchInstitutesAccess", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TableResearchInstitutesAccess");

            migrationBuilder.DropColumn(
                name: "AccessForResearchInstitutesGranted",
                table: "MeasurementDefinition");

            migrationBuilder.DropColumn(
                name: "NotificationOnNewRating",
                table: "MeasurementDefinition");

            migrationBuilder.DropColumn(
                name: "NotificationOnSubscription",
                table: "MeasurementDefinition");

            migrationBuilder.DropColumn(
                name: "NotificationOnUnsubscription",
                table: "MeasurementDefinition");

            migrationBuilder.DropColumn(
                name: "SendNotifications",
                table: "MeasurementDefinition");
        }
    }
}
