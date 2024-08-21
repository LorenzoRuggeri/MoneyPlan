using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Savings.DAO.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecurrencyAdjustements_RecurrentMoneyItems_RecurrentMoneyItemID",
                table: "RecurrencyAdjustements");

            migrationBuilder.DropColumn(
                name: "RecurrencyMoneyItemID",
                table: "RecurrencyAdjustements");

            migrationBuilder.RenameColumn(
                name: "Root",
                table: "RecurrentMoneyItems",
                newName: "DefaultCredit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "RecurrentMoneyItems",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "RecurrentMoneyItemID",
                table: "RecurrencyAdjustements",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MoneyCategories",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "MoneyCategories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "MoneyCategories",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Cash",
                table: "MaterializedMoneyItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "EndPeriodCashCarry",
                table: "MaterializedMoneyItems",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "FixedMoneyItemID",
                table: "MaterializedMoneyItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurrent",
                table: "MaterializedMoneyItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "RecurrentMoneyItemID",
                table: "MaterializedMoneyItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "FixedMoneyItems",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AddColumn<bool>(
                name: "Cash",
                table: "FixedMoneyItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "CashWithdrawalCategoryID",
                table: "Configuration",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "MaterializedMoneySubitems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    CategoryID = table.Column<long>(type: "INTEGER", nullable: true),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    MaterializedMoneyItemID = table.Column<long>(type: "INTEGER", nullable: true)
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

            migrationBuilder.InsertData(
                table: "Configuration",
                columns: new[] { "ID", "CashWithdrawalCategoryID", "EndPeriodRecurrencyInterval", "EndPeriodRecurrencyType" },
                values: new object[] { 1L, 0L, (short)1, 2 });

            migrationBuilder.InsertData(
                table: "MaterializedMoneyItems",
                columns: new[] { "ID", "Amount", "Cash", "CategoryID", "Date", "EndPeriod", "EndPeriodCashCarry", "FixedMoneyItemID", "IsRecurrent", "Note", "Projection", "RecurrentMoneyItemID", "TimelineWeight", "Type" },
                values: new object[] { 1L, 0m, false, null, new DateTime(2023, 12, 31, 0, 0, 0, 0, DateTimeKind.Local), true, 0m, null, false, null, 0m, null, 0, 0 });

            migrationBuilder.InsertData(
                table: "MoneyCategories",
                columns: new[] { "ID", "Description", "Icon", "ParentId" },
                values: new object[,]
                {
                    { 1L, "Family", null, null },
                    { 2L, "Home", null, null },
                    { 3L, "Leisure Time", null, null },
                    { 4L, "Transports", null, null },
                    { 5L, "Financial Trading", null, null },
                    { 6L, "Tech & Information Technologies", null, null },
                    { 7L, "Other", null, null },
                    { 8L, "Salary", null, null },
                    { 16L, "Food & Groceries", null, 1L },
                    { 17L, "Mortgage", null, 2L },
                    { 18L, "Restaurant", null, 3L },
                    { 19L, "Tobacco shop", null, 3L },
                    { 20L, "Other", null, 3L },
                    { 21L, "Shows, Concerts & Museums", null, 3L },
                    { 22L, "Subscriptions", null, 3L },
                    { 23L, "Public Transport", null, 4L },
                    { 24L, "Car", null, 4L },
                    { 25L, "Loan", null, 4L },
                    { 26L, "Fuel", null, 4L },
                    { 27L, "Compravendita titoli", null, 5L },
                    { 28L, "Subscriptions", null, 5L },
                    { 29L, "Subscriptions", null, 6L },
                    { 30L, "Insurances & Policies", null, 7L },
                    { 31L, "Duties", null, 7L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoneyCategories_ParentId",
                table: "MoneyCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterializedMoneySubitems_CategoryID",
                table: "MaterializedMoneySubitems",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterializedMoneySubitems_MaterializedMoneyItemID",
                table: "MaterializedMoneySubitems",
                column: "MaterializedMoneyItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_MoneyCategories_MoneyCategories_ParentId",
                table: "MoneyCategories",
                column: "ParentId",
                principalTable: "MoneyCategories",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_RecurrencyAdjustements_RecurrentMoneyItems_RecurrentMoneyItemID",
                table: "RecurrencyAdjustements",
                column: "RecurrentMoneyItemID",
                principalTable: "RecurrentMoneyItems",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MoneyCategories_MoneyCategories_ParentId",
                table: "MoneyCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_RecurrencyAdjustements_RecurrentMoneyItems_RecurrentMoneyItemID",
                table: "RecurrencyAdjustements");

            migrationBuilder.DropTable(
                name: "MaterializedMoneySubitems");

            migrationBuilder.DropIndex(
                name: "IX_MoneyCategories_ParentId",
                table: "MoneyCategories");

            migrationBuilder.DeleteData(
                table: "Configuration",
                keyColumn: "ID",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "MaterializedMoneyItems",
                keyColumn: "ID",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 16L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 17L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 18L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 19L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 20L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 21L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 22L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 23L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 24L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 25L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 26L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 27L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 28L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 29L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 30L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 31L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 7L);

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "MoneyCategories");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "MoneyCategories");

            migrationBuilder.DropColumn(
                name: "Cash",
                table: "MaterializedMoneyItems");

            migrationBuilder.DropColumn(
                name: "EndPeriodCashCarry",
                table: "MaterializedMoneyItems");

            migrationBuilder.DropColumn(
                name: "FixedMoneyItemID",
                table: "MaterializedMoneyItems");

            migrationBuilder.DropColumn(
                name: "IsRecurrent",
                table: "MaterializedMoneyItems");

            migrationBuilder.DropColumn(
                name: "RecurrentMoneyItemID",
                table: "MaterializedMoneyItems");

            migrationBuilder.DropColumn(
                name: "Cash",
                table: "FixedMoneyItems");

            migrationBuilder.DropColumn(
                name: "CashWithdrawalCategoryID",
                table: "Configuration");

            migrationBuilder.RenameColumn(
                name: "DefaultCredit",
                table: "RecurrentMoneyItems",
                newName: "Root");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "RecurrentMoneyItems",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "RecurrentMoneyItemID",
                table: "RecurrencyAdjustements",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<long>(
                name: "RecurrencyMoneyItemID",
                table: "RecurrencyAdjustements",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "Description",
                table: "MoneyCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "FixedMoneyItems",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RecurrencyAdjustements_RecurrentMoneyItems_RecurrentMoneyItemID",
                table: "RecurrencyAdjustements",
                column: "RecurrentMoneyItemID",
                principalTable: "RecurrentMoneyItems",
                principalColumn: "ID");
        }
    }
}
