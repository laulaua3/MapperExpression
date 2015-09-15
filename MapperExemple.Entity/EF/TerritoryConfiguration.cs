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
    // Territories
    internal class TerritoryConfiguration : EntityTypeConfiguration<Territory>
    {
        public TerritoryConfiguration()
            : this("dbo")
        {
        }
 
        public TerritoryConfiguration(string schema)
        {
            ToTable(schema + ".Territories");
            HasKey(x => x.TerritoryId);

            Property(x => x.TerritoryId).HasColumnName("TerritoryID").IsRequired().HasColumnType("nvarchar").HasMaxLength(20).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.TerritoryDescription).HasColumnName("TerritoryDescription").IsRequired().IsFixedLength().HasColumnType("nchar").HasMaxLength(50);
            Property(x => x.RegionId).HasColumnName("RegionID").IsRequired().HasColumnType("int");

            // Foreign keys
            HasRequired(a => a.Region).WithMany(b => b.Territories).HasForeignKey(c => c.RegionId); // FK_Territories_Region
        }
    }

}
