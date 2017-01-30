
using MapperExemple.Entity.EF;
using MapperExemple.Entity.Interface;
using MapperExpression.Extensions;
using System;
using System.Collections.Generic;

using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace MapperExemple.Entity
{
    public class ExempleProduct : ExempleBase, IExempleProduct
    {

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? SupplierId { get; set; }
        public int? CategoryId { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        public IExempleProduct GetFirstProduct()
        {
            IExempleProduct result ;

            result = _context.Products.Select<Product, IExempleProduct>().FirstOrDefault();
            Dispose();
            return result;
        }

        public IList<IExempleProduct> GetProductsList()
        {
            List<IExempleProduct> result ;

            result = _context.Products.Select<Product, IExempleProduct>().ToList();

            Dispose();
            return result;
        }
        public IList<TResult> GetProductsWithCriterias<TResult>(Expression<Func<IExempleProduct, bool>> criterias, Expression<Func<Product, TResult>> selectQuery)
        {
            List<TResult> result ;
            result = GetEntities(criterias, selectQuery).Take(10).ToList();
            Dispose();
            return result;
        }

        public IQueryable<IExempleProduct> GetProducts()
        {
            ExempleDbContext context = new ExempleDbContext();

            context.Database.Log = x => Debug.WriteLine(x);

            var result = context.Products;

            return result.Select<Product, IExempleProduct>();
        }

        public IList<TResult> GetProducts2<TResult>(Expression<Func<Product, TResult>> selectQuery)
        {
            var result = GetEntities(selectQuery).ToList();
            return result;
        }
    }
}
