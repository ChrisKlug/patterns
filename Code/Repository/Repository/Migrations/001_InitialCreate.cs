using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Repository.Data;

namespace Repository.Migrations
{
    [DbContext(typeof(UsersContext))]
    [Migration("001_InitialCreate")]
    public class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    IsAdmin = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "FirstName", "IsAdmin", "IsDeleted", "LastName" },
                values: new object[] { 1, "Chris", true, false, "Klug" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "FirstName", "IsAdmin", "IsDeleted", "LastName" },
                values: new object[] { 2, "Darth", true, false, "Vader" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "FirstName", "IsAdmin", "IsDeleted", "LastName" },
                values: new object[] { 3, "Chewbacca", false, false, "" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "FirstName", "IsAdmin", "IsDeleted", "LastName" },
                values: new object[] { 4, "R2D2", false, false, "" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "FirstName", "IsAdmin", "IsDeleted", "LastName" },
                values: new object[] { 5, "Han", false, true, "Solo" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "FirstName", "IsAdmin", "IsDeleted", "LastName" },
                values: new object[] { 6, "Obi-Wan", true, true, "Kenobi" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Users");
        }
    }
}
