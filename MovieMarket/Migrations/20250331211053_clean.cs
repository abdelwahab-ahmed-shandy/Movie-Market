using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieMarket.Migrations
{
    /// <inheritdoc />
    public partial class clean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CinemaMovie_Cinemas_CinemaId",
                table: "CinemaMovie");

            migrationBuilder.DropForeignKey(
                name: "FK_CinemaMovie_Movies_MovieId",
                table: "CinemaMovie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CinemaMovie",
                table: "CinemaMovie");

            migrationBuilder.RenameTable(
                name: "CinemaMovie",
                newName: "CinemaMovies");

            migrationBuilder.RenameIndex(
                name: "IX_CinemaMovie_MovieId",
                table: "CinemaMovies",
                newName: "IX_CinemaMovies_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CinemaMovies",
                table: "CinemaMovies",
                columns: new[] { "CinemaId", "MovieId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CinemaMovies_Cinemas_CinemaId",
                table: "CinemaMovies",
                column: "CinemaId",
                principalTable: "Cinemas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CinemaMovies_Movies_MovieId",
                table: "CinemaMovies",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CinemaMovies_Cinemas_CinemaId",
                table: "CinemaMovies");

            migrationBuilder.DropForeignKey(
                name: "FK_CinemaMovies_Movies_MovieId",
                table: "CinemaMovies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CinemaMovies",
                table: "CinemaMovies");

            migrationBuilder.RenameTable(
                name: "CinemaMovies",
                newName: "CinemaMovie");

            migrationBuilder.RenameIndex(
                name: "IX_CinemaMovies_MovieId",
                table: "CinemaMovie",
                newName: "IX_CinemaMovie_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CinemaMovie",
                table: "CinemaMovie",
                columns: new[] { "CinemaId", "MovieId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CinemaMovie_Cinemas_CinemaId",
                table: "CinemaMovie",
                column: "CinemaId",
                principalTable: "Cinemas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CinemaMovie_Movies_MovieId",
                table: "CinemaMovie",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
