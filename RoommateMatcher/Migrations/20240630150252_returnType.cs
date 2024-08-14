using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoommateMatcher.Migrations
{
    /// <inheritdoc />
    public partial class returnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "P",
                table: "DiffieHellmanParams",
                type: "text",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(1000,0)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "P",
                table: "DiffieHellmanParams",
                type: "numeric(1000,0)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
