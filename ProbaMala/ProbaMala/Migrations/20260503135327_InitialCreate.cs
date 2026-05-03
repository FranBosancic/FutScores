using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProbaMala.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    LastName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    FoundedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LeagueId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clubs_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LeagueId = table.Column<int>(type: "integer", nullable: false),
                    HomeTeamId = table.Column<int>(type: "integer", nullable: false),
                    AwayTeamId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    HomeGoals = table.Column<int>(type: "integer", nullable: false),
                    AwayGoals = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Clubs_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Clubs_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    LastName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    ClubId = table.Column<int>(type: "integer", nullable: false),
                    Nationality = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    MatchId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ratings_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Leagues",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Premier League" },
                    { 2, "La Liga" },
                    { 3, "Serie A" },
                    { 4, "Bundesliga" },
                    { 5, "Ligue 1" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName" },
                values: new object[,]
                {
                    { 1, "john.smith@email.com", "John", "Smith" },
                    { 2, "maria.garcia@email.com", "Maria", "Garcia" },
                    { 3, "alex.johnson@email.com", "Alex", "Johnson" },
                    { 4, "sofia.rossi@email.com", "Sofia", "Rossi" },
                    { 5, "lucas.silva@email.com", "Lucas", "Silva" }
                });

            migrationBuilder.InsertData(
                table: "Clubs",
                columns: new[] { "Id", "FoundedDate", "LeagueId", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(1878, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Manchester United" },
                    { 2, new DateTime(1892, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Liverpool FC" },
                    { 3, new DateTime(1880, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Manchester City" },
                    { 4, new DateTime(1902, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Real Madrid" },
                    { 5, new DateTime(1899, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "FC Barcelona" },
                    { 6, new DateTime(1899, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "AC Milan" },
                    { 7, new DateTime(1897, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Juventus" },
                    { 8, new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Bayern Munich" },
                    { 9, new DateTime(1909, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Borussia Dortmund" },
                    { 10, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "Paris Saint-Germain" }
                });

            migrationBuilder.InsertData(
                table: "Matches",
                columns: new[] { "Id", "AwayGoals", "AwayTeamId", "Date", "HomeGoals", "HomeTeamId", "LeagueId" },
                values: new object[,]
                {
                    { 1, 1, 2, new DateTime(2024, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, 1 },
                    { 2, 1, 1, new DateTime(2024, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, 1 },
                    { 3, 2, 3, new DateTime(2024, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2, 1 },
                    { 4, 2, 5, new DateTime(2024, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 4, 2 },
                    { 5, 1, 4, new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 5, 2 },
                    { 6, 0, 7, new DateTime(2024, 4, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 6, 3 },
                    { 7, 1, 9, new DateTime(2024, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 8, 4 },
                    { 8, 3, 1, new DateTime(2024, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 5 }
                });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "Id", "ClubId", "DateOfBirth", "FirstName", "LastName", "Nationality", "Position" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(1985, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cristiano", "Ronaldo", "Portugal", 3 },
                    { 2, 1, new DateTime(1993, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Harry", "Maguire", "England", 1 },
                    { 3, 1, new DateTime(1990, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "David", "De Gea", "Spain", 0 },
                    { 4, 2, new DateTime(1992, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mohamed", "Salah", "Egypt", 3 },
                    { 5, 2, new DateTime(1991, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Virgil", "van Dijk", "Netherlands", 1 },
                    { 6, 3, new DateTime(2000, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Erling", "Haaland", "Norway", 3 },
                    { 7, 3, new DateTime(1991, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kevin", "De Bruyne", "Belgium", 2 },
                    { 8, 4, new DateTime(1987, 12, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Karim", "Benzema", "France", 3 },
                    { 9, 4, new DateTime(1985, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Luka", "Modric", "Croatia", 2 },
                    { 10, 5, new DateTime(1988, 7, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sergio", "Busquets", "Spain", 2 },
                    { 11, 8, new DateTime(1988, 8, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Robert", "Lewandowski", "Poland", 3 },
                    { 12, 8, new DateTime(2003, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jamal", "Musiala", "Germany", 2 },
                    { 13, 9, new DateTime(1990, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Marco", "Reus", "Germany", 3 },
                    { 14, 10, new DateTime(1989, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Thomas", "Tuchel", "France", 2 },
                    { 15, 10, new DateTime(1998, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kylian", "Mbappe", "France", 3 }
                });

            migrationBuilder.InsertData(
                table: "Ratings",
                columns: new[] { "Id", "Comment", "MatchId", "PlayerId", "Score", "UserId" },
                values: new object[,]
                {
                    { 1, "Excellent performance!", 1, 1, 9, 1 },
                    { 2, "Solid defense", 1, 2, 7, 2 },
                    { 3, "Great goal!", 3, 4, 8, 1 },
                    { 4, "Defensive masterclass", 3, 5, 7, 3 },
                    { 5, "Best striker in the league", 2, 6, 9, 2 },
                    { 6, "Great midfield control", 2, 7, 8, 4 },
                    { 7, "Clinical finishing", 7, 11, 9, 5 },
                    { 8, "Impressive technical skills", 7, 12, 8, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clubs_LeagueId",
                table: "Clubs",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_AwayTeamId",
                table: "Matches",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_HomeTeamId",
                table: "Matches",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_LeagueId",
                table: "Matches",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_ClubId",
                table: "Players",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_MatchId",
                table: "Ratings",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_PlayerId",
                table: "Ratings",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserId",
                table: "Ratings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Clubs");

            migrationBuilder.DropTable(
                name: "Leagues");
        }
    }
}
