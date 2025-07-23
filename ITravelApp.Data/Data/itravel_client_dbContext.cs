using System;
using System.Collections.Generic;
using ITravelApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ITravelApp.Data.Data;

public partial class itravel_client_dbContext : DbContext
{
    public itravel_client_dbContext()
    {
    }

    public itravel_client_dbContext(DbContextOptions<itravel_client_dbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<destination_img> destination_imgs { get; set; }

    public virtual DbSet<destination_main> destination_mains { get; set; }

    public virtual DbSet<destination_translation> destination_translations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost:5432;Database=itravel_client_db;Username=postgres;Password=Berlin2020");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<destination_img>(entity =>
        {
            entity.HasKey(e => e.id).HasName("destination_imgs_pkey");

            entity.Property(e => e.img_name).HasMaxLength(100);
            entity.Property(e => e.img_path).HasMaxLength(100);
        });

        modelBuilder.Entity<destination_main>(entity =>
        {
            entity.HasKey(e => e.id).HasName("destination_main_pkey");

            entity.ToTable("destination_main");

            entity.Property(e => e.country_code).HasMaxLength(20);
            entity.Property(e => e.dest_code).HasMaxLength(20);
        });

        modelBuilder.Entity<destination_translation>(entity =>
        {
            entity.HasKey(e => e.id).HasName("destination_translations_pkey");

            entity.Property(e => e.dest_name).HasMaxLength(50);
            entity.Property(e => e.lang_code).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
