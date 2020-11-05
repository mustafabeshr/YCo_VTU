using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Oracle.EntityFrameworkCore.Metadata;

namespace Yvtu.Infra.Identity
{
    public partial class AddUserIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APP_USER",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    USER_NAME = table.Column<string>(maxLength: 256, nullable: true),
                    NORMALIZED_USER_NAME = table.Column<string>(maxLength: 256, nullable: true),
                    EMAIL = table.Column<string>(maxLength: 256, nullable: true),
                    NORMALIZED_EMAIL = table.Column<string>(maxLength: 256, nullable: true),
                    EMAIL_CONFIRMED = table.Column<bool>(nullable: false),
                    PASSWORD_HASH = table.Column<string>(nullable: true),
                    SECURITY_STAMP = table.Column<string>(nullable: true),
                    CONCURRENCY_STAMP = table.Column<string>(nullable: true),
                    PHONE_NUMBER = table.Column<string>(nullable: true),
                    PHONE_NUMBER_CONFIRMED = table.Column<bool>(nullable: false),
                    TWO_FACTOR_ENABLED = table.Column<bool>(nullable: false),
                    LOCKOUT_END = table.Column<DateTimeOffset>(nullable: true),
                    LOCKOUT_ENABLED = table.Column<bool>(nullable: false),
                    ACCESS_FAILED_COUNT = table.Column<int>(nullable: false),
                    DISPLAY_NAME = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_USER", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ROLE",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    NAME = table.Column<string>(maxLength: 256, nullable: true),
                    NORMALIZED_NAME = table.Column<string>(maxLength: 256, nullable: true),
                    CONCURRENCY_STAMP = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROLE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "APP_USER_CLAIMS",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false),
                    USER_ID = table.Column<string>(nullable: false),
                    CLAIM_TYPE = table.Column<string>(nullable: true),
                    CLAIM_VALUE = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_USER_CLAIMS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_APPUSER_CLAIMS_APPUSER_ID",
                        column: x => x.USER_ID,
                        principalTable: "APP_USER",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateSequence("SEQ_APP_USER_CLAIMS_ID", null, 1, 1, 1, null);
            migrationBuilder.Sql("CREATE TRIGGER TR_APP_USER_CLAIMS_BI BEFORE INSERT ON APP_USER_CLAIMS FOR EACH ROW BEGIN :NEW.ID := SEQ_APP_USER_CLAIMS_ID.NextVal; END;");

            migrationBuilder.CreateTable(
                name: "APP_USER_LOGINS",
                columns: table => new
                {
                    LOGIN_PROVIDER = table.Column<string>(nullable: false),
                    PROVIDER_KEY = table.Column<string>(nullable: false),
                    PROVIDER_DISPLAY_NAME = table.Column<string>(nullable: true),
                    USER_ID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_USER_LOGINS", x => new { x.LOGIN_PROVIDER, x.PROVIDER_KEY });
                    table.ForeignKey(
                        name: "FK_APPUSER_LOGINS_APPUSER_ID",
                        column: x => x.USER_ID,
                        principalTable: "APP_USER",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "APP_USER_TOKENS",
                columns: table => new
                {
                    USER_ID = table.Column<string>(nullable: false),
                    LOGIN_PROVIDER = table.Column<string>(nullable: false),
                    NAME = table.Column<string>(nullable: false),
                    VALUE = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_USER_TOKENS", x => new { x.USER_ID, x.LOGIN_PROVIDER, x.NAME });
                    table.ForeignKey(
                        name: "FK_APPUSER_TOKENS_APPUSER_ID",
                        column: x => x.USER_ID,
                        principalTable: "APP_USER",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "APP_ROLE_CLAIMS",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false),
                    ROLE_ID = table.Column<string>(nullable: false),
                    CLAIM_TYPE = table.Column<string>(nullable: true),
                    CLAIM_VALUE = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_ROLE_CLAIMS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_APPROLE_CLAIMS_ROLE_ID",
                        column: x => x.ROLE_ID,
                        principalTable: "ROLE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateSequence("SEQ_APP_ROLE_CLAIMS_ID", null, 1, 1, 1, null);
            migrationBuilder.Sql("CREATE TRIGGER TR_APP_ROLE_CLAIMS_BI BEFORE INSERT ON APP_ROLE_CLAIMS FOR EACH ROW BEGIN :NEW.ID := SEQ_APP_ROLE_CLAIMS_ID.NextVal; END;");

            migrationBuilder.CreateTable(
                name: "APP_USER_ROLES",
                columns: table => new
                {
                    USER_ID = table.Column<string>(nullable: false),
                    ROLE_ID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_USER_ROLES", x => new { x.USER_ID, x.ROLE_ID });
                    table.ForeignKey(
                        name: "FK_APPUSER_ROLES_ROLE_ID",
                        column: x => x.ROLE_ID,
                        principalTable: "ROLE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_APPUSER_ROLES_APPUSER_ID",
                        column: x => x.USER_ID,
                        principalTable: "APP_USER",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_APPROLE_CLAIMS_ROLE_ID",
                table: "APP_ROLE_CLAIMS",
                column: "ROLE_ID");

            migrationBuilder.CreateIndex(
                name: "EMAIL_INDEX",
                table: "APP_USER",
                column: "NORMALIZED_EMAIL");

            migrationBuilder.CreateIndex(
                name: "USER_NAME_INDEX",
                table: "APP_USER",
                column: "NORMALIZED_USER_NAME",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APPUSER_CLAIMS_USER_ID",
                table: "APP_USER_CLAIMS",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_APPUSER_LOGINS_USER_ID",
                table: "APP_USER_LOGINS",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_APPUSER_ROLES_ROLE_ID",
                table: "APP_USER_ROLES",
                column: "ROLE_ID");

            migrationBuilder.CreateIndex(
                name: "ROLE_NAME_INDEX",
                table: "ROLE",
                column: "NORMALIZED_NAME",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence("SEQ_APP_USER_CLAIMS_ID");
            migrationBuilder.DropSequence("SEQ_APP_ROLE_CLAIMS_ID");
            migrationBuilder.DropTable(
                name: "APP_ROLE_CLAIMS");

            migrationBuilder.DropTable(
                name: "APP_USER_CLAIMS");

            migrationBuilder.DropTable(
                name: "APP_USER_LOGINS");

            migrationBuilder.DropTable(
                name: "APP_USER_ROLES");

            migrationBuilder.DropTable(
                name: "APP_USER_TOKENS");

            migrationBuilder.DropTable(
                name: "ROLE");

            migrationBuilder.DropTable(
                name: "APP_USER");
        }
    }
}
