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
    // Employees
    internal class EmployeeConfiguration : EntityTypeConfiguration<Employee>
    {
        public EmployeeConfiguration()
            : this("dbo")
        {
        }
 
        public EmployeeConfiguration(string schema)
        {
            ToTable(schema + ".Employees");
            HasKey(x => x.EmployeeId);

            Property(x => x.EmployeeId).HasColumnName("EmployeeID").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.LastName).HasColumnName("LastName").IsRequired().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.FirstName).HasColumnName("FirstName").IsRequired().HasColumnType("nvarchar").HasMaxLength(10);
            Property(x => x.Title).HasColumnName("Title").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.TitleOfCourtesy).HasColumnName("TitleOfCourtesy").IsOptional().HasColumnType("nvarchar").HasMaxLength(25);
            Property(x => x.BirthDate).HasColumnName("BirthDate").IsOptional().HasColumnType("datetime");
            Property(x => x.HireDate).HasColumnName("HireDate").IsOptional().HasColumnType("datetime");
            Property(x => x.Address).HasColumnName("Address").IsOptional().HasColumnType("nvarchar").HasMaxLength(60);
            Property(x => x.City).HasColumnName("City").IsOptional().HasColumnType("nvarchar").HasMaxLength(15);
            Property(x => x.Region).HasColumnName("Region").IsOptional().HasColumnType("nvarchar").HasMaxLength(15);
            Property(x => x.PostalCode).HasColumnName("PostalCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
            Property(x => x.Country).HasColumnName("Country").IsOptional().HasColumnType("nvarchar").HasMaxLength(15);
            Property(x => x.HomePhone).HasColumnName("HomePhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(24);
            Property(x => x.Extension).HasColumnName("Extension").IsOptional().HasColumnType("nvarchar").HasMaxLength(4);
            Property(x => x.Photo).HasColumnName("Photo").IsOptional().HasColumnType("image").HasMaxLength(2147483647);
            Property(x => x.Notes).HasColumnName("Notes").IsOptional().HasColumnType("ntext").IsMaxLength();
            Property(x => x.ReportsTo).HasColumnName("ReportsTo").IsOptional().HasColumnType("int");
            Property(x => x.PhotoPath).HasColumnName("PhotoPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);

            // Foreign keys
            HasOptional(a => a.Employee_ReportsTo).WithMany(b => b.Employees).HasForeignKey(c => c.ReportsTo); // FK_Employees_Employees
            HasMany(t => t.Territories).WithMany(t => t.Employees).Map(m => 
            {
                m.ToTable("EmployeeTerritories", "dbo");
                m.MapLeftKey("EmployeeID");
                m.MapRightKey("TerritoryID");
            });
        }
    }

}
