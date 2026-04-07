using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyPlan.DAO.Migrations
{
    /// <inheritdoc />
    public partial class DeleteTableBudgetPlanBudgetPlanRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetPlanBudgetPlanRule");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BudgetPlanBudgetPlanRule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BudgetPlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    BudgetPlanRuleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetPlanBudgetPlanRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetPlanBudgetPlanRule_BudgetPlanRules_BudgetPlanRuleId",
                        column: x => x.BudgetPlanRuleId,
                        principalTable: "BudgetPlanRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetPlanBudgetPlanRule_BudgetPlans_BudgetPlanId",
                        column: x => x.BudgetPlanId,
                        principalTable: "BudgetPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPlanBudgetPlanRule_BudgetPlanId_BudgetPlanRuleId",
                table: "BudgetPlanBudgetPlanRule",
                columns: new[] { "BudgetPlanId", "BudgetPlanRuleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPlanBudgetPlanRule_BudgetPlanRuleId",
                table: "BudgetPlanBudgetPlanRule",
                column: "BudgetPlanRuleId");
        }
    }
}
