using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savings.DAO.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetPlanBudgetRulesRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanRules_BudgetPlans_BudgetPlanId",
                table: "BudgetPlanRules");

            migrationBuilder.DropIndex(
                name: "IX_BudgetPlanRules_BudgetPlanId",
                table: "BudgetPlanRules");

            migrationBuilder.DropColumn(
                name: "BudgetPlan",
                table: "BudgetPlanRules");

            migrationBuilder.RenameColumn(
                name: "BudgetPlanId",
                table: "BudgetPlanRules",
                newName: "Type");

            migrationBuilder.CreateTable(
                name: "BudgetPlanBudgetPlanRule",
                columns: table => new
                {
                    BudgetPlansId = table.Column<int>(type: "INTEGER", nullable: false),
                    RulesID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetPlanBudgetPlanRule", x => new { x.BudgetPlansId, x.RulesID });
                    table.ForeignKey(
                        name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_RulesID",
                        column: x => x.RulesID,
                        principalTable: "BudgetPlanRules",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetPlanBudgetPlanRule_BudgetPlans_BudgetPlansId",
                        column: x => x.BudgetPlansId,
                        principalTable: "BudgetPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPlanBudgetPlanRule_RulesID",
                table: "BudgetPlanBudgetPlanRule",
                column: "RulesID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetPlanBudgetPlanRule");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "BudgetPlanRules",
                newName: "BudgetPlanId");

            migrationBuilder.AddColumn<int>(
                name: "BudgetPlan",
                table: "BudgetPlanRules",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPlanRules_BudgetPlanId",
                table: "BudgetPlanRules",
                column: "BudgetPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlanRules_BudgetPlans_BudgetPlanId",
                table: "BudgetPlanRules",
                column: "BudgetPlanId",
                principalTable: "BudgetPlans",
                principalColumn: "Id");
        }
    }
}
