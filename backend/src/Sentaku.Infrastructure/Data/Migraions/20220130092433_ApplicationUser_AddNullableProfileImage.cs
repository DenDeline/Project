using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentaku.Infrastructure.Data.Migraions
{
  public partial class ApplicationUser_AddNullableProfileImage : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_AspNetUsers_AppFile_ProfileImageId",
          table: "AspNetUsers");

      migrationBuilder.DropIndex(
          name: "IX_AspNetUsers_ProfileImageId",
          table: "AspNetUsers");

      migrationBuilder.AlterColumn<string>(
          name: "ProfileImageId",
          table: "AspNetUsers",
          type: "nvarchar(450)",
          nullable: true,
          oldClrType: typeof(string),
          oldType: "nvarchar(450)");

      migrationBuilder.CreateIndex(
          name: "IX_AspNetUsers_ProfileImageId",
          table: "AspNetUsers",
          column: "ProfileImageId",
          unique: true,
          filter: "[ProfileImageId] IS NOT NULL");

      migrationBuilder.AddForeignKey(
          name: "FK_AspNetUsers_AppFile_ProfileImageId",
          table: "AspNetUsers",
          column: "ProfileImageId",
          principalTable: "AppFile",
          principalColumn: "Id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_AspNetUsers_AppFile_ProfileImageId",
          table: "AspNetUsers");

      migrationBuilder.DropIndex(
          name: "IX_AspNetUsers_ProfileImageId",
          table: "AspNetUsers");

      migrationBuilder.AlterColumn<string>(
          name: "ProfileImageId",
          table: "AspNetUsers",
          type: "nvarchar(450)",
          nullable: false,
          defaultValue: "",
          oldClrType: typeof(string),
          oldType: "nvarchar(450)",
          oldNullable: true);

      migrationBuilder.CreateIndex(
          name: "IX_AspNetUsers_ProfileImageId",
          table: "AspNetUsers",
          column: "ProfileImageId",
          unique: true);

      migrationBuilder.AddForeignKey(
          name: "FK_AspNetUsers_AppFile_ProfileImageId",
          table: "AspNetUsers",
          column: "ProfileImageId",
          principalTable: "AppFile",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);
    }
  }
}
