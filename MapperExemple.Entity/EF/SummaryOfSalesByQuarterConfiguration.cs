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
    // Summary of Sales by Quarter
    internal class SummaryOfSalesByQuarterConfiguration : EntityTypeConfiguration<SummaryOfSalesByQuarter>
    {
        public SummaryOfSalesByQuarterConfiguration()
            : this("dbo")
        {
        }
 
        public SummaryOfSalesByQuarterConfiguration(string schema)
        {
            ToTable(schema + ".Summary of Sales by Quarter");
            HasKey(x => x.OrderId);

            Property(x => x.ShippedDate).HasColumnName("ShippedDate").IsOptional().HasColumnType("datetime");
            Property(x => x.OrderId).HasColumnName("OrderID").IsRequired().HasColumnType("int");
            Property(x => x.Subtotal).HasColumnName("Subtotal").IsOptional().HasColumnType("money").HasPrecision(19,4);
        }
    }

}
