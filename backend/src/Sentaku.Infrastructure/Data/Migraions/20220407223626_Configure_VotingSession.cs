using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentaku.Infrastructure.Data.Migraions
{
    public partial class Configure_VotingSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Agenda",
                table: "VoteSessions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "VoteSessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartDate",
                table: "VoteSessions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "VoteSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Agenda",
                table: "VoteSessions");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "VoteSessions");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "VoteSessions");

            migrationBuilder.DropColumn(
                name: "State",
                table: "VoteSessions");
        }
    }
}
