using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapperExemple.Web.Models
{
    public class SortedCustomerModel
    {

        public string SortField { get; set; }
        public string SortDirection { get; set; }

        public IList<CustomerModel> Customers { get; set; }

        public SortedCustomerModel()
        {
            
        }
    }
    public class SortingPagingInfo
    {
      
 
    }

}