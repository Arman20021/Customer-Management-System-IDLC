using System;
using System.Collections.Generic;
using CMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.DAL.Data;

public partial class CmsDbContext : DbContext
{
    public CmsDbContext(DbContextOptions<CmsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; }

    public virtual DbSet<CustomerDocument> CustomerDocuments { get; set; }

    public virtual DbSet<CustomerMobile> CustomerMobiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK_dbo.Customers");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.LastUpdatedDate).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasDefaultValue("Inactive");
        });

        modelBuilder.Entity<CustomerAddress>(entity =>
        {
            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerAddresses).HasConstraintName("FK_CustomerAddresses_Customers");
        });

        modelBuilder.Entity<CustomerDocument>(entity =>
        {
            entity.Property(e => e.UploadedDate).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerDocuments).HasConstraintName("FK_CustomerDocuments_Customers");
        });

        modelBuilder.Entity<CustomerMobile>(entity =>
        {
            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerMobiles).HasConstraintName("FK_CustomerMobiles_Customers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
