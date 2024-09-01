using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savings.DAO.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecurrentMoneyItems_RecurrentMoneyItems_RecurrentMoneyItemID",
                table: "RecurrentMoneyItems");

            migrationBuilder.DropTable(
                name: "MaterializedMoneySubitems");

            migrationBuilder.DropTable(
                name: "RecurrencyAdjustements");

            migrationBuilder.DropIndex(
                name: "IX_RecurrentMoneyItems_RecurrentMoneyItemID",
                table: "RecurrentMoneyItems");

            migrationBuilder.DropColumn(
                name: "DefaultCredit",
                table: "RecurrentMoneyItems");

            migrationBuilder.DropColumn(
                name: "RecurrentMoneyItemID",
                table: "RecurrentMoneyItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DefaultCredit",
                table: "RecurrentMoneyItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "RecurrentMoneyItemID",
                table: "RecurrentMoneyItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MaterializedMoneySubitems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryID = table.Column<long>(type: "INTEGER", nullable: true),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MaterializedMoneyItemID = table.Column<long>(type: "INTEGER", nullable: true),
                    Note = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterializedMoneySubitems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterializedMoneySubitems_MaterializedMoneyItems_MaterializedMoneyItemID",
                        column: x => x.MaterializedMoneyItemID,
                        principalTable: "MaterializedMoneyItems",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MaterializedMoneySubitems_MoneyCategories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "MoneyCategories",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "RecurrencyAdjustements",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    RecurrencyDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RecurrencyNewAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    RecurrencyNewDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RecurrentMoneyItemID = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurrencyAdjustements", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RecurrencyAdjustements_RecurrentMoneyItems_RecurrentMoneyItemID",
                        column: x => x.RecurrentMoneyItemID,
                        principalTable: "RecurrentMoneyItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecurrentMoneyItems_RecurrentMoneyItemID",
                table: "RecurrentMoneyItems",
                column: "RecurrentMoneyItemID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterializedMoneySubitems_CategoryID",
                table: "MaterializedMoneySubitems",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterializedMoneySubitems_MaterializedMoneyItemID",
                table: "MaterializedMoneySubitems",
                column: "MaterializedMoneyItemID");

            migrationBuilder.CreateIndex(
                name: "IX_RecurrencyAdjustements_RecurrentMoneyItemID",
                table: "RecurrencyAdjustements",
                column: "RecurrentMoneyItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_RecurrentMoneyItems_RecurrentMoneyItems_RecurrentMoneyItemID",
                table: "RecurrentMoneyItems",
                column: "RecurrentMoneyItemID",
                principalTable: "RecurrentMoneyItems",
                principalColumn: "ID");
        }
    }
}
