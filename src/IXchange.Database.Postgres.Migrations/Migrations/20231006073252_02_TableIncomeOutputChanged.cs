using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class _02_TableIncomeOutputChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncomeOutput",
                table: "TblIncomeOutputs");

            migrationBuilder.AddColumn<int>(
                name: "CurrentTotalIxies",
                table: "TblIncomeOutputs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentTotalIxies",
                table: "TblIncomeOutputs");

            migrationBuilder.AddColumn<bool>(
                name: "IncomeOutput",
                table: "TblIncomeOutputs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
