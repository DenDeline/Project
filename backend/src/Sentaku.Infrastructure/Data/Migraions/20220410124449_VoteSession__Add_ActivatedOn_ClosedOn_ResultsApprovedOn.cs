using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentaku.Infrastructure.Data.Migraions
{
    public partial class VoteSession__Add_ActivatedOn_ClosedOn_ResultsApprovedOn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActivatedOn",
                table: "VoteSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "VoteSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResultsApprovedOn",
                table: "VoteSessions",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivatedOn",
                table: "VoteSessions");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "VoteSessions");

            migrationBuilder.DropColumn(
                name: "ResultsApprovedOn",
                table: "VoteSessions");
        }
    }
}
