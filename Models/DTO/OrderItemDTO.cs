using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MiApiORACLE.DTO
{
    public class OrderItemDTO
    {
        public int LineItemId { get; set; } //PK
        [Required]
        public int OrderId { get; set; } //FK
        [Required]
        public int ProductId { get; set; } //FK
        [Required]
        public decimal UnitPrice { get; set; }
        [Required]
        public int Quantity { get; set; }

    }
}