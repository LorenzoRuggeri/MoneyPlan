using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savings.DAO.Migrations
{
    /// <inheritdoc />
    public partial class ChangedRelationBetweenRules_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_BudgetPlansId",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_RulesID",
                table: "BudgetPlanBudgetPlanRule",
                column: "RulesID",
                principalTable: "BudgetPlanRules",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_RulesID",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_BudgetPlansId",
                table: "BudgetPlanBudgetPlanRule",
                column: "BudgetPlansId",
                principalTable: "BudgetPlanRules",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
