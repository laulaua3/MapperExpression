using System.Collections.Generic;
using System.Linq;
using MapperExemple.Entity.EF;

namespace MapperExemple.Entity.Interface
{
    public interface IExempleCustomer
    {
        IQueryable<Customer> GetCustomers();
        List<Customer> GetCustomersList();
        Customer GetFirstCustomer();
    }
}