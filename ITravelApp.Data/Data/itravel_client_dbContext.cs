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

    public virtual DbSet<facility_main> facility_mains { get; set; }

    public virtual DbSet<facility_translation> facility_translations { get; set; }

    public virtual DbSet<tbl_review> tbl_reviews { get; set; }

    public virtual DbSet<trip_facility> trip_facilities { get; set; }

    public virtual DbSet<trip_img> trip_imgs { get; set; }

    public virtual DbSet<trip_main> trip_mains { get; set; }

    public virtual DbSet<trip_pickups_main> trip_pickups_mains { get; set; }

    public virtual DbSet<trip_pickups_translation> trip_pickups_translations { get; set; }

    public virtual DbSet<trip_price> trip_prices { get; set; }

    public virtual DbSet<trip_translation> trip_translations { get; set; }

    public virtual DbSet<tripwithdetail> tripwithdetails { get; set; }

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

        modelBuilder.Entity<facility_main>(entity =>
        {
            entity.HasKey(e => e.id).HasName("facility_main_pkey");

            entity.ToTable("facility_main");

            entity.Property(e => e.facility_code).HasMaxLength(20);
            entity.Property(e => e.facility_default_name).HasMaxLength(50);
        });

        modelBuilder.Entity<facility_translation>(entity =>
        {
            entity.HasKey(e => e.id).HasName("facility_translations_pkey");

            entity.Property(e => e.lang_code).HasMaxLength(5);
        });

        modelBuilder.Entity<tbl_review>(entity =>
        {
            entity.HasKey(e => e.id).HasName("tbl_reviews_pkey");

            entity.Property(e => e.client_id).HasMaxLength(100);
            entity.Property(e => e.entry_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.review_title).HasMaxLength(100);
            entity.Property(e => e.trip_type).HasComment("1 = exercusion trip\n2 = transfer trip");
        });

        modelBuilder.Entity<trip_facility>(entity =>
        {
            entity.HasKey(e => e.id).HasName("trip_facility_pkey");

            entity.ToTable("trip_facility");
        });

        modelBuilder.Entity<trip_img>(entity =>
        {
            entity.HasKey(e => e.id).HasName("trip_imgs_pkey");

            entity.Property(e => e.img_name).HasMaxLength(50);
            entity.Property(e => e.img_path).HasMaxLength(50);
        });

        modelBuilder.Entity<trip_main>(entity =>
        {
            entity.HasKey(e => e.id).HasName("trip_main_pkey");

            entity.ToTable("trip_main");

            entity.Property(e => e.pickup).HasMaxLength(20);
            entity.Property(e => e.trip_code).HasMaxLength(20);
            entity.Property(e => e.trip_default_name).HasMaxLength(50);
            entity.Property(e => e.trip_duration).HasMaxLength(20);
        });

        modelBuilder.Entity<trip_pickups_main>(entity =>
        {
            entity.HasKey(e => e.id).HasName("trip_pickups_main_pkey");

            entity.ToTable("trip_pickups_main");

            entity.Property(e => e.duration).HasMaxLength(20);
            entity.Property(e => e.pickup_code).HasMaxLength(50);
            entity.Property(e => e.pickup_default_name).HasMaxLength(100);
            entity.Property(e => e.trip_type).HasComment("1 = exercusion trip\n2 = transfer trip");
        });

        modelBuilder.Entity<trip_pickups_translation>(entity =>
        {
            entity.HasKey(e => e.id).HasName("trip_pickups_translations_pkey");

            entity.Property(e => e.lang_code).HasMaxLength(5);
            entity.Property(e => e.pickup_name).HasMaxLength(100);
        });

        modelBuilder.Entity<trip_price>(entity =>
        {
            entity.HasKey(e => e.id).HasName("trip_prices_pkey");

            entity.Property(e => e.currency_code).HasMaxLength(5);
        });

        modelBuilder.Entity<trip_translation>(entity =>
        {
            entity.HasKey(e => e.id).HasName("trip_translations_pkey");

            entity.Property(e => e.lang_code).HasMaxLength(5);
            entity.Property(e => e.trip_name).HasMaxLength(50);
        });

        modelBuilder.Entity<tripwithdetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("tripwithdetails");

            entity.Property(e => e.country_code).HasMaxLength(20);
            entity.Property(e => e.currency_code).HasMaxLength(5);
            entity.Property(e => e.default_img).HasMaxLength(50);
            entity.Property(e => e.dest_code).HasMaxLength(20);
            entity.Property(e => e.lang_code).HasMaxLength(5);
            entity.Property(e => e.pickup).HasMaxLength(20);
            entity.Property(e => e.trip_code).HasMaxLength(20);
            entity.Property(e => e.trip_default_name).HasMaxLength(50);
            entity.Property(e => e.trip_duration).HasMaxLength(20);
            entity.Property(e => e.trip_name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
