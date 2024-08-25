using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savings.DAO.Migrations
{
    /// <inheritdoc />
    public partial class AddedMoneyAccountTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountID",
                table: "FixedMoneyItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "MoneyAccounts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoneyAccounts", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "MoneyAccounts",
                column: "ID",
                value: 1                
                );

            migrationBuilder.UpdateData(
                table: "MoneyAccounts",
                keyColumn: "ID",
                keyValue: 1,
                columns: ["ID", "Name"],
                values: [1, "Main Bank Account"]
                );

            migrationBuilder.UpdateData(
                table: "MaterializedMoneyItems",
                keyColumn: "ID",
                keyValue: 1L,                
                column: "Date",
                value: new DateTime(2024, 7, 31, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.CreateIndex(
                name: "IX_FixedMoneyItems_AccountID",
                table: "FixedMoneyItems",
                column: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_FixedMoneyItems_MoneyAccounts_AccountID",
                table: "FixedMoneyItems",
                column: "AccountID",
                principalTable: "MoneyAccounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FixedMoneyItems_MoneyAccounts_AccountID",
                table: "FixedMoneyItems");

            migrationBuilder.DropTable(
                name: "MoneyAccounts");

            migrationBuilder.DropIndex(
                name: "IX_FixedMoneyItems_AccountID",
                table: "FixedMoneyItems");

            migrationBuilder.DropColumn(
                name: "AccountID",
                table: "FixedMoneyItems");

            migrationBuilder.UpdateData(
                table: "MaterializedMoneyItems",
                keyColumn: "ID",
                keyValue: 1L,
                column: "Date",
                value: new DateTime(2023, 12, 31, 0, 0, 0, 0, DateTimeKind.Local));
        }
    }
}
