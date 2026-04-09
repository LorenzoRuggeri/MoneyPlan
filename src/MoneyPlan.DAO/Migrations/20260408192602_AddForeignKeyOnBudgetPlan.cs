using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyPlan.DAO.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyOnBudgetPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure there's at least one BudgetPlan to reference (if no BudgetPlans exist).
            migrationBuilder.Sql(@"
                INSERT INTO BudgetPlans (Name, NeedsPercentage, WantsPercentage, SavingsPercentage)
                SELECT 'Default Plan', 50, 30, 20
                WHERE NOT EXISTS (SELECT 1 FROM BudgetPlans);
            ");

            // Replace NULL BudgetPlanId values with an existing BudgetPlan Id to avoid FK violations.
            migrationBuilder.Sql(@"
                UPDATE BudgetPlanRules
                SET BudgetPlanId = (SELECT Id FROM BudgetPlans LIMIT 1)
                WHERE BudgetPlanId IS NULL;
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanRules_BudgetPlans_BudgetPlanId",
                table: "BudgetPlanRules");

            migrationBuilder.DropColumn(
                name: "Income",
                table: "BudgetPlanRules");

            migrationBuilder.AlterColumn<int>(
                name: "BudgetPlanId",
                table: "BudgetPlanRules",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlanRules_BudgetPlans_BudgetPlanId",
                table: "BudgetPlanRules",
                column: "BudgetPlanId",
                principalTable: "BudgetPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlanRules_BudgetPlans_BudgetPlanId",
                table: "BudgetPlanRules");

            migrationBuilder.AlterColumn<int>(
                name: "BudgetPlanId",
                table: "BudgetPlanRules",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<bool>(
                name: "Income",
                table: "BudgetPlanRules",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlanRules_BudgetPlans_BudgetPlanId",
                table: "BudgetPlanRules",
                column: "BudgetPlanId",
                principalTable: "BudgetPlans",
                principalColumn: "Id");
        }
    }
}
