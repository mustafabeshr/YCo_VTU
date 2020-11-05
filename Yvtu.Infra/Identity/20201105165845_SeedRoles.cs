using Microsoft.EntityFrameworkCore.Migrations;

namespace Yvtu.Infra.Identity
{
    public partial class SeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ROLE",
                columns: new[] { "ID", "CONCURRENCY_STAMP", "NAME", "NORMALIZED_NAME" },
                values: new object[] { "0ef7987e-a507-48e9-87af-75322325027e", "d1a2da16-2bf1-4a8a-b60c-60eef7be32ba", "admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "ROLE",
                columns: new[] { "ID", "CONCURRENCY_STAMP", "NAME", "NORMALIZED_NAME" },
                values: new object[] { "caa90326-6233-43fb-9b3e-908f38bdfbf1", "f3872093-51d9-4306-ba42-351ced4990e9", "user", "USER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ROLE",
                keyColumn: "ID",
                keyValue: "0ef7987e-a507-48e9-87af-75322325027e");

            migrationBuilder.DeleteData(
                table: "ROLE",
                keyColumn: "ID",
                keyValue: "caa90326-6233-43fb-9b3e-908f38bdfbf1");
        }
    }
}
