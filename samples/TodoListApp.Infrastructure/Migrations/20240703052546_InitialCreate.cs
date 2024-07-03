using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TodoListApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TodoItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PriorityLevel = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    IsDone = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItems", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TodoItems",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "PriorityLevel", "Text", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("08daa6de-fabb-4e57-86e0-386a66fe8ae7"), null, null, null, null, false, "Low", "Write project proposal 📃", null, null },
                    { new Guid("08daa6df-0599-4227-8e79-ac83b22305f3"), null, null, null, null, false, "Medium", "Schedule kick-off meeting ✅", null, null },
                    { new Guid("08daa6df-1457-4ff3-8080-a57e71d0d80c"), null, null, null, null, false, "High", "Review research results 🤯", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodoItems");
        }
    }
}
