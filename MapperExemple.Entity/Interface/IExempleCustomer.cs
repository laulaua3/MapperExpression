using System.Collections.Generic;
using System.Linq;
using MapperExemple.Entity.EF;
using System;

namespace MapperExemple.Entity.Interface
{
    public interface IExempleCustomer: IDisposable
    {
        IQueryable<Customer> GetCustomers();
        List<Customer> GetCustomersList();
        Customer GetFirstCustomer();
    }
}