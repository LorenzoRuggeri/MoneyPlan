using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savings.DAO.Migrations
{
    /// <inheritdoc />
    public partial class ChangedRelationBetweenRules_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_RulesID1",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlans_BudgetPlanRules_BudgetPlanRuleID",
                table: "BudgetPlans");

            migrationBuilder.DropIndex(
                name: "IX_BudgetPlans_BudgetPlanRuleID",
                table: "BudgetPlans");

            migrationBuilder.DropIndex(
                name: "IX_BudgetPlanBudgetPlanRule_RulesID1",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.DropColumn(
                name: "BudgetPlanRuleID",
                table: "BudgetPlans");

            migrationBuilder.DropColumn(
                name: "RulesID1",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_BudgetPlansId",
                table: "BudgetPlanBudgetPlanRule",
                column: "BudgetPlansId",
                principalTable: "BudgetPlanRules",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_BudgetPlansId",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.AddColumn<int>(
                name: "BudgetPlanRuleID",
                table: "BudgetPlans",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RulesID1",
                table: "BudgetPlanBudgetPlanRule",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPlans_BudgetPlanRuleID",
                table: "BudgetPlans",
                column: "BudgetPlanRuleID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPlanBudgetPlanRule_RulesID1",
                table: "BudgetPlanBudgetPlanRule",
                column: "RulesID1");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_RulesID1",
                table: "BudgetPlanBudgetPlanRule",
                column: "RulesID1",
                principalTable: "BudgetPlanRules",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlans_BudgetPlanRules_BudgetPlanRuleID",
                table: "BudgetPlans",
                column: "BudgetPlanRuleID",
                principalTable: "BudgetPlanRules",
                principalColumn: "ID");
        }
    }
}
