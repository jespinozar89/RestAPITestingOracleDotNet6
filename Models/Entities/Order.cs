using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MiApiORACLE.Models
{
    [Table("ORDERS")]
    public class Order
    {
        [Column("ORDER_ID")]
        public int OrderId { get; set; } //PK
        [Column("ORDER_TMS")]
        public DateTime OrderTimestamp { get; set; }
        [Column("CUSTOMER_ID")]
        public int CustomerId { get; set; } //FK
        [Column("ORDER_STATUS")]
        public string? OrderStatus { get; set; }
        [Column("STORE_ID")]
        public int StoreId { get; set; } //FK

        //Relacion: un pedido pertenece a un cliente
        public Customer? Customer { get; set; }

        //Relacion:un pedido puede teenr muchos items
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}