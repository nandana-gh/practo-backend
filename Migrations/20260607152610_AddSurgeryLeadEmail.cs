using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace practo_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddSurgeryLeadEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "SurgeryLeads",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "SurgeryLeads");
        }
    }
}
