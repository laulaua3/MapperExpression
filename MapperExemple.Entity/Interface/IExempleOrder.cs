using System.Linq;
using MapperExemple.Entity.EF;
using System;

namespace MapperExemple.Entity.Interface
{
    public interface IExempleOrder: IDisposable
    {
        Order GetFirstOrder();
        IQueryable<Order> GetOrders();
    }
}