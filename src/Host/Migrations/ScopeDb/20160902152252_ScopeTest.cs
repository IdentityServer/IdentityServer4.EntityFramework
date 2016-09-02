using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Host.Migrations.ScopeDb
{
    public partial class ScopeTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scopes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllowUnrestrictedIntrospection = table.Column<bool>(nullable: false),
                    ClaimsRule = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    DisplayName = table.Column<string>(maxLength: 200, nullable: true),
                    Emphasize = table.Column<bool>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    IncludeAllClaimsForUser = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Required = table.Column<bool>(nullable: false),
                    ShowInDiscoveryDocument = table.Column<bool>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScopeClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlwaysIncludeInIdToken = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    ScopeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScopeClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScopeClaims_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScopeSecrets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Expiration = table.Column<DateTimeOffset>(nullable: true),
                    ScopeId = table.Column<int>(nullable: true),
                    Type = table.Column<string>(maxLength: 250, nullable: true),
                    Value = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScopeSecrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScopeSecrets_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScopeClaims_ScopeId",
                table: "ScopeClaims",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScopeSecrets_ScopeId",
                table: "ScopeSecrets",
                column: "ScopeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScopeClaims");

            migrationBuilder.DropTable(
                name: "ScopeSecrets");

            migrationBuilder.DropTable(
                name: "Scopes");
        }
    }
}
