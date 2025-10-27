using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AchuBan_ECom.Migrations
{
    /// <inheritdoc />
    public partial class addingProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Price50 = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Price100 = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "CategoryId", "Description", "ISBN", "ListPrice", "Name", "Price100", "Price50" },
                values: new object[,]
                {
                    { 1, "Mr. Max seed", 1, "Description 1", "ISBN-123", 10.00m, "Product 1", 8.00m, 9.00m },
                    { 2, "Mr. Seed 123", 2, "Description 2", "ISBN-456", 20.00m, "Product 2", 16.00m, 18.00m },
                    { 3, "Mr. Mooc seed", 3, "Description 3", "ISBN-789", 30.00m, "Product 3", 24.00m, 27.00m },
                    { 4, "Mr. Mussie seed", 1, "Description 4", "ISBN-101", 40.00m, "Product 4", 32.00m, 36.00m },
                    { 5, "Mr. Max seed", 2, "Description 5", "ISBN-102", 50.00m, "Product 5", 40.00m, 45.00m },
                    { 6, "Mr. Haile seed", 3, "Description 6", "ISBN-103", 60.00m, "Product 6", 48.00m, 54.00m },
                    { 7, "Mr. Max seed", 1, "Description 7", "ISBN-104", 70.00m, "Product 7", 56.00m, 63.00m },
                    { 8, "Mr. Assin seed", 2, "Description 8", "ISBN-105", 80.00m, "Product 8", 64.00m, 72.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
