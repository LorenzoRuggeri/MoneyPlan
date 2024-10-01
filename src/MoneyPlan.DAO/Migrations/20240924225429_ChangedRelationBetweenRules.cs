using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savings.DAO.Migrations
{
    /// <inheritdoc />
    public partial class ChangedRelationBetweenRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_RulesID",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlans_BudgetPlansId",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BudgetPlanBudgetPlanRule",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.AddColumn<int>(
                name: "BudgetPlanRuleID",
                table: "BudgetPlans",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "BudgetPlanBudgetPlanRule",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "RulesID1",
                table: "BudgetPlanBudgetPlanRule",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BudgetPlanBudgetPlanRule",
                table: "BudgetPlanBudgetPlanRule",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPlans_BudgetPlanRuleID",
                table: "BudgetPlans",
                column: "BudgetPlanRuleID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPlanBudgetPlanRule_BudgetPlansId_RulesID",
                table: "BudgetPlanBudgetPlanRule",
                columns: new[] { "BudgetPlansId", "RulesID" },
                unique: true);

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
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlans_RulesID",
                table: "BudgetPlanBudgetPlanRule",
                column: "RulesID",
                principalTable: "BudgetPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlans_BudgetPlanRules_BudgetPlanRuleID",
                table: "BudgetPlans",
                column: "BudgetPlanRuleID",
                principalTable: "BudgetPlanRules",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_RulesID1",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlans_RulesID",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlans_BudgetPlanRules_BudgetPlanRuleID",
                table: "BudgetPlans");

            migrationBuilder.DropIndex(
                name: "IX_BudgetPlans_BudgetPlanRuleID",
                table: "BudgetPlans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BudgetPlanBudgetPlanRule",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.DropIndex(
                name: "IX_BudgetPlanBudgetPlanRule_BudgetPlansId_RulesID",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.DropIndex(
                name: "IX_BudgetPlanBudgetPlanRule_RulesID1",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.DropColumn(
                name: "BudgetPlanRuleID",
                table: "BudgetPlans");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.DropColumn(
                name: "RulesID1",
                table: "BudgetPlanBudgetPlanRule");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BudgetPlanBudgetPlanRule",
                table: "BudgetPlanBudgetPlanRule",
                columns: new[] { "BudgetPlansId", "RulesID" });

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_RulesID",
                table: "BudgetPlanBudgetPlanRule",
                column: "RulesID",
                principalTable: "BudgetPlanRules",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlanBudgetPlanRule_BudgetPlans_BudgetPlansId",
                table: "BudgetPlanBudgetPlanRule",
                column: "BudgetPlansId",
                principalTable: "BudgetPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
