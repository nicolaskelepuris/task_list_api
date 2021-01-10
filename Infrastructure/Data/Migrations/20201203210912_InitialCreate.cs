using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vessels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Imo = table.Column<string>(nullable: true),
                    Flag = table.Column<string>(nullable: true),
                    Deadweight = table.Column<double>(nullable: false),
                    LengthOverall = table.Column<double>(nullable: false),
                    Beam = table.Column<double>(nullable: false),
                    Depth = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vessels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(nullable: false),
                    VesselId = table.Column<int>(nullable: false),
                    DuvNumber = table.Column<string>(nullable: true),
                    ScheduleNumber = table.Column<string>(nullable: true),
                    VoyageNumber = table.Column<string>(nullable: true),
                    NextPorts = table.Column<string>(nullable: true),
                    OperationType = table.Column<int>(nullable: false),
                    Cargo = table.Column<string>(nullable: true),
                    Port = table.Column<int>(nullable: false),
                    HasCrewChange = table.Column<bool>(nullable: false),
                    OnSigners = table.Column<int>(nullable: false),
                    OffSigners = table.Column<int>(nullable: false),
                    EstimatedTimeOfArrivalOnFirstBrazillianPort = table.Column<DateTime>(nullable: false),
                    EstimatedTimeOfArrival = table.Column<DateTime>(nullable: false),
                    EstimatedTimeOfBerthing = table.Column<DateTime>(nullable: false),
                    EstimatedTimeOfSailing = table.Column<DateTime>(nullable: false),
                    Arrival = table.Column<DateTime>(nullable: false),
                    Berthing = table.Column<DateTime>(nullable: false),
                    Sailing = table.Column<DateTime>(nullable: false),
                    Done = table.Column<bool>(nullable: false),
                    Cancelled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Vessels_VesselId",
                        column: x => x.VesselId,
                        principalTable: "Vessels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Deadline = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    WhenToComplete = table.Column<int>(nullable: false),
                    AppointmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_VesselId",
                table: "Appointments",
                column: "VesselId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AppointmentId",
                table: "Tasks",
                column: "AppointmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Vessels");
        }
    }
}
