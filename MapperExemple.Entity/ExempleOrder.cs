using MapperExemple.Entity.EF;
using System.Diagnostics;
using System.Linq;

namespace MapperExemple.Entity
{
    public class ExempleOrder: ExempleBase
    {
        #region Custom mapping

        public Order GetFirstOrder()
        {
           
                
                // Need to include the sub object.
                var result = _context.Orders
                    .Include("Customer")
                    .Include("OrderDetails");
                return result.FirstOrDefault();
            
        }

        public IQueryable<Order> GetOrders()
        {
         

            var result = _context.Orders;

            return result;

        }

        #endregion
    }
}
