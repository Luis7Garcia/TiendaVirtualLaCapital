using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaVirtual.Migrations
{
    public partial class addCiudad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ciudad_Estados_EstadoId",
                table: "Ciudad");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ciudad",
                table: "Ciudad");

            migrationBuilder.RenameTable(
                name: "Ciudad",
                newName: "Ciudades");

            migrationBuilder.RenameIndex(
                name: "IX_Ciudad_Nombre",
                table: "Ciudades",
                newName: "IX_Ciudades_Nombre");

            migrationBuilder.RenameIndex(
                name: "IX_Ciudad_EstadoId",
                table: "Ciudades",
                newName: "IX_Ciudades_EstadoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ciudades",
                table: "Ciudades",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ciudades_Estados_EstadoId",
                table: "Ciudades",
                column: "EstadoId",
                principalTable: "Estados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ciudades_Estados_EstadoId",
                table: "Ciudades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ciudades",
                table: "Ciudades");

            migrationBuilder.RenameTable(
                name: "Ciudades",
                newName: "Ciudad");

            migrationBuilder.RenameIndex(
                name: "IX_Ciudades_Nombre",
                table: "Ciudad",
                newName: "IX_Ciudad_Nombre");

            migrationBuilder.RenameIndex(
                name: "IX_Ciudades_EstadoId",
                table: "Ciudad",
                newName: "IX_Ciudad_EstadoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ciudad",
                table: "Ciudad",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ciudad_Estados_EstadoId",
                table: "Ciudad",
                column: "EstadoId",
                principalTable: "Estados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
