namespace MapperExemple.Web.Models
{
    public class OrderDetailModel
    {
        public int OrderId { get; set; } // OrderID (Primary key)
        public int ProductId { get; set; } // ProductID (Primary key)
        public decimal UnitPrice { get; set; } // UnitPrice
        public short Quantity { get; set; } // Quantity
        public float Discount { get; set; } // Discount
    }
}