using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IXchangeDatabase.Migrations
{
    /// <inheritdoc />
    public partial class SplittingDependencies_MeasurementDefinitionAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_MeasurementDefinitionToProjectAssignment_MeasurementDefinition_TblMeasurementDefinitionId",
            //    table: "MeasurementDefinitionToProjectAssignment");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_MeasurementResult_MeasurementDefinition_TblMeasurementDefinitionId",
            //    table: "MeasurementResult");

            migrationBuilder.DropForeignKey(
                name: "FK_TblAbos_MeasurementDefinition_TblMeasurementDefinitionId",
                table: "TblAbos");

            migrationBuilder.DropForeignKey(
                name: "FK_TblNotifications_MeasurementDefinition_TblMeasurementDefinitionId",
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

            //migrationBuilder.RenameColumn(
            //    name: "TblMeasurementDefinitionAssignmentId",
            //    table: "MeasurementResult",
            //    newName: "TblMeasurementDefinitionId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_MeasurementResult_TblMeasurementDefinitionAssignmentId",
            //    table: "MeasurementResult",
            //    newName: "IX_MeasurementResult_TblMeasurementDefinitionId");

            //migrationBuilder.RenameColumn(
            //    name: "TblMeasurementDefinitionAssignmentId",
            //    table: "MeasurementDefinitionToProjectAssignment",
            //    newName: "TblMeasurementDefinitionId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_MeasurementDefinitionToProjectAssignment_TblMeasurementDefinitionAssignmentId",
            //    table: "MeasurementDefinitionToProjectAssignment",
            //    newName: "IX_MeasurementDefinitionToProjectAssignment_TblMeasurementDefinitionId");

            migrationBuilder.DropIndex(
                name: "IX_TblAbos_TblMeasurementDefinitionId",
                table: "TblAbos");
                

            migrationBuilder.DropIndex(
                name: "IX_TblNotifications_TblMeasurementDefinitionId",
                table: "TblNotifications");

            migrationBuilder.DropIndex(
                name: "IX_TblRatings_TblMeasurementDefinitionId",
                table: "TblRatings");

            migrationBuilder.RenameColumn(
                name: "TblMeasurementDefinitionId",
                table: "TblAbos",
                newName: "TblMeasurementDefinitionAssignmentId");

            migrationBuilder.RenameColumn(
                name: "TblMeasurementDefinitionId",
                table: "TblNotifications",
                newName: "TblMeasurementDefinitionAssignmentId");

            migrationBuilder.RenameColumn(
                name: "TblMeasurementDefinitionId",
                table: "TblRatings",
                newName: "TblMeasurementDefinitionAssignmentId");

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
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TblMeasurementDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    SendNotifications = table.Column<bool>(type: "bit", nullable: false),
                    NotificationOnNewRating = table.Column<bool>(type: "bit", nullable: false),
                    NotificationOnSubscription = table.Column<bool>(type: "bit", nullable: false),
                    NotificationOnUnsubscription = table.Column<bool>(type: "bit", nullable: false),
                    AccessForResearchInstitutesGranted = table.Column<bool>(type: "bit", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementDefinitionAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasurementDefinitionAssignment_MeasurementDefinition_TblMeasurementDefinitionId",
                        column: x => x.TblMeasurementDefinitionId,
                        principalTable: "MeasurementDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementDefinitionAssignment_TblMeasurementDefinitionId",
                table: "MeasurementDefinitionAssignment",
                column: "TblMeasurementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TblAbos_TblMeasurementDefinitionAssignmentId",
                table: "TblAbos",
                column: "TblMeasurementDefinitionAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TblNotifications_TblMeasurementDefinitionAssignmentId",
                table: "TblNotifications",
                column: "TblMeasurementDefinitionAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TblRatings_TblMeasurementDefinitionAssignmentId",
                table: "TblRatings",
                column: "TblMeasurementDefinitionAssignmentId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_MeasurementDefinitionToProjectAssignment_MeasurementDefinition_TblMeasurementDefinitionId",
            //    table: "MeasurementDefinitionToProjectAssignment",
            //    column: "TblMeasurementDefinitionId",
            //    principalTable: "MeasurementDefinition",
            //    principalColumn: "Id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_MeasurementResult_MeasurementDefinition_TblMeasurementDefinitionId",
            //    table: "MeasurementResult",
            //    column: "TblMeasurementDefinitionId",
            //    principalTable: "MeasurementDefinition",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblAbos_MeasurementDefinitionAssignment_TblMeasurementDefinitionAssignmentId",
                table: "TblAbos",
                column: "TblMeasurementDefinitionAssignmentId",
                principalTable: "MeasurementDefinitionAssignment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblNotifications_MeasurementDefinitionAssignment_TblMeasurementDefinitionAssignmentId",
                table: "TblNotifications",
                column: "TblMeasurementDefinitionAssignmentId",
                principalTable: "MeasurementDefinitionAssignment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblRatings_MeasurementDefinitionAssignment_TblMeasurementDefinitionAssignmentId",
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
                name: "FK_MeasurementDefinitionToProjectAssignment_MeasurementDefinition_TblMeasurementDefinitionId",
                table: "MeasurementDefinitionToProjectAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementResult_MeasurementDefinition_TblMeasurementDefinitionId",
                table: "MeasurementResult");

            migrationBuilder.DropForeignKey(
                name: "FK_TblAbos_MeasurementDefinitionAssignment_TblMeasurementDefinitionAssignmentId",
                table: "TblAbos");

            migrationBuilder.DropForeignKey(
                name: "FK_TblNotifications_MeasurementDefinitionAssignment_TblMeasurementDefinitionAssignmentId",
                table: "TblNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_TblRatings_MeasurementDefinitionAssignment_TblMeasurementDefinitionAssignmentId",
                table: "TblRatings");

            migrationBuilder.DropTable(
                name: "MeasurementDefinitionAssignment");

            migrationBuilder.DropColumn(
                name: "TblLatestMeasurementResultId",
                table: "MeasurementDefinition");

            migrationBuilder.RenameColumn(
                name: "TblMeasurementDefinitionId",
                table: "MeasurementResult",
                newName: "TblMeasurementDefinitionAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_MeasurementResult_TblMeasurementDefinitionId",
                table: "MeasurementResult",
                newName: "IX_MeasurementResult_TblMeasurementDefinitionAssignmentId");

            migrationBuilder.RenameColumn(
                name: "TblMeasurementDefinitionId",
                table: "MeasurementDefinitionToProjectAssignment",
                newName: "TblMeasurementDefinitionAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_MeasurementDefinitionToProjectAssignment_TblMeasurementDefinitionId",
                table: "MeasurementDefinitionToProjectAssignment",
                newName: "IX_MeasurementDefinitionToProjectAssignment_TblMeasurementDefinitionAssignmentId");

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

            migrationBuilder.AddColumn<bool>(
                name: "AccessForResearchInstitutesGranted",
                table: "MeasurementDefinition",
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

            migrationBuilder.AddColumn<bool>(
                name: "NotificationOnNewRating",
                table: "MeasurementDefinition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationOnSubscription",
                table: "MeasurementDefinition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationOnUnsubscription",
                table: "MeasurementDefinition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SendNotifications",
                table: "MeasurementDefinition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "MeasurementDefinition",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementDefinitionToProjectAssignment_MeasurementDefinition_TblMeasurementDefinitionAssignmentId",
                table: "MeasurementDefinitionToProjectAssignment",
                column: "TblMeasurementDefinitionAssignmentId",
                principalTable: "MeasurementDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementResult_MeasurementDefinition_TblMeasurementDefinitionAssignmentId",
                table: "MeasurementResult",
                column: "TblMeasurementDefinitionAssignmentId",
                principalTable: "MeasurementDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblAbos_MeasurementDefinition_TblMeasurementDefinitionAssignmentId",
                table: "TblAbos",
                column: "TblMeasurementDefinitionAssignmentId",
                principalTable: "MeasurementDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblNotifications_MeasurementDefinition_TblMeasurementDefinitionAssignmentId",
                table: "TblNotifications",
                column: "TblMeasurementDefinitionAssignmentId",
                principalTable: "MeasurementDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblRatings_MeasurementDefinition_TblMeasurementDefinitionAssignmentId",
                table: "TblRatings",
                column: "TblMeasurementDefinitionAssignmentId",
                principalTable: "MeasurementDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
