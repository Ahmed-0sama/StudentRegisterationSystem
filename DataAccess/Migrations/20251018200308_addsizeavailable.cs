using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addsizeavailable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FacultyId",
                table: "Doctors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AvailableSeats",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_FacultyId",
                table: "Doctors",
                column: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Faculties_FacultyId",
                table: "Doctors",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Faculties_FacultyId",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_FacultyId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "FacultyId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "AvailableSeats",
                table: "Classes");
        }
    }
}
