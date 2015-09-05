
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MapperExemple.Entity.Interface
{
  public   interface IExempleProduct
    {
         int ProductId { get; set; } 
         string ProductName { get; set; } 
         int? SupplierId { get; set; } 
         int? CategoryId { get; set; } 
         string QuantityPerUnit { get; set; } 
         decimal? UnitPrice { get; set; } 
         short? UnitsInStock { get; set; } 
         short? UnitsOnOrder { get; set; } 
         short? ReorderLevel { get; set; } 
         bool Discontinued { get; set; } 

        IList<IExempleProduct> GetProductsList();

        IQueryable<IExempleProduct> GetProducts();
        IExempleProduct GetFirstProduct();

        IList<TResult> GetProducts2<TResult>(Expression<Func<Product, TResult>> selectQuery);
    }
}