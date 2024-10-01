using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savings.DAO.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BudgetPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    NeedsPercentage = table.Column<int>(type: "INTEGER", nullable: false),
                    WantsPercentage = table.Column<int>(type: "INTEGER", nullable: false),
                    SavingsPercentage = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetPlans", x => x.Id);
                });

            migrationBuilder.InsertData("BudgetPlans",
                new string[] { "Name", "NeedsPercentage", "WantsPercentage", "SavingsPercentage" },
                new object[] { "Standard", 50, 30, 20 });

            migrationBuilder.CreateTable(
                name: "BudgetPlanRules",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<long>(type: "INTEGER", nullable: true),
                    CategoryFilter = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryText = table.Column<string>(type: "TEXT", nullable: true),
                    BudgetPlan = table.Column<int>(type: "INTEGER", nullable: true),
                    Income = table.Column<bool>(type: "INTEGER", nullable: false),
                    BudgetPlanId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetPlanRules", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BudgetPlanRules_BudgetPlans_BudgetPlanId",
                        column: x => x.BudgetPlanId,
                        principalTable: "BudgetPlans",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BudgetPlanRules_MoneyCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "MoneyCategories",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPlanRules_BudgetPlanId",
                table: "BudgetPlanRules",
                column: "BudgetPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPlanRules_CategoryId",
                table: "BudgetPlanRules",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetPlanRules");

            migrationBuilder.DropTable(
                name: "BudgetPlans");
        }
    }
}
