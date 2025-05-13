
using System.ComponentModel.DataAnnotations.Schema;

namespace MiApiORACLE.Models
{
    [Table("CUSTOMERS")]
    public class Customer
    {
        [Column("CUSTOMER_ID")]
        public int CustomerId { get; set; }//PK
        [Column("EMAIL_ADDRESS")]
        public string? EmailAdress { get; set; }
        [Column("FULL_NAME")]
        public string? FullName { get; set; }

        //Relacion: un Clinete puede tener muchos Pedidos
        public ICollection<Order>? Orders { get; set; }

    }
}