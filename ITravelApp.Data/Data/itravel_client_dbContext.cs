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

    public virtual DbSet<client_Profile> client_Profiles { get; set; }

    public virtual DbSet<client_image> client_images { get; set; }

    public virtual DbSet<client_notification_setting> client_notification_settings { get; set; }

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

    public virtual DbSet<trips_booking> trips_bookings { get; set; }

    public virtual DbSet<trips_wishlist> trips_wishlists { get; set; }

    public virtual DbSet<tripwithdetail> tripwithdetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost:5432;Database=itravel_client_db;Username=postgres;Password=Berlin2020");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<client_Profile>(entity =>
        {
            entity.HasKey(e => e.profile_id).HasName("client_Profile_pkey");

            entity.ToTable("client_Profile");

            entity.Property(e => e.client_email).HasMaxLength(50);
            entity.Property(e => e.client_id).HasColumnType("character varying");
            entity.Property(e => e.client_name).HasMaxLength(50);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.created_by).HasMaxLength(100);
            entity.Property(e => e.gender).HasMaxLength(50);
            entity.Property(e => e.lang).HasMaxLength(20);
            entity.Property(e => e.nation).HasMaxLength(50);
            entity.Property(e => e.pay_code).HasMaxLength(20);
            entity.Property(e => e.phone_number).HasMaxLength(50);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<client_image>(entity =>
        {
            entity.HasKey(e => e.id).HasName("client_images_pkey");

            entity.Property(e => e.client_id).HasColumnType("character varying");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.created_by).HasMaxLength(100);
            entity.Property(e => e.img_name).HasMaxLength(50);
            entity.Property(e => e.img_path).HasMaxLength(100);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<client_notification_setting>(entity =>
        {
            entity.HasKey(e => e.id).HasName("client_notification_setting_pkey");

            entity.ToTable("client_notification_setting");

            entity.Property(e => e.client_id).HasMaxLength(100);
        });

        modelBuilder.Entity<destination_img>(entity =>
        {
            entity.HasKey(e => e.id).HasName("destination_imgs_pkey");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.created_by).HasMaxLength(100);
            entity.Property(e => e.img_name).HasMaxLength(100);
            entity.Property(e => e.img_path).HasMaxLength(100);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<destination_main>(entity =>
        {
            entity.HasKey(e => e.id).HasName("destination_main_pkey");

            entity.ToTable("destination_main");

            entity.Property(e => e.country_code).HasMaxLength(20);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.created_by).HasMaxLength(100);
            entity.Property(e => e.dest_code).HasMaxLength(20);
            entity.Property(e => e.route).HasMaxLength(100);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<destination_translation>(entity =>
        {
            entity.HasKey(e => e.id).HasName("destination_translations_pkey");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.created_by).HasMaxLength(100);
            entity.Property(e => e.dest_name).HasMaxLength(50);
            entity.Property(e => e.lang_code).HasMaxLength(20);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<facility_main>(entity =>
        {
            entity.HasKey(e => e.id).HasName("facility_main_pkey");

            entity.ToTable("facility_main");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.created_by).HasMaxLength(100);
            entity.Property(e => e.facility_code).HasMaxLength(20);
            entity.Property(e => e.facility_default_name).HasMaxLength(50);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<facility_translation>(entity =>
        {
            entity.HasKey(e => e.id).HasName("facility_translations_pkey");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.created_by).HasMaxLength(100);
            entity.Property(e => e.lang_code).HasMaxLength(5);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
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

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.created_by).HasMaxLength(100);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<trip_img>(entity =>
        {
            entity.HasKey(e => e.id).HasName("trip_imgs_pkey");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.created_by).HasMaxLength(100);
            entity.Property(e => e.img_name).HasMaxLength(50);
            entity.Property(e => e.img_path).HasMaxLength(50);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<trip_main>(entity =>
        {
            entity.HasKey(e => e.id).HasName("trip_main_pkey");

            entity.ToTable("trip_main");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.created_by).HasMaxLength(100);
            entity.Property(e => e.pickup).HasMaxLength(20);
            entity.Property(e => e.route).HasMaxLength(100);
            entity.Property(e => e.trip_code).HasMaxLength(20);
            entity.Property(e => e.trip_default_name).HasMaxLength(50);
            entity.Property(e => e.trip_duration).HasMaxLength(20);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<trip_pickups_main>(entity =>
        {
            entity.HasKey(e => e.id).HasName("trip_pickups_main_pkey");

            entity.ToTable("trip_pickups_main");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.created_by).HasMaxLength(100);
            entity.Property(e => e.duration).HasMaxLength(20);
            entity.Property(e => e.pickup_code).HasMaxLength(50);
            entity.Property(e => e.pickup_default_name).HasMaxLength(100);
            entity.Property(e => e.trip_type).HasComment("1 = exercusion trip\n2 = transfer trip");
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<trip_pickups_translation>(entity =>
        {
            entity.HasKey(e => e.id).HasName("trip_pickups_translations_pkey");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.created_by).HasMaxLength(100);
            entity.Property(e => e.lang_code).HasMaxLength(5);
            entity.Property(e => e.pickup_name).HasMaxLength(100);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<trip_price>(entity =>
        {
            entity.HasKey(e => e.id).HasName("trip_prices_pkey");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.created_by).HasMaxLength(100);
            entity.Property(e => e.currency_code).HasMaxLength(5);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<trip_translation>(entity =>
        {
            entity.HasKey(e => e.id).HasName("trip_translations_pkey");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.created_by).HasMaxLength(100);
            entity.Property(e => e.lang_code).HasMaxLength(5);
            entity.Property(e => e.trip_name).HasMaxLength(50);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<trips_booking>(entity =>
        {
            entity.HasKey(e => e.id).HasName("trips_booking_pkey");

            entity.ToTable("trips_booking");

            entity.Property(e => e.booking_code).HasMaxLength(50);
            entity.Property(e => e.booking_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.booking_status).HasComment("1 = requested\n2 = confirmed\n3 = canceled");
            entity.Property(e => e.client_email).HasMaxLength(100);
            entity.Property(e => e.client_id).HasMaxLength(100);
            entity.Property(e => e.pickup_time).HasMaxLength(20);
            entity.Property(e => e.trip_code).HasMaxLength(20);
            entity.Property(e => e.trip_date).HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<trips_wishlist>(entity =>
        {
            entity.HasKey(e => e.id).HasName("trips_wishlist_pkey");

            entity.ToTable("trips_wishlist");

            entity.Property(e => e.client_id).HasColumnType("character varying");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.trip_type).HasComment("1 = exercusion trip\n2 = transfer trip");
        });

        modelBuilder.Entity<tripwithdetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("tripwithdetails");

            entity.Property(e => e.client_id).HasColumnType("character varying");
            entity.Property(e => e.country_code).HasMaxLength(20);
            entity.Property(e => e.currency_code).HasMaxLength(5);
            entity.Property(e => e.default_img).HasMaxLength(50);
            entity.Property(e => e.dest_code).HasMaxLength(20);
            entity.Property(e => e.dest_route).HasMaxLength(100);
            entity.Property(e => e.lang_code).HasMaxLength(5);
            entity.Property(e => e.pickup).HasMaxLength(20);
            entity.Property(e => e.route).HasMaxLength(100);
            entity.Property(e => e.trip_code).HasMaxLength(20);
            entity.Property(e => e.trip_default_name).HasMaxLength(50);
            entity.Property(e => e.trip_duration).HasMaxLength(20);
            entity.Property(e => e.trip_name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
