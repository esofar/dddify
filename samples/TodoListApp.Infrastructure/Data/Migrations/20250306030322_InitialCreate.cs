using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TodoListApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", maxLength: 20, nullable: false),
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
                    table.PrimaryKey("PK_Todos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Todos",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "Priority", "Text", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("08daa6de-fabb-4e57-86e0-386a66fe8ae7"), null, null, null, null, false, 0, "Write project proposal 📃", null, null },
                    { new Guid("08daa6df-0599-4227-8e79-ac83b22305f3"), null, null, null, null, false, 1, "Schedule kick-off meeting ✅", null, null },
                    { new Guid("08daa6df-1457-4ff3-8080-a57e71d0d80c"), null, null, null, null, false, 2, "Review research results 🤯", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Todos");
        }
    }
}
