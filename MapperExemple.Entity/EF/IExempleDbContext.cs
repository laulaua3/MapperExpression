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
    public interface IExempleDbContext : IDisposable
    {
        DbSet<AlphabeticalListOfProduct> AlphabeticalListOfProducts { get; set; } // Alphabetical list of products
        DbSet<Category> Categories { get; set; } // Categories
        DbSet<CategorySalesFor1997> CategorySalesFor1997 { get; set; } // Category Sales for 1997
        DbSet<CurrentProductList> CurrentProductLists { get; set; } // Current Product List
        DbSet<Customer> Customers { get; set; } // Customers
        DbSet<CustomerAndSuppliersByCity> CustomerAndSuppliersByCities { get; set; } // Customer and Suppliers by City
        DbSet<CustomerDemographic> CustomerDemographics { get; set; } // CustomerDemographics
        DbSet<Employee> Employees { get; set; } // Employees
        DbSet<Invoice> Invoices { get; set; } // Invoices
        DbSet<Order> Orders { get; set; } // Orders
        DbSet<OrderDetail> OrderDetails { get; set; } // Order Details
        DbSet<OrderDetailsExtended> OrderDetailsExtendeds { get; set; } // Order Details Extended
        DbSet<OrdersQry> OrdersQries { get; set; } // Orders Qry
        DbSet<OrderSubtotal> OrderSubtotals { get; set; } // Order Subtotals
        DbSet<Product> Products { get; set; } // Products
        DbSet<ProductsAboveAveragePrice> ProductsAboveAveragePrices { get; set; } // Products Above Average Price
        DbSet<ProductSalesFor1997> ProductSalesFor1997 { get; set; } // Product Sales for 1997
        DbSet<ProductsByCategory> ProductsByCategories { get; set; } // Products by Category
        DbSet<Region> Regions { get; set; } // Region
        DbSet<SalesByCategory> SalesByCategories { get; set; } // Sales by Category
        DbSet<SalesTotalsByAmount> SalesTotalsByAmounts { get; set; } // Sales Totals by Amount
        DbSet<Shipper> Shippers { get; set; } // Shippers
        DbSet<SummaryOfSalesByQuarter> SummaryOfSalesByQuarters { get; set; } // Summary of Sales by Quarter
        DbSet<SummaryOfSalesByYear> SummaryOfSalesByYears { get; set; } // Summary of Sales by Year
        DbSet<Supplier> Suppliers { get; set; } // Suppliers
        DbSet<Sysdiagram> Sysdiagrams { get; set; } // sysdiagrams
        DbSet<Territory> Territories { get; set; } // Territories

        int SaveChanges();
        System.Threading.Tasks.Task<int> SaveChangesAsync();
        System.Threading.Tasks.Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        
        // Stored Procedures
        List<CustOrderHistReturnModel> CustOrderHist(string customerId, out int procResult);
        List<CustOrdersDetailReturnModel> CustOrdersDetail(int? orderId, out int procResult);
        List<CustOrdersOrdersReturnModel> CustOrdersOrders(string customerId, out int procResult);
        List<EmployeeSalesByCountryReturnModel> EmployeeSalesByCountry(DateTime? beginningDate, DateTime? endingDate, out int procResult);
        List<SalesByYearReturnModel> SalesByYear(DateTime? beginningDate, DateTime? endingDate, out int procResult);
        List<SalesByCategoryReturnModel> SalesByCategory(string categoryName, string ordYear, out int procResult);
        List<TenMostExpensiveProductsReturnModel> TenMostExpensiveProducts( out int procResult);
    }

}
