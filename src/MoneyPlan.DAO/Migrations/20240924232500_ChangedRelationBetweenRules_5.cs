using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savings.DAO.Migrations
{
    /// <inheritdoc />
    public partial class ChangedRelationBetweenRules_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_BudgetPlansId",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlans_RulesID",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "BudgetPlanRules",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "RulesID",
                table: "BudgetPlanBudgetPlanRule",
                newName: "BudgetPlanRuleId");

            migrationBuilder.RenameColumn(
                name: "BudgetPlansId",
                table: "BudgetPlanBudgetPlanRule",
                newName: "BudgetPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetPlanBudgetPlanRule_RulesID",
                table: "BudgetPlanBudgetPlanRule",
                newName: "IX_BudgetPlanBudgetPlanRule_BudgetPlanRuleId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetPlanBudgetPlanRule_BudgetPlansId_RulesID",
                table: "BudgetPlanBudgetPlanRule",
                newName: "IX_BudgetPlanBudgetPlanRule_BudgetPlanId_BudgetPlanRuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_BudgetPlanId",
                table: "BudgetPlanBudgetPlanRule",
                column: "BudgetPlanId",
                principalTable: "BudgetPlanRules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlans_BudgetPlanRuleId",
                table: "BudgetPlanBudgetPlanRule",
                column: "BudgetPlanRuleId",
                principalTable: "BudgetPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_BudgetPlanId",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlans_BudgetPlanRuleId",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BudgetPlanRules",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "BudgetPlanRuleId",
                table: "BudgetPlanBudgetPlanRule",
                newName: "RulesID");

            migrationBuilder.RenameColumn(
                name: "BudgetPlanId",
                table: "BudgetPlanBudgetPlanRule",
                newName: "BudgetPlansId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetPlanBudgetPlanRule_BudgetPlanRuleId",
                table: "BudgetPlanBudgetPlanRule",
                newName: "IX_BudgetPlanBudgetPlanRule_RulesID");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetPlanBudgetPlanRule_BudgetPlanId_BudgetPlanRuleId",
                table: "BudgetPlanBudgetPlanRule",
                newName: "IX_BudgetPlanBudgetPlanRule_BudgetPlansId_RulesID");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_BudgetPlansId",
                table: "BudgetPlanBudgetPlanRule",
                column: "BudgetPlansId",
                principalTable: "BudgetPlanRules",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlans_RulesID",
                table: "BudgetPlanBudgetPlanRule",
                column: "RulesID",
                principalTable: "BudgetPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
