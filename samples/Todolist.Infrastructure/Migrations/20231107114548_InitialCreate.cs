using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Todolist.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TodoList",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Colour_Code = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TodoListItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Priority = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    TodoListId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoListItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodoListItem_TodoList_TodoListId",
                        column: x => x.TodoListId,
                        principalTable: "TodoList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TodoList",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"), null, null, false, "Todo List", null, null });

            migrationBuilder.InsertData(
                table: "TodoListItem",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "Note", "Order", "Priority", "TodoListId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("08daa6de-fabb-4e57-86e0-386a66fe8ae7"), null, null, false, "Make a todo list 📃", 1, "Low", new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"), null, null },
                    { new Guid("08daa6df-0599-4227-8e79-ac83b22305f3"), null, null, false, "Check off the first item ✅", 2, "Medium", new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"), null, null },
                    { new Guid("08daa6df-1457-4ff3-8080-a57e71d0d80c"), null, null, false, "Realise you've already done two things on the list! 🤯", 3, "High", new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"), null, null },
                    { new Guid("08daf207-8abf-4fff-830f-93e48ed9a34c"), null, null, false, "Reward yourself with a nice, long nap 🏆", 4, "None", new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"), null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoListItem_TodoListId",
                table: "TodoListItem",
                column: "TodoListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodoListItem");

            migrationBuilder.DropTable(
                name: "TodoList");
        }
    }
}
