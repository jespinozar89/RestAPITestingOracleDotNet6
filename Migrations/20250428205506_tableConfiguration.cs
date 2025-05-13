using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiApiORACLE.Migrations
{
    public partial class tableConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CUSTOMERS",
                columns: table => new
                {
                    CUSTOMER_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    EMAIL_ADDRESS = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    FULL_NAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOMERS", x => x.CUSTOMER_ID);
                });

            migrationBuilder.CreateTable(
                name: "ORDERS",
                columns: table => new
                {
                    ORDER_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ORDER_TMS = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CUSTOMER_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ORDER_STATUS = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    STORE_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDERS", x => x.ORDER_ID);
                    table.ForeignKey(
                        name: "FK_ORDERS_CUSTOMERS_CUSTOMER_ID",
                        column: x => x.CUSTOMER_ID,
                        principalTable: "CUSTOMERS",
                        principalColumn: "CUSTOMER_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ORDERS_ITEMS",
                columns: table => new
                {
                    ORDER_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    LINE_ITEM_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PRODUCT_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UNIT_PRICE = table.Column<decimal>(type: "DECIMAL(18, 2)", nullable: false),
                    QUANTITY = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SHIPMENT_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDERS_ITEMS", x => new { x.ORDER_ID, x.LINE_ITEM_ID });
                    table.ForeignKey(
                        name: "FK_ORDERS_ITEMS_ORDERS_ORDER_ID",
                        column: x => x.ORDER_ID,
                        principalTable: "ORDERS",
                        principalColumn: "ORDER_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ORDERS_CUSTOMER_ID",
                table: "ORDERS",
                column: "CUSTOMER_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ORDERS_ITEMS");

            migrationBuilder.DropTable(
                name: "ORDERS");

            migrationBuilder.DropTable(
                name: "CUSTOMERS");
        }
    }
}
