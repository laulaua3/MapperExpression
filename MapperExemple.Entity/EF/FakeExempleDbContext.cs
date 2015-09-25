// ReSharper disable RedundantUsingDirective
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable RedundantNameQualifier
// TargetFrameworkVersion = 4.51
#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.Entity.ModelConfiguration;
using System.Threading;
using DatabaseGeneratedOption = System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption;

namespace MapperExemple.Entity.EF
{
    [GeneratedCodeAttribute("EF.Reverse.POCO.Generator", "2.15.1.0")]
    public class FakeExempleDbContext : IExempleDbContext
    {
        public DbSet<AlphabeticalListOfProduct> AlphabeticalListOfProducts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategorySalesFor1997> CategorySalesFor1997 { get; set; }
        public DbSet<CurrentProductList> CurrentProductLists { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerAndSuppliersByCity> CustomerAndSuppliersByCities { get; set; }
        public DbSet<CustomerDemographic> CustomerDemographics { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderDetailsExtended> OrderDetailsExtendeds { get; set; }
        public DbSet<OrdersQry> OrdersQries { get; set; }
        public DbSet<OrderSubtotal> OrderSubtotals { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductsAboveAveragePrice> ProductsAboveAveragePrices { get; set; }
        public DbSet<ProductSalesFor1997> ProductSalesFor1997 { get; set; }
        public DbSet<ProductsByCategory> ProductsByCategories { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<SalesByCategory> SalesByCategories { get; set; }
        public DbSet<SalesTotalsByAmount> SalesTotalsByAmounts { get; set; }
        public DbSet<Shipper> Shippers { get; set; }
        public DbSet<SummaryOfSalesByQuarter> SummaryOfSalesByQuarters { get; set; }
        public DbSet<SummaryOfSalesByYear> SummaryOfSalesByYears { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Sysdiagram> Sysdiagrams { get; set; }
        public DbSet<Territory> Territories { get; set; }

        public FakeExempleDbContext()
        {
            AlphabeticalListOfProducts = new FakeDbSet<AlphabeticalListOfProduct>();
            Categories = new FakeDbSet<Category>();
            CategorySalesFor1997 = new FakeDbSet<CategorySalesFor1997>();
            CurrentProductLists = new FakeDbSet<CurrentProductList>();
            Customers = new FakeDbSet<Customer>();
            CustomerAndSuppliersByCities = new FakeDbSet<CustomerAndSuppliersByCity>();
            CustomerDemographics = new FakeDbSet<CustomerDemographic>();
            Employees = new FakeDbSet<Employee>();
            Invoices = new FakeDbSet<Invoice>();
            Orders = new FakeDbSet<Order>();
            OrderDetails = new FakeDbSet<OrderDetail>();
            OrderDetailsExtendeds = new FakeDbSet<OrderDetailsExtended>();
            OrdersQries = new FakeDbSet<OrdersQry>();
            OrderSubtotals = new FakeDbSet<OrderSubtotal>();
            Products = new FakeDbSet<Product>();
            ProductsAboveAveragePrices = new FakeDbSet<ProductsAboveAveragePrice>();
            ProductSalesFor1997 = new FakeDbSet<ProductSalesFor1997>();
            ProductsByCategories = new FakeDbSet<ProductsByCategory>();
            Regions = new FakeDbSet<Region>();
            SalesByCategories = new FakeDbSet<SalesByCategory>();
            SalesTotalsByAmounts = new FakeDbSet<SalesTotalsByAmount>();
            Shippers = new FakeDbSet<Shipper>();
            SummaryOfSalesByQuarters = new FakeDbSet<SummaryOfSalesByQuarter>();
            SummaryOfSalesByYears = new FakeDbSet<SummaryOfSalesByYear>();
            Suppliers = new FakeDbSet<Supplier>();
            Sysdiagrams = new FakeDbSet<Sysdiagram>();
            Territories = new FakeDbSet<Territory>();
        }
        
        public int SaveChangesCount { get; private set; } 
        public int SaveChanges()
        {
            ++SaveChangesCount;
            return 1;
        }

        public System.Threading.Tasks.Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
        
        // Stored Procedures
        public List<CustOrderHistReturnModel> CustOrderHist(string customerId)
        {
            int procResult;
            return CustOrderHist(customerId, out procResult);
        }

        public List<CustOrderHistReturnModel> CustOrderHist(string customerId, out int procResult)
        {
 
            procResult = 0;
            return new List<CustOrderHistReturnModel>();
        }

        public List<CustOrdersDetailReturnModel> CustOrdersDetail(int? orderId)
        {
            int procResult;
            return CustOrdersDetail(orderId, out procResult);
        }

        public List<CustOrdersDetailReturnModel> CustOrdersDetail(int? orderId, out int procResult)
        {
 
            procResult = 0;
            return new List<CustOrdersDetailReturnModel>();
        }

        public List<CustOrdersOrdersReturnModel> CustOrdersOrders(string customerId)
        {
            int procResult;
            return CustOrdersOrders(customerId, out procResult);
        }

        public List<CustOrdersOrdersReturnModel> CustOrdersOrders(string customerId, out int procResult)
        {
 
            procResult = 0;
            return new List<CustOrdersOrdersReturnModel>();
        }

        public List<EmployeeSalesByCountryReturnModel> EmployeeSalesByCountry(DateTime? beginningDate, DateTime? endingDate)
        {
            int procResult;
            return EmployeeSalesByCountry(beginningDate, endingDate, out procResult);
        }

        public List<EmployeeSalesByCountryReturnModel> EmployeeSalesByCountry(DateTime? beginningDate, DateTime? endingDate, out int procResult)
        {
 
            procResult = 0;
            return new List<EmployeeSalesByCountryReturnModel>();
        }

        public List<SalesByYearReturnModel> SalesByYear(DateTime? beginningDate, DateTime? endingDate)
        {
            int procResult;
            return SalesByYear(beginningDate, endingDate, out procResult);
        }

        public List<SalesByYearReturnModel> SalesByYear(DateTime? beginningDate, DateTime? endingDate, out int procResult)
        {
 
            procResult = 0;
            return new List<SalesByYearReturnModel>();
        }

        public List<SalesByCategoryReturnModel> SalesByCategory(string categoryName, string ordYear)
        {
            int procResult;
            return SalesByCategory(categoryName, ordYear, out procResult);
        }

        public List<SalesByCategoryReturnModel> SalesByCategory(string categoryName, string ordYear, out int procResult)
        {
 
            procResult = 0;
            return new List<SalesByCategoryReturnModel>();
        }

        public List<TenMostExpensiveProductsReturnModel> TenMostExpensiveProducts()
        {
            int procResult;
            return TenMostExpensiveProducts(out procResult);
        }

        public List<TenMostExpensiveProductsReturnModel> TenMostExpensiveProducts( out int procResult)
        {
 
            procResult = 0;
            return new List<TenMostExpensiveProductsReturnModel>();
        }

    }
}
