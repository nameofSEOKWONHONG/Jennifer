using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jennifer.Account.Migrations
{
    /// <inheritdoc />
    public partial class change_audit_userid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserID",
                schema: "account",
                table: "Audits",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "account",
                table: "Audits",
                newName: "UserID");
        }
    }
}
