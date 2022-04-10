using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentaku.Infrastructure.Data.Migraions
{
    public partial class VotingManager__Add_CreatedOn__Rename_IsDisabled_DisabledOn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoteSessions_VotingManagers_VotingManagerId",
                table: "VoteSessions");

            migrationBuilder.RenameColumn(
                name: "IsDisabled",
                table: "VotingManagers",
                newName: "IsArchived");

            migrationBuilder.RenameColumn(
                name: "DisabledOn",
                table: "VotingManagers",
                newName: "ArchivedOn");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "VotingManagers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<Guid>(
                name: "VotingManagerId",
                table: "VoteSessions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_VoteSessions_VotingManagers_VotingManagerId",
                table: "VoteSessions",
                column: "VotingManagerId",
                principalTable: "VotingManagers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoteSessions_VotingManagers_VotingManagerId",
                table: "VoteSessions");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "VotingManagers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "IsArchived",
                table: "VotingManagers",
                newName: "IsDisabled");

            migrationBuilder.RenameColumn(
                name: "ArchivedOn",
                table: "VotingManagers",
                newName: "DisabledOn");

            migrationBuilder.AlterColumn<Guid>(
                name: "VotingManagerId",
                table: "VoteSessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VoteSessions_VotingManagers_VotingManagerId",
                table: "VoteSessions",
                column: "VotingManagerId",
                principalTable: "VotingManagers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
