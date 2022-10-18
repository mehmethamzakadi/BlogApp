using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Persistence.Migrations
{
    public partial class BaseEntityEdit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "PostImages",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PostImages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "PostImages",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "PostImages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "PostCategories",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PostCategories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "PostCategories",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "PostCategories",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "PostImages");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PostImages");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "PostImages");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "PostImages");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "PostCategories");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PostCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "PostCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "PostCategories");
        }
    }
}
