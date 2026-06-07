using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace practo_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCartAndMedicalRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LabTestOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    ShippingAddress = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RazorpayOrderId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RazorpayPaymentId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTestOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabTestOrders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UploadedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Users_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LabTestOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LabTestOrderId = table.Column<int>(type: "int", nullable: false),
                    DiagnosticTestId = table.Column<int>(type: "int", nullable: true),
                    HealthCheckupPackageId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTestOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabTestOrderItems_DiagnosticTests_DiagnosticTestId",
                        column: x => x.DiagnosticTestId,
                        principalTable: "DiagnosticTests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LabTestOrderItems_HealthCheckupPackages_HealthCheckupPackage~",
                        column: x => x.HealthCheckupPackageId,
                        principalTable: "HealthCheckupPackages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LabTestOrderItems_LabTestOrders_LabTestOrderId",
                        column: x => x.LabTestOrderId,
                        principalTable: "LabTestOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestOrderItems_DiagnosticTestId",
                table: "LabTestOrderItems",
                column: "DiagnosticTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestOrderItems_HealthCheckupPackageId",
                table: "LabTestOrderItems",
                column: "HealthCheckupPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestOrderItems_LabTestOrderId",
                table: "LabTestOrderItems",
                column: "LabTestOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestOrders_UserId",
                table: "LabTestOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_PatientId",
                table: "MedicalRecords",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabTestOrderItems");

            migrationBuilder.DropTable(
                name: "MedicalRecords");

            migrationBuilder.DropTable(
                name: "LabTestOrders");
        }
    }
}
