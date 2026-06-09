using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialCare.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIconePrograma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconeBase64",
                table: "programa_social",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "programa_social",
                keyColumn: "Id",
                keyValue: 1,
                column: "IconeBase64",
                value: null);

            migrationBuilder.UpdateData(
                table: "programa_social",
                keyColumn: "Id",
                keyValue: 2,
                column: "IconeBase64",
                value: null);

            migrationBuilder.UpdateData(
                table: "programa_social",
                keyColumn: "Id",
                keyValue: 3,
                column: "IconeBase64",
                value: null);

            migrationBuilder.UpdateData(
                table: "programa_social",
                keyColumn: "Id",
                keyValue: 4,
                column: "IconeBase64",
                value: null);

            migrationBuilder.UpdateData(
                table: "programa_social",
                keyColumn: "Id",
                keyValue: 5,
                column: "IconeBase64",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconeBase64",
                table: "programa_social");
        }
    }
}
