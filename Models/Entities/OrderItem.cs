using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MiApiORACLE.Models
{
    [Table("ORDERS_ITEMS")]
    public class OrderItem
    {
        [Column("ORDER_ID")]
        public int OrderId { get; set; } //FK
        [Column("LINE_ITEM_ID")]
        public int LineItemId { get; set; } //PK
        [Column("PRODUCT_ID")]
        public int ProductId { get; set; } //FK
        [Column("UNIT_PRICE", TypeName = "NUMBER(18,2)")]
        public decimal UnitPrice { get; set; }
        [Column("QUANTITY")]
        public int Quantity { get; set; }
        [Column("SHIPMENT_ID")]
        public int? ShipmentId { get; set; } //FK

        //Relacion: Un item pertenece a un pedido
        public Order? Order { get; set; }
    }
}