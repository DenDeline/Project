using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentaku.Infrastructure.Data.Migraions
{
    public partial class VoteSesions__Add_Questions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionCount",
                table: "VoteSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    VoteSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_VoteSessions_VoteSessionId",
                        column: x => x.VoteSessionId,
                        principalTable: "VoteSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Question_VoteSessionId_Index",
                table: "Question",
                columns: new[] { "VoteSessionId", "Index" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropColumn(
                name: "QuestionCount",
                table: "VoteSessions");
        }
    }
}
