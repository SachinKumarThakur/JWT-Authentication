﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace OrgDataLayer.Migrations
{
    public partial class initialcraft2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Employee",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Department",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Department");
        }
    }
}
