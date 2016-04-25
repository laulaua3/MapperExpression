using System.Linq;
using MapperExemple.Entity.EF;

namespace MapperExemple.Entity.Interface
{
    public interface IExempleOrder
    {
        Order GetFirstOrder();
        IQueryable<Order> GetOrders();
    }
}