using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentaku.Infrastructure.Data.Migraions
{
    public partial class Add_JoinedVotersSessions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.CreateTable(
                name: "JoinedVotersSessions",
                columns: table => new
                {
                    VoterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoteSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsVoted = table.Column<bool>(type: "bit", nullable: false),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinedVotersSessions", x => new { x.VoterId, x.VoteSessionId });
                    table.ForeignKey(
                        name: "FK_JoinedVotersSessions_Voters_VoterId",
                        column: x => x.VoterId,
                        principalTable: "Voters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinedVotersSessions_VoteSessions_VoteSessionId",
                        column: x => x.VoteSessionId,
                        principalTable: "VoteSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

          migrationBuilder.Sql(@"
INSERT INTO JoinedVotersSessions(VoteSessionId, VoterId, IsVoted, IsFavorite) 
SELECT SessionsId as VoteSessionId, VotersId as VoterId, 0, 0
FROM VoteSessionVoter
");
          
          migrationBuilder.DropTable(
            name: "VoteSessionVoter");
          
            migrationBuilder.CreateIndex(
                name: "IX_JoinedVotersSessions_VoteSessionId",
                table: "JoinedVotersSessions",
                column: "VoteSessionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
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

            migrationBuilder.Sql(@"
INSERT INTO VoteSessionVoter(SessionsId, VotersId) 
SELECT VoteSessionId as SessionsId, VoterId as VotersId
FROM JoinedVotersSessions
");
            
            migrationBuilder.DropTable(
              name: "JoinedVotersSessions");
            
            migrationBuilder.CreateIndex(
                name: "IX_VoteSessionVoter_VotersId",
                table: "VoteSessionVoter",
                column: "VotersId");
        }
    }
}
