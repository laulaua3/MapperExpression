using MapperExemple.Entity.EF;
using MapperExemple.Entity.Interface;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MapperExemple.Entity
{
    public class ExempleCustomer : ExempleBase, IExempleCustomer
    {
        #region Simple mapping

        public Customer GetFirstCustomer()
        {
            var result = from c in _context.Customers
                         select c;
            return result.FirstOrDefault();

        }

        public IQueryable<Customer> GetCustomers()
        {


            var result = from c in _context.Customers
                         select c;
            return result;

        }

        public List<Customer> GetCustomersList()
        {

            var result = from c in _context.Customers
                         select c;
            return result.ToList();

        }


        #endregion


    }
}
