using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyPlan.DAO.Migrations
{
    /// <inheritdoc />
    public partial class MigrateBudgetPlanRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("REINDEX;");

            migrationBuilder.Sql(@"UPDATE [BudgetPlanRules] AS DEST 
                                    SET BudgetPlanId = (
                                    SELECT BudgetPlanId FROM [BudgetPlanBudgetPlanRule] AS SOURCE
                                    WHERE SOURCE.BudgetPlanRuleId = DEST.Id)
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
