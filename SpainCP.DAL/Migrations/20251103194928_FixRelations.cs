using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpainCP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Clubs_Team1ID",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Clubs_Team2ID",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Clubs_ClubID",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_ClubID",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Matches_Team1ID",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_Team2ID",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "ClubID",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "GoalsTeam1",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "GoalsTeam2",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Team1ID",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Team2ID",
                table: "Matches");

            migrationBuilder.CreateTable(
                name: "ClubMatch",
                columns: table => new
                {
                    ClubsID = table.Column<int>(type: "int", nullable: false),
                    MatchesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubMatch", x => new { x.ClubsID, x.MatchesID });
                    table.ForeignKey(
                        name: "FK_ClubMatch_Clubs_ClubsID",
                        column: x => x.ClubsID,
                        principalTable: "Clubs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClubMatch_Matches_MatchesID",
                        column: x => x.MatchesID,
                        principalTable: "Matches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClubPlayer",
                columns: table => new
                {
                    ClubsID = table.Column<int>(type: "int", nullable: false),
                    PlayersID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubPlayer", x => new { x.ClubsID, x.PlayersID });
                    table.ForeignKey(
                        name: "FK_ClubPlayer_Clubs_ClubsID",
                        column: x => x.ClubsID,
                        principalTable: "Clubs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClubPlayer_Players_PlayersID",
                        column: x => x.PlayersID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClubMatch_MatchesID",
                table: "ClubMatch",
                column: "MatchesID");

            migrationBuilder.CreateIndex(
                name: "IX_ClubPlayer_PlayersID",
                table: "ClubPlayer",
                column: "PlayersID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubMatch");

            migrationBuilder.DropTable(
                name: "ClubPlayer");

            migrationBuilder.AddColumn<int>(
                name: "ClubID",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoalsTeam1",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoalsTeam2",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Team1ID",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Team2ID",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Players_ClubID",
                table: "Players",
                column: "ClubID");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Team1ID",
                table: "Matches",
                column: "Team1ID");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Team2ID",
                table: "Matches",
                column: "Team2ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Clubs_Team1ID",
                table: "Matches",
                column: "Team1ID",
                principalTable: "Clubs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Clubs_Team2ID",
                table: "Matches",
                column: "Team2ID",
                principalTable: "Clubs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Clubs_ClubID",
                table: "Players",
                column: "ClubID",
                principalTable: "Clubs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
