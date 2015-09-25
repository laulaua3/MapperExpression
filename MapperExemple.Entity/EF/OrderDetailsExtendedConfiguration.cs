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
    // Order Details Extended
    internal class OrderDetailsExtendedConfiguration : EntityTypeConfiguration<OrderDetailsExtended>
    {
        public OrderDetailsExtendedConfiguration()
            : this("dbo")
        {
        }
 
        public OrderDetailsExtendedConfiguration(string schema)
        {
            ToTable(schema + ".Order Details Extended");
            HasKey(x => new { x.OrderId, x.ProductId, x.ProductName, x.UnitPrice, x.Quantity, x.Discount });

            Property(x => x.OrderId).HasColumnName("OrderID").IsRequired().HasColumnType("int");
            Property(x => x.ProductId).HasColumnName("ProductID").IsRequired().HasColumnType("int");
            Property(x => x.ProductName).HasColumnName("ProductName").IsRequired().HasColumnType("nvarchar").HasMaxLength(40);
            Property(x => x.UnitPrice).HasColumnName("UnitPrice").IsRequired().HasColumnType("money").HasPrecision(19,4);
            Property(x => x.Quantity).HasColumnName("Quantity").IsRequired().HasColumnType("smallint");
            Property(x => x.Discount).HasColumnName("Discount").IsRequired().HasColumnType("real");
            Property(x => x.ExtendedPrice).HasColumnName("ExtendedPrice").IsOptional().HasColumnType("money").HasPrecision(19,4);
        }
    }

}
