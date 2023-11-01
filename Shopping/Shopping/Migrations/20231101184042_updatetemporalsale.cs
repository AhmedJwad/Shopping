using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopping.Migrations
{
    /// <inheritdoc />
    public partial class updatetemporalsale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemporalSales_AspNetUsers_UserId",
                table: "TemporalSales");

            migrationBuilder.DropIndex(
                name: "IX_TemporalSales_UserId",
                table: "TemporalSales");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TemporalSales");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TemporalSales",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemporalSales_UserId",
                table: "TemporalSales",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TemporalSales_AspNetUsers_UserId",
                table: "TemporalSales",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
