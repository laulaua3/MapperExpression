using System;
using System.Collections.Generic;
namespace MapperExemple.Web.Models
{
    public class OrderModel
    {
        public int OrderId { get; set; } 
        public string CustomerId { get; set; }
        public int? EmployeeId { get; set; } 
        public DateTime? OrderDate { get; set; } 
        public DateTime? RequiredDate { get; set; } 
        public DateTime? ShippedDate { get; set; }
        public int? ShipVia { get; set; } 
      
        public string ShipAddress { get; set; } 
        public string ShipCity { get; set; } 
        public string ShipRegion { get; set; }
        public string ShipPostalCode { get; set; } 
        public string ShipCountry { get; set; }

        public string  CustomerName { get; set; }

        public virtual ICollection<OrderDetailModel> OrderDetails { get; set; }

      
        public virtual CustomerModel Customer { get; set; } 


        
    }
}