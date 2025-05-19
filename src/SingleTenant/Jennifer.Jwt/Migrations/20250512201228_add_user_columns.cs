using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jennifer.Jwt.Migrations
{
    /// <inheritdoc />
    public partial class add_user_columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                schema: "account",
                table: "Users",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                schema: "account",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModifiedOn",
                schema: "account",
                table: "Users",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "account",
                table: "Users",
                type: "int",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                schema: "account",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                schema: "account",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                schema: "account",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "account",
                table: "Users");
        }
    }
}
