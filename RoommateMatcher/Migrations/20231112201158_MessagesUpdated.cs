using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoommateMatcher.Migrations
{
    /// <inheritdoc />
    public partial class MessagesUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Messages",
                newName: "SenderUserName");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Messages",
                newName: "RecieverUserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SenderUserName",
                table: "Messages",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "RecieverUserName",
                table: "Messages",
                newName: "ReceiverId");
        }
    }
}
