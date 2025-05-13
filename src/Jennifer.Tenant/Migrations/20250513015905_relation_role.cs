using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jennifer.Tenant.Migrations
{
    /// <inheritdoc />
    public partial class relation_role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "account",
                table: "Roles",
                type: "nvarchar(5)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_TenantId",
                schema: "account",
                table: "Roles",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Tenants_TenantId",
                schema: "account",
                table: "Roles",
                column: "TenantId",
                principalSchema: "account",
                principalTable: "Tenants",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Tenants_TenantId",
                schema: "account",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_TenantId",
                schema: "account",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "account",
                table: "Roles");
        }
    }
}
