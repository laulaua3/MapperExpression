using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperExemple.Entity
{
    public class ExempleEntity
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

        #region Custom mapping

        public Order GetFirstOrder()
        {
            using (ExempleDbContext context = new ExempleDbContext())
            {

                context.Database.Log = x => Debug.WriteLine(x);
                //Need to include the sub object
                var result = context.Orders.Include("Customer");


                return result.FirstOrDefault();
            }
        }

        public IQueryable<Order> GetOrders()
        {
            ExempleDbContext context = new ExempleDbContext();

            context.Database.Log = x => Debug.WriteLine(x);

            var result = context.Orders;

            return result;

        }

        #endregion
    }
}
