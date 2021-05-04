using Microsoft.EntityFrameworkCore.Migrations;

namespace EventsAPI.Migrations
{
    public partial class AddingRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventRegistration_Events_EventId",
                table: "EventRegistration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventRegistration",
                table: "EventRegistration");

            migrationBuilder.RenameTable(
                name: "EventRegistration",
                newName: "EventRegistrations");

            migrationBuilder.RenameIndex(
                name: "IX_EventRegistration_EventId",
                table: "EventRegistrations",
                newName: "IX_EventRegistrations_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventRegistrations",
                table: "EventRegistrations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventRegistrations_Events_EventId",
                table: "EventRegistrations",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventRegistrations_Events_EventId",
                table: "EventRegistrations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventRegistrations",
                table: "EventRegistrations");

            migrationBuilder.RenameTable(
                name: "EventRegistrations",
                newName: "EventRegistration");

            migrationBuilder.RenameIndex(
                name: "IX_EventRegistrations_EventId",
                table: "EventRegistration",
                newName: "IX_EventRegistration_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventRegistration",
                table: "EventRegistration",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventRegistration_Events_EventId",
                table: "EventRegistration",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
