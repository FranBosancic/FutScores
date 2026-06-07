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
                    { 1, "liam.oconnor@futscores.app", "Liam", "O'Connor" },
                    { 2, "sofia.marchetti@futscores.app", "Sofia", "Marchetti" },
                    { 3, "mateo.fernandez@futscores.app", "Mateo", "Fernández" },
                    { 4, "emma.schneider@futscores.app", "Emma", "Schneider" },
                    { 5, "hugo.lefevre@futscores.app", "Hugo", "Lefèvre" },
                    { 6, "olivia.bennett@futscores.app", "Olivia", "Bennett" },
                    { 7, "noah.andersson@futscores.app", "Noah", "Andersson" },
                    { 8, "chiara.romano@futscores.app", "Chiara", "Romano" },
                    { 9, "daniel.kovac@futscores.app", "Daniel", "Kovač" },
                    { 10, "aisha.rahman@futscores.app", "Aisha", "Rahman" }
                });

            migrationBuilder.InsertData(
                table: "Clubs",
                columns: new[] { "Id", "FoundedDate", "LeagueId", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(1886, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Arsenal" },
                    { 2, new DateTime(1880, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Manchester City" },
                    { 3, new DateTime(1878, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Manchester United" },
                    { 4, new DateTime(1874, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Aston Villa" },
                    { 5, new DateTime(1892, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Liverpool" },
                    { 6, new DateTime(1905, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Chelsea" },
                    { 7, new DateTime(1892, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Newcastle United" },
                    { 8, new DateTime(1882, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Tottenham Hotspur" },
                    { 9, new DateTime(1899, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "FC Barcelona" },
                    { 10, new DateTime(1902, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Real Madrid" },
                    { 11, new DateTime(1903, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Atlético Madrid" },
                    { 12, new DateTime(1923, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Villarreal" },
                    { 13, new DateTime(1907, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Real Betis" },
                    { 14, new DateTime(1898, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Athletic Bilbao" },
                    { 15, new DateTime(1909, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Real Sociedad" },
                    { 16, new DateTime(1890, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Sevilla" },
                    { 17, new DateTime(1908, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Inter Milan" },
                    { 18, new DateTime(1899, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "AC Milan" },
                    { 19, new DateTime(1897, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Juventus" },
                    { 20, new DateTime(1926, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Napoli" },
                    { 21, new DateTime(1927, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Roma" },
                    { 22, new DateTime(1907, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Atalanta" },
                    { 23, new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Lazio" },
                    { 24, new DateTime(1926, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Fiorentina" },
                    { 25, new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Bayern Munich" },
                    { 26, new DateTime(1909, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Borussia Dortmund" },
                    { 27, new DateTime(2009, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "RB Leipzig" },
                    { 28, new DateTime(1904, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Bayer Leverkusen" },
                    { 29, new DateTime(1893, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "VfB Stuttgart" },
                    { 30, new DateTime(1899, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Eintracht Frankfurt" },
                    { 31, new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Borussia Mönchengladbach" },
                    { 32, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "Paris Saint-Germain" },
                    { 33, new DateTime(1899, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "Marseille" },
                    { 34, new DateTime(1924, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "Monaco" },
                    { 35, new DateTime(1950, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "Lyon" },
                    { 36, new DateTime(1944, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "Lille" },
                    { 37, new DateTime(1906, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "Lens" }
                });

            migrationBuilder.InsertData(
                table: "Matches",
                columns: new[] { "Id", "AwayGoals", "AwayTeamId", "Date", "HomeGoals", "HomeTeamId", "LeagueId" },
                values: new object[,]
                {
                    { 1, 1, 2, new DateTime(2025, 9, 21, 16, 30, 0, 0, DateTimeKind.Unspecified), 1, 1, 1 },
                    { 2, 1, 1, new DateTime(2026, 2, 22, 16, 30, 0, 0, DateTimeKind.Unspecified), 2, 2, 1 },
                    { 3, 2, 3, new DateTime(2026, 1, 18, 16, 30, 0, 0, DateTimeKind.Unspecified), 1, 5, 1 },
                    { 4, 0, 5, new DateTime(2025, 8, 24, 16, 30, 0, 0, DateTimeKind.Unspecified), 2, 3, 1 },
                    { 5, 1, 8, new DateTime(2025, 11, 23, 14, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, 1 },
                    { 6, 1, 1, new DateTime(2025, 11, 30, 16, 30, 0, 0, DateTimeKind.Unspecified), 1, 6, 1 },
                    { 7, 0, 5, new DateTime(2025, 11, 9, 16, 30, 0, 0, DateTimeKind.Unspecified), 3, 2, 1 },
                    { 8, 2, 1, new DateTime(2025, 9, 28, 14, 0, 0, 0, DateTimeKind.Unspecified), 1, 7, 1 },
                    { 9, 0, 2, new DateTime(2026, 1, 25, 14, 0, 0, 0, DateTimeKind.Unspecified), 1, 4, 1 },
                    { 10, 1, 9, new DateTime(2025, 10, 26, 21, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 2 },
                    { 11, 0, 10, new DateTime(2026, 5, 10, 21, 0, 0, 0, DateTimeKind.Unspecified), 2, 9, 2 },
                    { 12, 1, 11, new DateTime(2025, 12, 21, 21, 0, 0, 0, DateTimeKind.Unspecified), 3, 9, 2 },
                    { 13, 2, 13, new DateTime(2026, 3, 8, 18, 30, 0, 0, DateTimeKind.Unspecified), 0, 16, 2 },
                    { 14, 2, 14, new DateTime(2025, 11, 2, 18, 30, 0, 0, DateTimeKind.Unspecified), 3, 15, 2 },
                    { 15, 2, 9, new DateTime(2026, 2, 15, 21, 0, 0, 0, DateTimeKind.Unspecified), 0, 12, 2 },
                    { 16, 1, 18, new DateTime(2025, 10, 19, 20, 45, 0, 0, DateTimeKind.Unspecified), 0, 17, 3 },
                    { 17, 0, 17, new DateTime(2026, 2, 1, 20, 45, 0, 0, DateTimeKind.Unspecified), 1, 18, 3 },
                    { 18, 3, 17, new DateTime(2025, 9, 13, 18, 0, 0, 0, DateTimeKind.Unspecified), 4, 19, 3 },
                    { 19, 1, 17, new DateTime(2025, 11, 30, 20, 45, 0, 0, DateTimeKind.Unspecified), 3, 20, 3 },
                    { 20, 0, 19, new DateTime(2025, 11, 23, 20, 45, 0, 0, DateTimeKind.Unspecified), 0, 18, 3 },
                    { 21, 0, 23, new DateTime(2026, 1, 11, 20, 45, 0, 0, DateTimeKind.Unspecified), 2, 21, 3 },
                    { 22, 2, 20, new DateTime(2026, 3, 1, 20, 45, 0, 0, DateTimeKind.Unspecified), 2, 17, 3 },
                    { 23, 0, 18, new DateTime(2026, 4, 5, 20, 45, 0, 0, DateTimeKind.Unspecified), 0, 19, 3 },
                    { 24, 1, 25, new DateTime(2026, 4, 4, 18, 30, 0, 0, DateTimeKind.Unspecified), 2, 26, 4 },
                    { 25, 0, 27, new DateTime(2025, 9, 20, 18, 30, 0, 0, DateTimeKind.Unspecified), 6, 25, 4 },
                    { 26, 1, 25, new DateTime(2026, 2, 7, 18, 30, 0, 0, DateTimeKind.Unspecified), 1, 28, 4 },
                    { 27, 1, 27, new DateTime(2025, 10, 25, 15, 30, 0, 0, DateTimeKind.Unspecified), 1, 26, 4 },
                    { 28, 5, 26, new DateTime(2026, 3, 14, 15, 30, 0, 0, DateTimeKind.Unspecified), 1, 27, 4 },
                    { 29, 0, 33, new DateTime(2025, 9, 21, 20, 45, 0, 0, DateTimeKind.Unspecified), 5, 32, 5 },
                    { 30, 0, 32, new DateTime(2026, 3, 15, 20, 45, 0, 0, DateTimeKind.Unspecified), 1, 33, 5 },
                    { 31, 3, 34, new DateTime(2025, 12, 14, 20, 45, 0, 0, DateTimeKind.Unspecified), 1, 32, 5 },
                    { 32, 2, 35, new DateTime(2025, 11, 8, 20, 0, 0, 0, DateTimeKind.Unspecified), 3, 33, 5 },
                    { 33, 1, 32, new DateTime(2026, 1, 31, 20, 0, 0, 0, DateTimeKind.Unspecified), 1, 36, 5 },
                    { 34, 2, 35, new DateTime(2026, 2, 8, 20, 45, 0, 0, DateTimeKind.Unspecified), 1, 32, 5 }
                });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "Id", "ClubId", "DateOfBirth", "FirstName", "LastName", "Nationality", "Position" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(1995, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "David", "Raya", "Spain", 0 },
                    { 2, 1, new DateTime(2001, 3, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "William", "Saliba", "France", 1 },
                    { 3, 1, new DateTime(1997, 12, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gabriel", "Magalhães", "Brazil", 1 },
                    { 4, 1, new DateTime(1999, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Declan", "Rice", "England", 2 },
                    { 5, 1, new DateTime(1998, 12, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Martin", "Ødegaard", "Norway", 2 },
                    { 6, 1, new DateTime(1999, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Martín", "Zubimendi", "Spain", 2 },
                    { 7, 1, new DateTime(2001, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bukayo", "Saka", "England", 3 },
                    { 8, 1, new DateTime(2001, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gabriel", "Martinelli", "Brazil", 3 },
                    { 9, 1, new DateTime(1998, 6, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Viktor", "Gyökeres", "Sweden", 3 },
                    { 10, 1, new DateTime(1999, 6, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kai", "Havertz", "Germany", 3 },
                    { 11, 2, new DateTime(1999, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gianluigi", "Donnarumma", "Italy", 0 },
                    { 12, 2, new DateTime(1997, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rúben", "Dias", "Portugal", 1 },
                    { 13, 2, new DateTime(2002, 1, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Joško", "Gvardiol", "Croatia", 1 },
                    { 14, 2, new DateTime(1994, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bernardo", "Silva", "Portugal", 2 },
                    { 15, 2, new DateTime(2000, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Phil", "Foden", "England", 2 },
                    { 16, 2, new DateTime(1998, 7, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tijjani", "Reijnders", "Netherlands", 2 },
                    { 17, 2, new DateTime(2003, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rayan", "Cherki", "France", 2 },
                    { 18, 2, new DateTime(2000, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Erling", "Haaland", "Norway", 3 },
                    { 19, 2, new DateTime(2002, 5, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jérémy", "Doku", "Belgium", 3 },
                    { 20, 3, new DateTime(1996, 4, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "André", "Onana", "Cameroon", 0 },
                    { 21, 3, new DateTime(1999, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Matthijs", "de Ligt", "Netherlands", 1 },
                    { 22, 3, new DateTime(2005, 11, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Leny", "Yoro", "France", 1 },
                    { 23, 3, new DateTime(1994, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bruno", "Fernandes", "Portugal", 2 },
                    { 24, 3, new DateTime(2005, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kobbie", "Mainoo", "England", 2 },
                    { 25, 3, new DateTime(2001, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Manuel", "Ugarte", "Uruguay", 2 },
                    { 26, 3, new DateTime(1999, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bryan", "Mbeumo", "Cameroon", 3 },
                    { 27, 3, new DateTime(1999, 5, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Matheus", "Cunha", "Brazil", 3 },
                    { 28, 3, new DateTime(2003, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Benjamin", "Šeško", "Slovenia", 3 },
                    { 29, 4, new DateTime(1992, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Emiliano", "Martínez", "Argentina", 0 },
                    { 30, 4, new DateTime(1997, 10, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ezri", "Konsa", "England", 1 },
                    { 31, 4, new DateTime(1997, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pau", "Torres", "Spain", 1 },
                    { 32, 4, new DateTime(1993, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lucas", "Digne", "France", 1 },
                    { 33, 4, new DateTime(1994, 10, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "John", "McGinn", "Scotland", 2 },
                    { 34, 4, new DateTime(1997, 5, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Youri", "Tielemans", "Belgium", 2 },
                    { 35, 4, new DateTime(2002, 7, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Morgan", "Rogers", "England", 2 },
                    { 36, 4, new DateTime(1995, 12, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ollie", "Watkins", "England", 3 },
                    { 37, 4, new DateTime(1996, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Emiliano", "Buendía", "Argentina", 2 },
                    { 38, 5, new DateTime(1992, 10, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alisson", "Becker", "Brazil", 0 },
                    { 39, 5, new DateTime(1991, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Virgil", "van Dijk", "Netherlands", 1 },
                    { 40, 5, new DateTime(1999, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ibrahima", "Konaté", "France", 1 },
                    { 41, 5, new DateTime(2003, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Milos", "Kerkez", "Hungary", 1 },
                    { 42, 5, new DateTime(2002, 5, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ryan", "Gravenberch", "Netherlands", 2 },
                    { 43, 5, new DateTime(2000, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dominik", "Szoboszlai", "Hungary", 2 },
                    { 44, 5, new DateTime(1998, 12, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alexis", "Mac Allister", "Argentina", 2 },
                    { 45, 5, new DateTime(2003, 5, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Florian", "Wirtz", "Germany", 2 },
                    { 46, 5, new DateTime(1992, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mohamed", "Salah", "Egypt", 3 },
                    { 47, 5, new DateTime(1999, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alexander", "Isak", "Sweden", 3 },
                    { 48, 6, new DateTime(1997, 11, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Robert", "Sánchez", "Spain", 0 },
                    { 49, 6, new DateTime(2003, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Levi", "Colwill", "England", 1 },
                    { 50, 6, new DateTime(1999, 12, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Reece", "James", "England", 1 },
                    { 51, 6, new DateTime(1998, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Marc", "Cucurella", "Spain", 1 },
                    { 52, 6, new DateTime(2001, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Moisés", "Caicedo", "Ecuador", 2 },
                    { 53, 6, new DateTime(2001, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Enzo", "Fernández", "Argentina", 2 },
                    { 54, 6, new DateTime(2002, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cole", "Palmer", "England", 2 },
                    { 55, 6, new DateTime(2001, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "João", "Pedro", "Brazil", 3 },
                    { 56, 6, new DateTime(2000, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pedro", "Neto", "Portugal", 3 },
                    { 57, 7, new DateTime(1992, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nick", "Pope", "England", 0 },
                    { 58, 7, new DateTime(2000, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sven", "Botman", "Netherlands", 1 },
                    { 59, 7, new DateTime(1990, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kieran", "Trippier", "England", 1 },
                    { 60, 7, new DateTime(1997, 11, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bruno", "Guimarães", "Brazil", 2 },
                    { 61, 7, new DateTime(2000, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sandro", "Tonali", "Italy", 2 },
                    { 62, 7, new DateTime(1996, 8, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Joelinton", "Cássio", "Brazil", 2 },
                    { 63, 7, new DateTime(2001, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Anthony", "Gordon", "England", 3 },
                    { 64, 7, new DateTime(2002, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nick", "Woltemade", "Germany", 3 },
                    { 65, 7, new DateTime(2002, 4, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Anthony", "Elanga", "Sweden", 3 },
                    { 66, 8, new DateTime(1996, 10, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Guglielmo", "Vicario", "Italy", 0 },
                    { 67, 8, new DateTime(1998, 4, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cristian", "Romero", "Argentina", 1 },
                    { 68, 8, new DateTime(2001, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Micky", "van de Ven", "Netherlands", 1 },
                    { 69, 8, new DateTime(1999, 9, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pedro", "Porro", "Spain", 1 },
                    { 70, 8, new DateTime(1996, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "James", "Maddison", "England", 2 },
                    { 71, 8, new DateTime(2003, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Xavi", "Simons", "Netherlands", 2 },
                    { 72, 8, new DateTime(2000, 8, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mohammed", "Kudus", "Ghana", 3 },
                    { 73, 8, new DateTime(1997, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dominic", "Solanke", "England", 3 },
                    { 74, 8, new DateTime(1998, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Randal", "Kolo Muani", "France", 3 },
                    { 75, 9, new DateTime(2001, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Joan", "García", "Spain", 0 },
                    { 76, 9, new DateTime(2007, 1, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pau", "Cubarsí", "Spain", 1 },
                    { 77, 9, new DateTime(1999, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ronald", "Araújo", "Uruguay", 1 },
                    { 78, 9, new DateTime(1998, 11, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jules", "Koundé", "France", 1 },
                    { 79, 9, new DateTime(2002, 11, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pedri", "González", "Spain", 2 },
                    { 80, 9, new DateTime(2004, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gavi", "Páez", "Spain", 2 },
                    { 81, 9, new DateTime(1997, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Frenkie", "de Jong", "Netherlands", 2 },
                    { 82, 9, new DateTime(1996, 12, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Raphinha", "Dias", "Brazil", 3 },
                    { 83, 9, new DateTime(2007, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lamine", "Yamal", "Spain", 3 },
                    { 84, 9, new DateTime(1988, 8, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Robert", "Lewandowski", "Poland", 3 },
                    { 85, 10, new DateTime(1992, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Thibaut", "Courtois", "Belgium", 0 },
                    { 86, 10, new DateTime(1992, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dani", "Carvajal", "Spain", 1 },
                    { 87, 10, new DateTime(1998, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Éder", "Militão", "Brazil", 1 },
                    { 88, 10, new DateTime(1993, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Antonio", "Rüdiger", "Germany", 1 },
                    { 89, 10, new DateTime(1998, 10, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trent", "Alexander-Arnold", "England", 1 },
                    { 90, 10, new DateTime(2000, 1, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Aurélien", "Tchouaméni", "France", 2 },
                    { 91, 10, new DateTime(2003, 6, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jude", "Bellingham", "England", 2 },
                    { 92, 10, new DateTime(1998, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Federico", "Valverde", "Uruguay", 2 },
                    { 93, 10, new DateTime(1998, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kylian", "Mbappé", "France", 3 },
                    { 94, 10, new DateTime(2000, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vinícius", "Júnior", "Brazil", 3 },
                    { 95, 10, new DateTime(2001, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rodrygo", "Goes", "Brazil", 3 },
                    { 96, 11, new DateTime(1993, 1, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jan", "Oblak", "Slovenia", 0 },
                    { 97, 11, new DateTime(1996, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Robin", "Le Normand", "Spain", 1 },
                    { 98, 11, new DateTime(1995, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "José María", "Giménez", "Uruguay", 1 },
                    { 99, 11, new DateTime(1998, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nahuel", "Molina", "Argentina", 1 },
                    { 100, 11, new DateTime(1995, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Marcos", "Llorente", "Spain", 2 },
                    { 101, 11, new DateTime(2003, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pablo", "Barrios", "Spain", 2 },
                    { 102, 11, new DateTime(2001, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Álex", "Baena", "Spain", 2 },
                    { 103, 11, new DateTime(1991, 3, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Antoine", "Griezmann", "France", 3 },
                    { 104, 11, new DateTime(2000, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Julián", "Álvarez", "Argentina", 3 },
                    { 105, 11, new DateTime(1995, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alexander", "Sørloth", "Norway", 3 },
                    { 106, 12, new DateTime(1989, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dani", "Parejo", "Spain", 2 },
                    { 107, 12, new DateTime(1998, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Juan", "Foyth", "Argentina", 1 },
                    { 108, 12, new DateTime(1993, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Thomas", "Partey", "Ghana", 2 },
                    { 109, 12, new DateTime(1992, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gerard", "Moreno", "Spain", 3 },
                    { 110, 12, new DateTime(1993, 7, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ayoze", "Pérez", "Spain", 3 },
                    { 111, 12, new DateTime(1995, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nicolas", "Pépé", "Ivory Coast", 3 },
                    { 112, 12, new DateTime(1996, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alfonso", "Pedraza", "Spain", 1 },
                    { 113, 13, new DateTime(1995, 12, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pau", "López", "Spain", 0 },
                    { 114, 13, new DateTime(1995, 3, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Héctor", "Bellerín", "Spain", 1 },
                    { 115, 13, new DateTime(1991, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Marc", "Bartra", "Spain", 1 },
                    { 116, 13, new DateTime(1992, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Isco", "Alarcón", "Spain", 2 },
                    { 117, 13, new DateTime(1996, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pablo", "Fornals", "Spain", 2 },
                    { 118, 13, new DateTime(1996, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Giovani", "Lo Celso", "Argentina", 2 },
                    { 119, 13, new DateTime(2000, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Antony", "Santos", "Brazil", 3 },
                    { 120, 13, new DateTime(1999, 4, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cucho", "Hernández", "Colombia", 3 },
                    { 121, 13, new DateTime(2001, 12, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Abde", "Ezzalzouli", "Morocco", 3 },
                    { 122, 14, new DateTime(1997, 6, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unai", "Simón", "Spain", 0 },
                    { 123, 14, new DateTime(1999, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dani", "Vivian", "Spain", 1 },
                    { 124, 14, new DateTime(1995, 1, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Yeray", "Álvarez", "Spain", 1 },
                    { 125, 14, new DateTime(1994, 5, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Aymeric", "Laporte", "Spain", 1 },
                    { 126, 14, new DateTime(2000, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Oihan", "Sancet", "Spain", 2 },
                    { 127, 14, new DateTime(1994, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Iñaki", "Williams", "Ghana", 3 },
                    { 128, 14, new DateTime(2002, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nico", "Williams", "Spain", 3 },
                    { 129, 14, new DateTime(1995, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Álex", "Berenguer", "Spain", 3 },
                    { 130, 14, new DateTime(1996, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gorka", "Guruzeta", "Spain", 3 },
                    { 131, 15, new DateTime(1995, 3, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Álex", "Remiro", "Spain", 0 },
                    { 132, 15, new DateTime(1997, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Igor", "Zubeldia", "Spain", 1 },
                    { 133, 15, new DateTime(1994, 3, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Aritz", "Elustondo", "Spain", 1 },
                    { 134, 15, new DateTime(1997, 1, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Brais", "Méndez", "Spain", 2 },
                    { 135, 15, new DateTime(2001, 6, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Takefusa", "Kubo", "Japan", 3 },
                    { 136, 15, new DateTime(1997, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mikel", "Oyarzabal", "Spain", 3 },
                    { 137, 15, new DateTime(2001, 12, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ander", "Barrenetxea", "Spain", 3 },
                    { 138, 15, new DateTime(2002, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Luka", "Sučić", "Croatia", 2 },
                    { 139, 16, new DateTime(1991, 11, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nemanja", "Gudelj", "Serbia", 2 },
                    { 140, 16, new DateTime(1989, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "César", "Azpilicueta", "Spain", 1 },
                    { 141, 16, new DateTime(2002, 6, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tanguy", "Nianzou", "France", 1 },
                    { 142, 16, new DateTime(1997, 8, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gabriel", "Suazo", "Chile", 1 },
                    { 143, 16, new DateTime(1997, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Djibril", "Sow", "Switzerland", 2 },
                    { 144, 16, new DateTime(1998, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rubén", "Vargas", "Switzerland", 3 },
                    { 145, 16, new DateTime(2000, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Isaac", "Romero", "Spain", 3 },
                    { 146, 16, new DateTime(1988, 12, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alexis", "Sánchez", "Chile", 3 },
                    { 147, 17, new DateTime(1988, 12, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Yann", "Sommer", "Switzerland", 0 },
                    { 148, 17, new DateTime(1999, 4, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alessandro", "Bastoni", "Italy", 1 },
                    { 149, 17, new DateTime(1997, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Federico", "Dimarco", "Italy", 1 },
                    { 150, 17, new DateTime(1996, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Denzel", "Dumfries", "Netherlands", 1 },
                    { 151, 17, new DateTime(1997, 2, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nicolò", "Barella", "Italy", 2 },
                    { 152, 17, new DateTime(1994, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hakan", "Çalhanoğlu", "Turkey", 2 },
                    { 153, 17, new DateTime(1997, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lautaro", "Martínez", "Argentina", 3 },
                    { 154, 17, new DateTime(1997, 8, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Marcus", "Thuram", "France", 3 },
                    { 155, 17, new DateTime(2005, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Francesco Pio", "Esposito", "Italy", 3 },
                    { 156, 18, new DateTime(1995, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mike", "Maignan", "France", 0 },
                    { 157, 18, new DateTime(1997, 12, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fikayo", "Tomori", "England", 1 },
                    { 158, 18, new DateTime(2001, 5, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Strahinja", "Pavlović", "Serbia", 1 },
                    { 159, 18, new DateTime(1999, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Youssouf", "Fofana", "France", 2 },
                    { 160, 18, new DateTime(1985, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Luka", "Modrić", "Croatia", 2 },
                    { 161, 18, new DateTime(1995, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Adrien", "Rabiot", "France", 2 },
                    { 162, 18, new DateTime(1998, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Christian", "Pulisic", "United States", 3 },
                    { 163, 18, new DateTime(1999, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rafael", "Leão", "Portugal", 3 },
                    { 164, 18, new DateTime(2001, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Santiago", "Giménez", "Mexico", 3 },
                    { 165, 19, new DateTime(1997, 7, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Michele", "Di Gregorio", "Italy", 0 },
                    { 166, 19, new DateTime(1997, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gleison", "Bremer", "Brazil", 1 },
                    { 167, 19, new DateTime(1998, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Federico", "Gatti", "Italy", 1 },
                    { 168, 19, new DateTime(2000, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Andrea", "Cambiaso", "Italy", 1 },
                    { 169, 19, new DateTime(1998, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Manuel", "Locatelli", "Italy", 2 },
                    { 170, 19, new DateTime(2001, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khéphren", "Thuram", "France", 2 },
                    { 171, 19, new DateTime(2005, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kenan", "Yıldız", "Turkey", 3 },
                    { 172, 19, new DateTime(2000, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dušan", "Vlahović", "Serbia", 3 },
                    { 173, 19, new DateTime(2000, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jonathan", "David", "Canada", 3 },
                    { 174, 20, new DateTime(1997, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alex", "Meret", "Italy", 0 },
                    { 175, 20, new DateTime(1993, 8, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Giovanni", "Di Lorenzo", "Italy", 1 },
                    { 176, 20, new DateTime(1994, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Amir", "Rrahmani", "Kosovo", 1 },
                    { 177, 20, new DateTime(1999, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alessandro", "Buongiorno", "Italy", 1 },
                    { 178, 20, new DateTime(1994, 11, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Stanislav", "Lobotka", "Slovakia", 2 },
                    { 179, 20, new DateTime(1996, 12, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Scott", "McTominay", "Scotland", 2 },
                    { 180, 20, new DateTime(1991, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kevin", "De Bruyne", "Belgium", 2 },
                    { 181, 20, new DateTime(1993, 5, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Romelu", "Lukaku", "Belgium", 3 },
                    { 182, 20, new DateTime(1993, 8, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Matteo", "Politano", "Italy", 3 },
                    { 183, 21, new DateTime(1999, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mile", "Svilar", "Serbia", 0 },
                    { 184, 21, new DateTime(1996, 4, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gianluca", "Mancini", "Italy", 1 },
                    { 185, 21, new DateTime(1999, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Evan", "Ndicka", "Ivory Coast", 1 },
                    { 186, 21, new DateTime(1995, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bryan", "Cristante", "Italy", 2 },
                    { 187, 21, new DateTime(2001, 5, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Manu", "Koné", "France", 2 },
                    { 188, 21, new DateTime(1996, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lorenzo", "Pellegrini", "Italy", 2 },
                    { 189, 21, new DateTime(1993, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Paulo", "Dybala", "Argentina", 3 },
                    { 190, 21, new DateTime(2003, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Matías", "Soulé", "Argentina", 3 },
                    { 191, 21, new DateTime(1997, 6, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Artem", "Dovbyk", "Ukraine", 3 },
                    { 192, 22, new DateTime(2000, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Marco", "Carnesecchi", "Italy", 0 },
                    { 193, 22, new DateTime(1999, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Isak", "Hien", "Sweden", 1 },
                    { 194, 22, new DateTime(1993, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Berat", "Djimsiti", "Albania", 1 },
                    { 195, 22, new DateTime(2000, 5, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Raoul", "Bellanova", "Italy", 1 },
                    { 196, 22, new DateTime(1991, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Marten", "de Roon", "Netherlands", 2 },
                    { 197, 22, new DateTime(1999, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Éderson", "dos Santos", "Brazil", 2 },
                    { 198, 22, new DateTime(2001, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Charles", "De Ketelaere", "Belgium", 2 },
                    { 199, 22, new DateTime(1999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gianluca", "Scamacca", "Italy", 3 },
                    { 200, 22, new DateTime(2000, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nikola", "Krstović", "Montenegro", 3 },
                    { 201, 23, new DateTime(1994, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ivan", "Provedel", "Italy", 0 },
                    { 202, 23, new DateTime(1995, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alessio", "Romagnoli", "Italy", 1 },
                    { 203, 23, new DateTime(2000, 8, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mario", "Gila", "Spain", 1 },
                    { 204, 23, new DateTime(2000, 1, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nuno", "Tavares", "Portugal", 1 },
                    { 205, 23, new DateTime(2001, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nicolò", "Rovella", "Italy", 2 },
                    { 206, 23, new DateTime(1995, 6, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mattia", "Zaccagni", "Italy", 3 },
                    { 207, 23, new DateTime(1987, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pedro", "Rodríguez", "Spain", 3 },
                    { 208, 23, new DateTime(1998, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Valentín", "Castellanos", "Argentina", 3 },
                    { 209, 23, new DateTime(2001, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gustav", "Isaksen", "Denmark", 3 },
                    { 210, 24, new DateTime(1990, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "David", "de Gea", "Spain", 0 },
                    { 211, 24, new DateTime(1998, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dodô", "Silva", "Brazil", 1 },
                    { 212, 24, new DateTime(1994, 7, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Daniele", "Rugani", "Italy", 1 },
                    { 213, 24, new DateTime(1994, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Robin", "Gosens", "Germany", 1 },
                    { 214, 24, new DateTime(1997, 6, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rolando", "Mandragora", "Italy", 2 },
                    { 215, 24, new DateTime(2001, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nicolò", "Fagioli", "Italy", 2 },
                    { 216, 24, new DateTime(2000, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Moise", "Kean", "Italy", 3 },
                    { 217, 24, new DateTime(1997, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Albert", "Guðmundsson", "Iceland", 3 },
                    { 218, 24, new DateTime(2001, 1, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Roberto", "Piccoli", "Italy", 3 },
                    { 219, 25, new DateTime(1986, 3, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Manuel", "Neuer", "Germany", 0 },
                    { 220, 25, new DateTime(1998, 10, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dayot", "Upamecano", "France", 1 },
                    { 221, 25, new DateTime(1996, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jonathan", "Tah", "Germany", 1 },
                    { 222, 25, new DateTime(2000, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alphonso", "Davies", "Canada", 1 },
                    { 223, 25, new DateTime(1995, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Joshua", "Kimmich", "Germany", 2 },
                    { 224, 25, new DateTime(1995, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Leon", "Goretzka", "Germany", 2 },
                    { 225, 25, new DateTime(2003, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jamal", "Musiala", "Germany", 2 },
                    { 226, 25, new DateTime(2001, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Michael", "Olise", "France", 3 },
                    { 227, 25, new DateTime(1993, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Harry", "Kane", "England", 3 },
                    { 228, 25, new DateTime(1997, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Luis", "Díaz", "Colombia", 3 },
                    { 229, 26, new DateTime(1997, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gregor", "Kobel", "Switzerland", 0 },
                    { 230, 26, new DateTime(1999, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nico", "Schlotterbeck", "Germany", 1 },
                    { 231, 26, new DateTime(1995, 9, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Niklas", "Süle", "Germany", 1 },
                    { 232, 26, new DateTime(1996, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Waldemar", "Anton", "Germany", 1 },
                    { 233, 26, new DateTime(1994, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Emre", "Can", "Germany", 2 },
                    { 234, 26, new DateTime(1996, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Julian", "Brandt", "Germany", 2 },
                    { 235, 26, new DateTime(2005, 9, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jobe", "Bellingham", "England", 2 },
                    { 236, 26, new DateTime(2002, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Karim", "Adeyemi", "Germany", 3 },
                    { 237, 26, new DateTime(1996, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Serhou", "Guirassy", "Guinea", 3 },
                    { 238, 26, new DateTime(2002, 10, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Maximilian", "Beier", "Germany", 3 },
                    { 239, 27, new DateTime(1990, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Péter", "Gulácsi", "Hungary", 0 },
                    { 240, 27, new DateTime(1992, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Willi", "Orbán", "Hungary", 1 },
                    { 241, 27, new DateTime(2002, 12, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Castello", "Lukeba", "France", 1 },
                    { 242, 27, new DateTime(1998, 4, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "David", "Raum", "Germany", 1 },
                    { 243, 27, new DateTime(1997, 9, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Xaver", "Schlager", "Austria", 2 },
                    { 244, 27, new DateTime(1999, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Christoph", "Baumgartner", "Austria", 2 },
                    { 245, 27, new DateTime(2001, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nicolas", "Seiwald", "Austria", 2 },
                    { 246, 27, new DateTime(2005, 4, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Antonio", "Nusa", "Norway", 3 },
                    { 247, 27, new DateTime(2003, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Johan", "Bakayoko", "Belgium", 3 },
                    { 248, 27, new DateTime(2005, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Conrad", "Harder", "Denmark", 3 },
                    { 249, 28, new DateTime(1993, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mark", "Flekken", "Netherlands", 0 },
                    { 250, 28, new DateTime(1999, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Edmond", "Tapsoba", "Burkina Faso", 1 },
                    { 251, 28, new DateTime(2000, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Loïc", "Badé", "France", 1 },
                    { 252, 28, new DateTime(1995, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Álex", "Grimaldo", "Spain", 1 },
                    { 253, 28, new DateTime(1994, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Robert", "Andrich", "Germany", 2 },
                    { 254, 28, new DateTime(1997, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Aleix", "García", "Spain", 2 },
                    { 255, 28, new DateTime(1998, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Exequiel", "Palacios", "Argentina", 2 },
                    { 256, 28, new DateTime(2002, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Malik", "Tillman", "United States", 2 },
                    { 257, 28, new DateTime(1996, 1, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Patrik", "Schick", "Czech Republic", 3 },
                    { 258, 28, new DateTime(1997, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Martin", "Terrier", "France", 3 },
                    { 259, 29, new DateTime(1996, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alexander", "Nübel", "Germany", 0 },
                    { 260, 29, new DateTime(1998, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jeff", "Chabot", "Germany", 1 },
                    { 261, 29, new DateTime(1997, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Maximilian", "Mittelstädt", "Germany", 1 },
                    { 262, 29, new DateTime(2001, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Angelo", "Stiller", "Germany", 2 },
                    { 263, 29, new DateTime(1998, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chris", "Führich", "Germany", 2 },
                    { 264, 29, new DateTime(1996, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Atakan", "Karazor", "Turkey", 2 },
                    { 265, 29, new DateTime(2004, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bilal", "El Khannouss", "Morocco", 2 },
                    { 266, 29, new DateTime(1996, 7, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Deniz", "Undav", "Germany", 3 },
                    { 267, 29, new DateTime(1998, 3, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ermedin", "Demirović", "Bosnia and Herzegovina", 3 },
                    { 268, 29, new DateTime(2001, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jamie", "Leweling", "Germany", 3 },
                    { 269, 30, new DateTime(1995, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Michael", "Zetterer", "Germany", 0 },
                    { 270, 30, new DateTime(1996, 7, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Robin", "Koch", "Germany", 1 },
                    { 271, 30, new DateTime(2000, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Arthur", "Theate", "Belgium", 1 },
                    { 272, 30, new DateTime(1997, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rasmus", "Kristensen", "Denmark", 1 },
                    { 273, 30, new DateTime(1995, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ellyes", "Skhiri", "Tunisia", 2 },
                    { 274, 30, new DateTime(2004, 6, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hugo", "Larsson", "Sweden", 2 },
                    { 275, 30, new DateTime(1992, 6, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mario", "Götze", "Germany", 2 },
                    { 276, 30, new DateTime(1998, 6, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ritsu", "Dōan", "Japan", 3 },
                    { 277, 30, new DateTime(2000, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jonathan", "Burkardt", "Germany", 3 },
                    { 278, 30, new DateTime(2002, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ansgar", "Knauff", "Germany", 3 },
                    { 279, 31, new DateTime(1995, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tim", "Kleindienst", "Germany", 3 },
                    { 280, 31, new DateTime(1996, 8, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Franck", "Honorat", "France", 3 },
                    { 281, 31, new DateTime(1997, 3, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Florian", "Neuhaus", "Germany", 2 },
                    { 282, 31, new DateTime(2002, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rocco", "Reitz", "Germany", 2 },
                    { 283, 31, new DateTime(1996, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nico", "Elvedi", "Switzerland", 1 },
                    { 284, 31, new DateTime(1993, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kevin", "Stöger", "Austria", 2 },
                    { 285, 31, new DateTime(2002, 11, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Giovanni", "Reyna", "United States", 2 },
                    { 286, 31, new DateTime(2002, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Joe", "Scally", "United States", 1 },
                    { 287, 32, new DateTime(2001, 11, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lucas", "Chevalier", "France", 0 },
                    { 288, 32, new DateTime(1998, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Achraf", "Hakimi", "Morocco", 1 },
                    { 289, 32, new DateTime(1994, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Marquinhos", "Corrêa", "Brazil", 1 },
                    { 290, 32, new DateTime(2002, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nuno", "Mendes", "Portugal", 1 },
                    { 291, 32, new DateTime(2000, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vitinha", "Ferreira", "Portugal", 2 },
                    { 292, 32, new DateTime(2004, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "João", "Neves", "Portugal", 2 },
                    { 293, 32, new DateTime(1996, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fabián", "Ruiz", "Spain", 2 },
                    { 294, 32, new DateTime(1997, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ousmane", "Dembélé", "France", 3 },
                    { 295, 32, new DateTime(2001, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khvicha", "Kvaratskhelia", "Georgia", 3 },
                    { 296, 32, new DateTime(2002, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bradley", "Barcola", "France", 3 },
                    { 297, 32, new DateTime(2005, 6, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Désiré", "Doué", "France", 3 },
                    { 298, 33, new DateTime(1992, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gerónimo", "Rulli", "Argentina", 0 },
                    { 299, 33, new DateTime(1999, 1, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Leonardo", "Balerdi", "Argentina", 1 },
                    { 300, 33, new DateTime(1996, 3, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Benjamin", "Pavard", "France", 1 },
                    { 301, 33, new DateTime(1996, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nayef", "Aguerd", "Morocco", 1 },
                    { 302, 33, new DateTime(1995, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pierre-Emile", "Højbjerg", "Denmark", 2 },
                    { 303, 33, new DateTime(1993, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Geoffrey", "Kondogbia", "Central African Republic", 2 },
                    { 304, 33, new DateTime(2001, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mason", "Greenwood", "England", 3 },
                    { 305, 33, new DateTime(2000, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Amine", "Gouiri", "Algeria", 3 },
                    { 306, 33, new DateTime(1989, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pierre-Emerick", "Aubameyang", "Gabon", 3 },
                    { 307, 34, new DateTime(1998, 4, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Philipp", "Köhn", "Switzerland", 0 },
                    { 308, 34, new DateTime(1994, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Eric", "Dier", "England", 1 },
                    { 309, 34, new DateTime(1996, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Thilo", "Kehrer", "Germany", 1 },
                    { 310, 34, new DateTime(2001, 6, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vanderson", "de Oliveira", "Brazil", 1 },
                    { 311, 34, new DateTime(1996, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Denis", "Zakaria", "Switzerland", 2 },
                    { 312, 34, new DateTime(1993, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Paul", "Pogba", "France", 2 },
                    { 313, 34, new DateTime(1996, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Aleksandr", "Golovin", "Russia", 2 },
                    { 314, 34, new DateTime(2002, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Maghnes", "Akliouche", "France", 2 },
                    { 315, 34, new DateTime(2001, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Folarin", "Balogun", "United States", 3 },
                    { 316, 35, new DateTime(1997, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dominik", "Greif", "Slovakia", 0 },
                    { 317, 35, new DateTime(1992, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nicolás", "Tagliafico", "Argentina", 1 },
                    { 318, 35, new DateTime(1996, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Moussa", "Niakhaté", "Senegal", 1 },
                    { 319, 35, new DateTime(1992, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Clinton", "Mata", "Angola", 1 },
                    { 320, 35, new DateTime(1994, 8, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Corentin", "Tolisso", "France", 2 },
                    { 321, 35, new DateTime(1998, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Orel", "Mangala", "Belgium", 2 },
                    { 322, 35, new DateTime(2000, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Georges", "Mikautadze", "Georgia", 3 },
                    { 323, 35, new DateTime(2005, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Malick", "Fofana", "Belgium", 3 },
                    { 324, 35, new DateTime(2006, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Endrick", "Felipe", "Brazil", 3 },
                    { 325, 36, new DateTime(2000, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Berke", "Özer", "Turkey", 0 },
                    { 326, 36, new DateTime(1991, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Aïssa", "Mandi", "Algeria", 1 },
                    { 327, 36, new DateTime(1991, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Thomas", "Meunier", "Belgium", 1 },
                    { 328, 36, new DateTime(1994, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chancel", "Mbemba", "DR Congo", 1 },
                    { 329, 36, new DateTime(1990, 8, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Benjamin", "André", "France", 2 },
                    { 330, 36, new DateTime(1994, 11, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nabil", "Bentaleb", "Algeria", 2 },
                    { 331, 36, new DateTime(2003, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hákon", "Haraldsson", "Iceland", 2 },
                    { 332, 36, new DateTime(1986, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Olivier", "Giroud", "France", 3 },
                    { 333, 36, new DateTime(2007, 12, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ethan", "Mbappé", "France", 2 },
                    { 334, 37, new DateTime(1990, 10, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Florian", "Sotoca", "France", 3 },
                    { 335, 37, new DateTime(1993, 1, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Florian", "Thauvin", "France", 3 },
                    { 336, 37, new DateTime(1997, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Allan", "Saint-Maximin", "France", 3 },
                    { 337, 37, new DateTime(1998, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Odsonne", "Édouard", "France", 3 },
                    { 338, 37, new DateTime(1998, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Amadou", "Haidara", "Mali", 2 },
                    { 339, 37, new DateTime(1992, 11, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jonathan", "Gradit", "France", 1 },
                    { 340, 37, new DateTime(1993, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ruben", "Aguilar", "France", 1 },
                    { 341, 37, new DateTime(2001, 6, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Abdallah", "Sima", "Senegal", 3 }
                });

            migrationBuilder.InsertData(
                table: "Ratings",
                columns: new[] { "Id", "Comment", "MatchId", "PlayerId", "Score", "UserId" },
                values: new object[,]
                {
                    { 1, "Unplayable on the right — two goals and a constant threat.", 5, 7, 9, 1 },
                    { 2, "Pulled the strings and capped it with a classy assist.", 5, 5, 8, 3 },
                    { 3, "Powerful display through the middle, took his goal well.", 5, 9, 8, 2 },
                    { 4, "A few bright moments but couldn't drag Spurs back in.", 5, 72, 5, 4 },
                    { 5, "Bossed the midfield battle against City.", 1, 4, 8, 5 },
                    { 6, "Took his goal but starved of service otherwise.", 1, 18, 7, 1 },
                    { 7, "Marshalled Haaland superbly all afternoon.", 1, 2, 8, 6 },
                    { 8, "Decisive brace — back to his ruthless best.", 2, 18, 9, 7 },
                    { 9, "Drove City forward relentlessly.", 2, 15, 8, 2 },
                    { 10, "Scored but was largely kept quiet.", 2, 7, 7, 8 },
                    { 11, "Two goals at Anfield — utterly ruthless.", 3, 26, 9, 1 },
                    { 12, "Ran the show for United from midfield.", 3, 23, 8, 2 },
                    { 13, "Scored but lacked support up top.", 3, 46, 7, 3 },
                    { 14, "Strong hold-up play crowned with a goal.", 4, 28, 8, 4 },
                    { 15, "Tireless running, thoroughly deserved his goal.", 4, 27, 8, 5 },
                    { 16, "An off night for the skipper.", 4, 39, 5, 6 },
                    { 17, "Hat-trick hero — simply unstoppable.", 7, 18, 10, 9 },
                    { 18, "Terrorised the full-back all game.", 7, 19, 8, 10 },
                    { 19, "His pace cut Newcastle open repeatedly.", 8, 8, 8, 1 },
                    { 20, "Worked hard but little end product.", 8, 63, 6, 2 },
                    { 21, "Match-winning goal and led the line superbly.", 9, 36, 9, 3 },
                    { 22, "A wall in goal — kept City out single-handedly.", 9, 29, 9, 4 },
                    { 23, "Decisive brace in the Clásico.", 10, 93, 9, 5 },
                    { 24, "Bossed the midfield in the biggest game.", 10, 91, 8, 6 },
                    { 25, "Scored a beauty to keep Barça in it.", 10, 83, 8, 7 },
                    { 26, "Clinical finish and led the press.", 11, 84, 8, 8 },
                    { 27, "Conducted the win — flawless tempo.", 11, 79, 9, 9 },
                    { 28, "Frustrating night, well shackled by the back line.", 11, 94, 5, 10 },
                    { 29, "Two goals and relentless from the left.", 12, 82, 9, 1 },
                    { 30, "Battled hard and took his goal.", 12, 104, 7, 2 },
                    { 31, "Stole the derby with a stunning strike.", 13, 119, 9, 3 },
                    { 32, "A masterclass in the number ten role.", 13, 116, 8, 4 },
                    { 33, "Captain's brace in a thrilling Basque derby.", 14, 136, 9, 5 },
                    { 34, "Lively throughout and scored a fine goal.", 14, 128, 7, 6 },
                    { 35, "A wonderful solo goal sealed it.", 15, 83, 8, 7 },
                    { 36, "Couldn't contain Barça's movement.", 15, 107, 5, 8 },
                    { 37, "Won the derby with a brilliant solo goal.", 16, 163, 9, 9 },
                    { 38, "Controlled the tempo with ease at his age.", 16, 160, 8, 10 },
                    { 39, "Quiet by his lofty standards.", 16, 153, 5, 1 },
                    { 40, "Sensational in a seven-goal thriller.", 18, 171, 9, 2 },
                    { 41, "Scored twice but ended on the losing side.", 18, 153, 8, 3 },
                    { 42, "Pulled the strings with two assists.", 19, 180, 9, 4 },
                    { 43, "Bullied his former club all night.", 19, 181, 8, 5 },
                    { 44, "Derby della Capitale hero.", 21, 189, 9, 6 },
                    { 45, "Direct and dangerous every time he ran.", 21, 190, 8, 7 },
                    { 46, "Two assists and ran the midfield.", 22, 152, 8, 8 },
                    { 47, "A relentless box-to-box engine.", 22, 179, 8, 9 },
                    { 48, "Decisive brace in Der Klassiker.", 24, 237, 9, 10 },
                    { 49, "Scored but Bayern fell short.", 24, 227, 7, 1 },
                    { 50, "Hat-trick in a ruthless rout.", 25, 227, 9, 2 },
                    { 51, "Three assists — utterly unplayable.", 25, 226, 9, 3 },
                    { 52, "Dazzling on the ball throughout.", 25, 225, 8, 4 },
                    { 53, "His pace destroyed Leipzig.", 28, 236, 9, 5 },
                    { 54, "Two more goals for the in-form striker.", 28, 237, 8, 6 },
                    { 55, "Ballon d'Or form — a brace in Le Classique.", 29, 294, 9, 7 },
                    { 56, "Unstoppable: a goal and two assists.", 29, 295, 9, 8 },
                    { 57, "Electric pace down the flank.", 29, 296, 8, 9 },
                    { 58, "The winner against the champions.", 30, 306, 9, 10 },
                    { 59, "Immense in midfield, never gave the ball away.", 30, 302, 8, 1 },
                    { 60, "A clinical hat-trick at the Parc des Princes.", 31, 315, 9, 2 },
                    { 61, "Turned back the clock with a commanding display.", 31, 312, 8, 3 },
                    { 62, "Two goals in a five-goal thriller.", 32, 304, 9, 4 },
                    { 63, "A brace for Lyon in a narrow defeat.", 32, 322, 8, 5 },
                    { 64, "The match-winner at the Parc — a star turn.", 34, 324, 9, 6 },
                    { 65, "Tried everything to rescue PSG.", 34, 291, 7, 7 }
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
