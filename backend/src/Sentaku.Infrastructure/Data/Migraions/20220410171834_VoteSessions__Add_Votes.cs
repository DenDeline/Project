using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentaku.Infrastructure.Data.Migraions
{
    public partial class VoteSessions__Add_Votes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vote",
                columns: table => new
                {
                    VoterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vote", x => new { x.QuestionId, x.VoterId });
                    table.ForeignKey(
                        name: "FK_Vote_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vote_Voters_VoterId",
                        column: x => x.VoterId,
                        principalTable: "Voters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoteSessionVoter",
                columns: table => new
                {
                    SessionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VotersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteSessionVoter", x => new { x.SessionsId, x.VotersId });
                    table.ForeignKey(
                        name: "FK_VoteSessionVoter_Voters_VotersId",
                        column: x => x.VotersId,
                        principalTable: "Voters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoteSessionVoter_VoteSessions_SessionsId",
                        column: x => x.SessionsId,
                        principalTable: "VoteSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vote_VoterId",
                table: "Vote",
                column: "VoterId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteSessionVoter_VotersId",
                table: "VoteSessionVoter",
                column: "VotersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vote");

            migrationBuilder.DropTable(
                name: "VoteSessionVoter");
        }
    }
}
