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
    // Products by Category
    internal class ProductsByCategoryConfiguration : EntityTypeConfiguration<ProductsByCategory>
    {
        public ProductsByCategoryConfiguration()
            : this("dbo")
        {
        }
 
        public ProductsByCategoryConfiguration(string schema)
        {
            ToTable(schema + ".Products by Category");
            HasKey(x => new { x.CategoryName, x.ProductName, x.Discontinued });

            Property(x => x.CategoryName).HasColumnName("CategoryName").IsRequired().HasColumnType("nvarchar").HasMaxLength(15);
            Property(x => x.ProductName).HasColumnName("ProductName").IsRequired().HasColumnType("nvarchar").HasMaxLength(40);
            Property(x => x.QuantityPerUnit).HasColumnName("QuantityPerUnit").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.UnitsInStock).HasColumnName("UnitsInStock").IsOptional().HasColumnType("smallint");
            Property(x => x.Discontinued).HasColumnName("Discontinued").IsRequired().HasColumnType("bit");
        }
    }

}
