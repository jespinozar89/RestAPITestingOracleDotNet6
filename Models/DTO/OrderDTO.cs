using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MiApiORACLE.DTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; } //PK
        [Required]
        public int CustomerId { get; set; } //FK
        [Required]
        public string? OrderStatus { get; set; }
    }
}