using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jennifer.Account.Migrations
{
    /// <inheritdoc />
    public partial class change_audit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                schema: "account",
                table: "Audits");

            migrationBuilder.RenameColumn(
                name: "ProfileId",
                schema: "account",
                table: "Audits",
                newName: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserID",
                schema: "account",
                table: "Audits",
                newName: "ProfileId");

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                schema: "account",
                table: "Audits",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
