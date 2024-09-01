using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savings.DAO.Migrations
{
    /// <inheritdoc />
    public partial class RelationshipOnMaterializedMoneyItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MaterializedMoneyItems_RecurrentMoneyItemID",
                table: "MaterializedMoneyItems",
                column: "RecurrentMoneyItemID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MaterializedMoneyItems_RecurrentMoneyItemID",
                table: "MaterializedMoneyItems");
        }
    }
}
