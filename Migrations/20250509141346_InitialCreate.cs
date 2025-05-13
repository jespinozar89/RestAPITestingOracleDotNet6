using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiApiORACLE.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "UNIT_PRICE",
                table: "ORDERS_ITEMS",
                type: "NUMBER(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "SHIPMENT_ID",
                table: "ORDERS_ITEMS",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "UNIT_PRICE",
                table: "ORDERS_ITEMS",
                type: "DECIMAL(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "NUMBER(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "SHIPMENT_ID",
                table: "ORDERS_ITEMS",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);
        }
    }
}
