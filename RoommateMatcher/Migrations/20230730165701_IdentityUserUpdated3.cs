using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoommateMatcher.Migrations
{
    /// <inheritdoc />
    public partial class IdentityUserUpdated3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Gender",
                table: "AspNetUsers",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Gender",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }
    }
}
