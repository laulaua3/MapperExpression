using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperExemple.Entity
{
   public class ExempleOrder
    {
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
