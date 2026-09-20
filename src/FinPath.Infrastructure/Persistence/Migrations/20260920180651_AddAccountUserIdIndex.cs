using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinPath.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountUserIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_accounts_user_id",
                table: "accounts",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_accounts_user_id",
                table: "accounts");
        }
    }
}
