using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savings.DAO.Migrations
{
    /// <inheritdoc />
    public partial class AddedOccurrencyForRecurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OccurrencyType",
                table: "RecurrentMoneyItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OccurrencyType",
                table: "RecurrentMoneyItems");
        }
    }
}
