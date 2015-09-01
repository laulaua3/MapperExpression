using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MapperExemple.Entity
{
    public class ExempleCustomer
    {
        #region Simple mapping

        public Customer GetFirstCustomer()
        {
            using (ExempleDbContext context = new ExempleDbContext())
            {
                context.Database.Log = x => Debug.WriteLine(x);
                var result = from c in context.Customers
                             select c;
                return result.FirstOrDefault();
            }
        }

        public IQueryable<Customer> GetCustomers()
        {
            ExempleDbContext context = new ExempleDbContext();
            context.Database.Log = x => Debug.WriteLine(x);
            var result = from c in context.Customers
                         select c;
            return result;

        }

        public List<Customer> GetCustomersList()
        {
            using (ExempleDbContext context = new ExempleDbContext())
            {
                context.Database.Log = x => Debug.WriteLine(x);
                var result = from c in context.Customers
                             select c;
                return result.ToList();
            }
        }


        #endregion

     
    }
}
