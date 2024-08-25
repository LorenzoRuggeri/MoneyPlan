using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savings.DAO.Migrations
{
    /// <inheritdoc />
    public partial class AddedMoneyAccountONRecurrentMoneyItemsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MoneyAccountId",
                table: "RecurrentMoneyItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecurrentMoneyItems_MoneyAccountId",
                table: "RecurrentMoneyItems",
                column: "MoneyAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecurrentMoneyItems_MoneyAccounts_MoneyAccountId",
                table: "RecurrentMoneyItems",
                column: "MoneyAccountId",
                principalTable: "MoneyAccounts",
                principalColumn: "ID");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecurrentMoneyItems_MoneyAccounts_MoneyAccountId",
                table: "RecurrentMoneyItems");

            migrationBuilder.DropIndex(
                name: "IX_RecurrentMoneyItems_MoneyAccountId",
                table: "RecurrentMoneyItems");

            migrationBuilder.DropColumn(
                name: "MoneyAccountId",
                table: "RecurrentMoneyItems");
        }
    }
}
