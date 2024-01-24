using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IXchangeDatabase.Migrations
{
    /// <inheritdoc />
    public partial class IncomeOutputMeasurementDefinitionNullablep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblIncomeOutputs_MeasurementDefinition_TblMeasurementDefinitonId",
                table: "TblIncomeOutputs");

            migrationBuilder.AlterColumn<long>(
                name: "TblMeasurementDefinitonId",
                table: "TblIncomeOutputs",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_TblIncomeOutputs_MeasurementDefinition_TblMeasurementDefinitonId",
                table: "TblIncomeOutputs",
                column: "TblMeasurementDefinitonId",
                principalTable: "MeasurementDefinition",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblIncomeOutputs_MeasurementDefinition_TblMeasurementDefinitonId",
                table: "TblIncomeOutputs");

            migrationBuilder.AlterColumn<long>(
                name: "TblMeasurementDefinitonId",
                table: "TblIncomeOutputs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TblIncomeOutputs_MeasurementDefinition_TblMeasurementDefinitonId",
                table: "TblIncomeOutputs",
                column: "TblMeasurementDefinitonId",
                principalTable: "MeasurementDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
