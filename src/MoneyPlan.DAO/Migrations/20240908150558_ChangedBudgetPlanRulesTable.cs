using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savings.DAO.Migrations
{
    /// <inheritdoc />
    public partial class ChangedBudgetPlanRulesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Income",
                table: "BudgetPlanRules",
                type: "BOOL",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Income",
                table: "BudgetPlanRules",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "BOOL");
        }
    }
}
