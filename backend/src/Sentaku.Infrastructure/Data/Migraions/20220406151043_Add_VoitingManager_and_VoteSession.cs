using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentaku.Infrastructure.Data.Migraions
{
    public partial class Add_VoitingManager_and_VoteSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VotingManagers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    DisabledOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotingManagers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VotingManagers_AspNetUsers_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoteSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VotingManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoteSessions_VotingManagers_VotingManagerId",
                        column: x => x.VotingManagerId,
                        principalTable: "VotingManagers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VoteSessions_VotingManagerId",
                table: "VoteSessions",
                column: "VotingManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_VotingManagers_IdentityId",
                table: "VotingManagers",
                column: "IdentityId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoteSessions");

            migrationBuilder.DropTable(
                name: "VotingManagers");
        }
    }
}
