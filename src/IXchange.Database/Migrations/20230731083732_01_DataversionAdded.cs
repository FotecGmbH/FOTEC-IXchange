using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IXchangeDatabase.Migrations
{
    /// <inheritdoc />
    public partial class _01_DataversionAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "User",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "TblRatings",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "TblRatings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "TblNotifications",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "TblNotifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "TblIncomeOutputs",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "TblIncomeOutputs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "TblAbos",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "TblAbos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "Setting",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Setting",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "Project",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Project",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "Permission",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Permission",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "MeasurementResult",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "MeasurementResult",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "MeasurementDefinitionToProjectAssignment",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "MeasurementDefinitionToProjectAssignment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "MeasurementDefinitionTemplate",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "MeasurementDefinitionTemplate",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "MeasurementDefinition",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "MeasurementDefinition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "IotDevice",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "IotDevice",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "Gateway",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Gateway",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "File",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "File",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "Device",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Device",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "Dataconverter",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Dataconverter",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "CompanyGlobalConfig",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "CompanyGlobalConfig",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "Company",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Company",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "DataVersion",
                table: "AccessToken",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "AccessToken",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "TblRatings");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "TblRatings");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "TblNotifications");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "TblNotifications");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "TblIncomeOutputs");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "TblIncomeOutputs");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "TblAbos");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "TblAbos");

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
                name: "DataVersion",
                table: "MeasurementDefinition");

            migrationBuilder.DropColumn(
                name: "IsArchived",
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
        }
    }
}
