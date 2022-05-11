using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCompany.MyProject.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Colour_Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TodoItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Reminder = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TodoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodoItems_Todos_TodoId",
                        column: x => x.TodoId,
                        principalTable: "Todos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_TodoId",
                table: "TodoItems",
                column: "TodoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodoItems");

            migrationBuilder.DropTable(
                name: "Todos");
        }
    }
}
