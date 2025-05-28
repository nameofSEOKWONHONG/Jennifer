using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jennifer.Account.Migrations
{
    /// <inheritdoc />
    public partial class change_2fa_secretkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TwoFactorSecretKey",
                schema: "account",
                table: "Users",
                newName: "AuthenticatorKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthenticatorKey",
                schema: "account",
                table: "Users",
                newName: "TwoFactorSecretKey");
        }
    }
}
