using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savings.DAO.Migrations
{
    /// <inheritdoc />
    public partial class ChangedRelationBetweenRules_7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlans_BudgetPlanRuleId",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlans_BudgetPlanId",
                table: "BudgetPlanBudgetPlanRule",
                column: "BudgetPlanId",
                principalTable: "BudgetPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlans_BudgetPlanId",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlans_BudgetPlanRuleId",
                table: "BudgetPlanBudgetPlanRule",
                column: "BudgetPlanRuleId",
                principalTable: "BudgetPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
