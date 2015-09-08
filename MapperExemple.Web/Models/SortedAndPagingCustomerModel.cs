using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapperExemple.Web.Models
{
    public class SortedAndPagingCustomerModel : SortedAndPagingModel
    {

      
        public IList<CustomerModel> Customers { get; set; }

        public SortedAndPagingCustomerModel()
        {
            
        }
    }

}