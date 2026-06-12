using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialCare.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCoordenadasEndereco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "endereco",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "endereco",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "endereco");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "endereco");
        }
    }
}
