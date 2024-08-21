using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savings.DAO.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAccumulateForBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccumulateForBudget",
                table: "FixedMoneyItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AccumulateForBudget",
                table: "FixedMoneyItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
