using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyPlan.DAO.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBudgetPlanId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BudgetPlanRules_BudgetPlanId",
                table: "BudgetPlanRules",
                column: "BudgetPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BudgetPlanRules_BudgetPlanId",
                table: "BudgetPlanRules");
        }
    }
}
