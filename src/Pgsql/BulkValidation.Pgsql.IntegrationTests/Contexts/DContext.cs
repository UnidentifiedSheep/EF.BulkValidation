using System;
using System.Collections.Generic;
using BulkValidation.Pgsql.IntegrationTests.Entities;
using Microsoft.EntityFrameworkCore;

namespace BulkValidation.Pgsql.Benchmarks.Contexts;

public partial class DContext : DbContext
{
    public DContext()
    {
    }

    public DContext(DbContextOptions<DContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RandomDatum> RandomData { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=bulk_validation;Username=postgres;Password=PleasKillMe21");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RandomDatum>(entity =>
        {
            entity.HasKey(e => new { e.First, e.Second }).HasName("random_data_pk");

            entity.ToTable("random_data");

            entity.HasIndex(e => e.Guid, "random_data_guid_uindex").IsUnique();

            entity.HasIndex(e => e.Second, "random_data_second_index");

            entity.Property(e => e.First).HasColumnName("first");
            entity.Property(e => e.Second).HasColumnName("second");
            entity.Property(e => e.Guid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("guid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
