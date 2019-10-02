using Microsoft.EntityFrameworkCore.Migrations;

namespace Cinema.Migrations
{
    public partial class second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Is3Dx",
                table: "Halls",
                newName: "Is3D");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Is3D",
                table: "Halls",
                newName: "Is3Dx");
        }
    }
}
