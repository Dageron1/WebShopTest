using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebShop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class beforeDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "OrderBeforeDiscount",
                table: "OrderHeaders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderBeforeDiscount",
                table: "OrderHeaders");
        }
    }
}
