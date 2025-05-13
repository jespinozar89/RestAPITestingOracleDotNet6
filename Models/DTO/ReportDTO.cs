
namespace MiApiORACLE.Models.DTO
{
    public class ReportDTO
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderTimestamp { get; set; }
        public string? FullName { get; set; }
        public string? EmailAdress { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
        public int TotalQuantity { get; set; }
        public int ShipmentId { get; set; }

    }
}