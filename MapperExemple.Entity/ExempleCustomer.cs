using MapperExemple.Entity.EF;
using MapperExemple.Entity.Interface;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MapperExemple.Entity
{
    public class ExempleCustomer : IExempleCustomer
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
            //It's only un exemple ,don't use in normal development
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
