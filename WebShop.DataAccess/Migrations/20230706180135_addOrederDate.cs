using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebShop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addOrederDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderDate",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "OrderDetails");
        }
    }
}
