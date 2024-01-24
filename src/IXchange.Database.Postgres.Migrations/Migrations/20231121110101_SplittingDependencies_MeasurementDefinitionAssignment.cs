using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class SplittingDependencies_MeasurementDefinitionAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblAbos_MeasurementDefinition_TblMeasurementDefinitionId",
                table: "TblAbos");

            migrationBuilder.DropForeignKey(
                name: "FK_TblNotifications_MeasurementDefinition_TblMeasurementDefini~",
                table: "TblNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_TblRatings_MeasurementDefinition_TblMeasurementDefinitionId",
                table: "TblRatings");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "TblIncomeOutputs");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "MeasurementResult");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "MeasurementResult");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "MeasurementDefinitionToProjectAssignment");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "MeasurementDefinitionToProjectAssignment");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "MeasurementDefinitionTemplate");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "MeasurementDefinitionTemplate");

            migrationBuilder.DropColumn(
                name: "AccessForResearchInstitutesGranted",
                table: "MeasurementDefinition");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "MeasurementDefinition");

            migrationBuilder.DropColumn(
                name: "IsArchived",
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

            migrationBuilder.DropColumn(
                name: "Type",
                table: "MeasurementDefinition");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "IotDevice");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "IotDevice");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "Gateway");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Gateway");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "File");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "File");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "Dataconverter");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Dataconverter");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "CompanyGlobalConfig");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "CompanyGlobalConfig");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "AccessToken");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "AccessToken");

            migrationBuilder.RenameColumn(
                name: "TblMeasurementDefinitionId",
                table: "TblRatings",
                newName: "TblMeasurementDefinitionAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_TblRatings_TblMeasurementDefinitionId",
                table: "TblRatings",
                newName: "IX_TblRatings_TblMeasurementDefinitionAssignmentId");

            migrationBuilder.RenameColumn(
                name: "TblMeasurementDefinitionId",
                table: "TblNotifications",
                newName: "TblMeasurementDefinitionAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_TblNotifications_TblMeasurementDefinitionId",
                table: "TblNotifications",
                newName: "IX_TblNotifications_TblMeasurementDefinitionAssignmentId");

            migrationBuilder.RenameColumn(
                name: "TblMeasurementDefinitionId",
                table: "TblAbos",
                newName: "TblMeasurementDefinitionAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_TblAbos_TblMeasurementDefinitionId",
                table: "TblAbos",
                newName: "IX_TblAbos_TblMeasurementDefinitionAssignmentId");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStamp",
                table: "TblNotifications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Option",
                table: "TblIncomeOutputs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "TblMeasurementDefinitonId",
                table: "TblIncomeOutputs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TblLatestMeasurementResultId",
                table: "MeasurementDefinition",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MeasurementDefinitionAssignment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TblMeasurementDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    SendNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    NotificationOnNewRating = table.Column<bool>(type: "boolean", nullable: false),
                    NotificationOnSubscription = table.Column<bool>(type: "boolean", nullable: false),
                    NotificationOnUnsubscription = table.Column<bool>(type: "boolean", nullable: false),
                    AccessForResearchInstitutesGranted = table.Column<bool>(type: "boolean", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementDefinitionAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasurementDefinitionAssignment_MeasurementDefinition_TblMe~",
                        column: x => x.TblMeasurementDefinitionId,
                        principalTable: "MeasurementDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblIncomeOutputs_TblMeasurementDefinitonId",
                table: "TblIncomeOutputs",
                column: "TblMeasurementDefinitonId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementDefinitionAssignment_TblMeasurementDefinitionId",
                table: "MeasurementDefinitionAssignment",
                column: "TblMeasurementDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblAbos_MeasurementDefinitionAssignment_TblMeasurementDefin~",
                table: "TblAbos",
                column: "TblMeasurementDefinitionAssignmentId",
                principalTable: "MeasurementDefinitionAssignment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblIncomeOutputs_MeasurementDefinition_TblMeasurementDefini~",
                table: "TblIncomeOutputs",
                column: "TblMeasurementDefinitonId",
                principalTable: "MeasurementDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TblNotifications_MeasurementDefinitionAssignment_TblMeasure~",
                table: "TblNotifications",
                column: "TblMeasurementDefinitionAssignmentId",
                principalTable: "MeasurementDefinitionAssignment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblRatings_MeasurementDefinitionAssignment_TblMeasurementDe~",
                table: "TblRatings",
                column: "TblMeasurementDefinitionAssignmentId",
                principalTable: "MeasurementDefinitionAssignment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblAbos_MeasurementDefinitionAssignment_TblMeasurementDefin~",
                table: "TblAbos");

            migrationBuilder.DropForeignKey(
                name: "FK_TblIncomeOutputs_MeasurementDefinition_TblMeasurementDefini~",
                table: "TblIncomeOutputs");

            migrationBuilder.DropForeignKey(
                name: "FK_TblNotifications_MeasurementDefinitionAssignment_TblMeasure~",
                table: "TblNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_TblRatings_MeasurementDefinitionAssignment_TblMeasurementDe~",
                table: "TblRatings");

            migrationBuilder.DropTable(
                name: "MeasurementDefinitionAssignment");

            migrationBuilder.DropIndex(
                name: "IX_TblIncomeOutputs_TblMeasurementDefinitonId",
                table: "TblIncomeOutputs");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "TblNotifications");

            migrationBuilder.DropColumn(
                name: "Option",
                table: "TblIncomeOutputs");

            migrationBuilder.DropColumn(
                name: "TblMeasurementDefinitonId",
                table: "TblIncomeOutputs");

            migrationBuilder.DropColumn(
                name: "TblLatestMeasurementResultId",
                table: "MeasurementDefinition");

            migrationBuilder.RenameColumn(
                name: "TblMeasurementDefinitionAssignmentId",
                table: "TblRatings",
                newName: "TblMeasurementDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_TblRatings_TblMeasurementDefinitionAssignmentId",
                table: "TblRatings",
                newName: "IX_TblRatings_TblMeasurementDefinitionId");

            migrationBuilder.RenameColumn(
                name: "TblMeasurementDefinitionAssignmentId",
                table: "TblNotifications",
                newName: "TblMeasurementDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_TblNotifications_TblMeasurementDefinitionAssignmentId",
                table: "TblNotifications",
                newName: "IX_TblNotifications_TblMeasurementDefinitionId");

            migrationBuilder.RenameColumn(
                name: "TblMeasurementDefinitionAssignmentId",
                table: "TblAbos",
                newName: "TblMeasurementDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_TblAbos_TblMeasurementDefinitionAssignmentId",
                table: "TblAbos",
                newName: "IX_TblAbos_TblMeasurementDefinitionId");

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "User",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "User",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TblIncomeOutputs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "Setting",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Setting",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "Project",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Project",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "Permission",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Permission",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "MeasurementResult",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "MeasurementResult",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "MeasurementDefinitionToProjectAssignment",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "MeasurementDefinitionToProjectAssignment",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "MeasurementDefinitionTemplate",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "MeasurementDefinitionTemplate",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AccessForResearchInstitutesGranted",
                table: "MeasurementDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "MeasurementDefinition",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "MeasurementDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "MeasurementDefinition",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "IotDevice",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "IotDevice",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "Gateway",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Gateway",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "File",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "File",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "Device",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Device",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "Dataconverter",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Dataconverter",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "CompanyGlobalConfig",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "CompanyGlobalConfig",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "Company",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Company",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "AccessToken",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "AccessToken",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_TblAbos_MeasurementDefinition_TblMeasurementDefinitionId",
                table: "TblAbos",
                column: "TblMeasurementDefinitionId",
                principalTable: "MeasurementDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblNotifications_MeasurementDefinition_TblMeasurementDefini~",
                table: "TblNotifications",
                column: "TblMeasurementDefinitionId",
                principalTable: "MeasurementDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblRatings_MeasurementDefinition_TblMeasurementDefinitionId",
                table: "TblRatings",
                column: "TblMeasurementDefinitionId",
                principalTable: "MeasurementDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
