using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IXchangeDatabase.Migrations
{
    /// <inheritdoc />
    public partial class IncomeOutputMeasurementDefAndOptionsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "TblIncomeOutputs");

            migrationBuilder.AddColumn<int>(
                name: "Option",
                table: "TblIncomeOutputs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "TblMeasurementDefinitonId",
                table: "TblIncomeOutputs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_TblIncomeOutputs_TblMeasurementDefinitonId",
                table: "TblIncomeOutputs",
                column: "TblMeasurementDefinitonId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblIncomeOutputs_MeasurementDefinition_TblMeasurementDefinitonId",
                table: "TblIncomeOutputs",
                column: "TblMeasurementDefinitonId",
                principalTable: "MeasurementDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblIncomeOutputs_MeasurementDefinition_TblMeasurementDefinitonId",
                table: "TblIncomeOutputs");

            migrationBuilder.DropIndex(
                name: "IX_TblIncomeOutputs_TblMeasurementDefinitonId",
                table: "TblIncomeOutputs");

            migrationBuilder.DropColumn(
                name: "Option",
                table: "TblIncomeOutputs");

            migrationBuilder.DropColumn(
                name: "TblMeasurementDefinitonId",
                table: "TblIncomeOutputs");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TblIncomeOutputs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
