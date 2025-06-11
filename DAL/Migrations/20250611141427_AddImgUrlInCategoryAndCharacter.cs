using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddImgUrlInCategoryAndCharacter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000002"),
                columns: new[] { "CurrentState", "ImgUrl" },
                values: new object[] { 2, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000001"),
                columns: new[] { "CurrentState", "ImgUrl" },
                values: new object[] { 2, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0020-000000000003"),
                columns: new[] { "CurrentState", "ImgUrl" },
                values: new object[] { 2, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0030-000000000004"),
                columns: new[] { "CurrentState", "ImgUrl" },
                values: new object[] { 2, null });

            migrationBuilder.UpdateData(
                table: "CharacterTvSeries",
                keyColumns: new[] { "CharacterId", "TvSeriesId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000301"), new Guid("00000000-0000-0000-0000-000000000101") },
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "CharacterTvSeries",
                keyColumns: new[] { "CharacterId", "TvSeriesId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000302"), new Guid("00000000-0000-0000-0000-000000000101") },
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "CharacterTvSeries",
                keyColumns: new[] { "CharacterId", "TvSeriesId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000303"), new Guid("00000000-0000-0000-0000-000000000102") },
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "CharacterTvSeries",
                keyColumns: new[] { "CharacterId", "TvSeriesId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000304"), new Guid("00000000-0000-0000-0000-000000000102") },
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Characters",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000301"),
                columns: new[] { "CurrentState", "ImgUrl" },
                values: new object[] { 2, null });

            migrationBuilder.UpdateData(
                table: "Characters",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000302"),
                columns: new[] { "CurrentState", "ImgUrl" },
                values: new object[] { 2, null });

            migrationBuilder.UpdateData(
                table: "Characters",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000303"),
                columns: new[] { "CurrentState", "ImgUrl" },
                values: new object[] { 2, null });

            migrationBuilder.UpdateData(
                table: "Characters",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000304"),
                columns: new[] { "CurrentState", "ImgUrl" },
                values: new object[] { 2, null });

            migrationBuilder.UpdateData(
                table: "Characters",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000305"),
                columns: new[] { "CurrentState", "ImgUrl" },
                values: new object[] { 2, null });

            migrationBuilder.UpdateData(
                table: "CinemaMovies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000801"),
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "CinemaMovies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000802"),
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "CinemaMovies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000803"),
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Cinemas",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000701"),
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Cinemas",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000702"),
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Episodes",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000601"),
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Episodes",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000602"),
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Episodes",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000603"),
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000201"),
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000202"),
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000203"),
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Seasons",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000501"),
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Seasons",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000502"),
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Seasons",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000503"),
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "TvSeries",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000101"),
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "TvSeries",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000102"),
                column: "CurrentState",
                value: 2);

            migrationBuilder.UpdateData(
                table: "TvSeries",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000103"),
                column: "CurrentState",
                value: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "Categories");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000002"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000001"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0020-000000000003"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0030-000000000004"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "CharacterTvSeries",
                keyColumns: new[] { "CharacterId", "TvSeriesId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000301"), new Guid("00000000-0000-0000-0000-000000000101") },
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "CharacterTvSeries",
                keyColumns: new[] { "CharacterId", "TvSeriesId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000302"), new Guid("00000000-0000-0000-0000-000000000101") },
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "CharacterTvSeries",
                keyColumns: new[] { "CharacterId", "TvSeriesId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000303"), new Guid("00000000-0000-0000-0000-000000000102") },
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "CharacterTvSeries",
                keyColumns: new[] { "CharacterId", "TvSeriesId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000304"), new Guid("00000000-0000-0000-0000-000000000102") },
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Characters",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000301"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Characters",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000302"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Characters",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000303"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Characters",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000304"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Characters",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000305"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "CinemaMovies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000801"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "CinemaMovies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000802"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "CinemaMovies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000803"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Cinemas",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000701"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Cinemas",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000702"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Episodes",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000601"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Episodes",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000602"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Episodes",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000603"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000201"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000202"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000203"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Seasons",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000501"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Seasons",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000502"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Seasons",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000503"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "TvSeries",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000101"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "TvSeries",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000102"),
                column: "CurrentState",
                value: 0);

            migrationBuilder.UpdateData(
                table: "TvSeries",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000103"),
                column: "CurrentState",
                value: 0);
        }
    }
}
