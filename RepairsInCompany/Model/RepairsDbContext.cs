using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RepairsInCompany.Model;

public partial class RepairsDbContext : DbContext
{
    public RepairsDbContext()
    {
    }

    public RepairsDbContext(DbContextOptions<RepairsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Equipment> Equipment { get; set; }

    public virtual DbSet<EquipmentBreakDownDate> EquipmentBreakDownDates { get; set; }

    public virtual DbSet<EquipmentInRepair> EquipmentInRepairs { get; set; }

    public virtual DbSet<LastRepairDate> LastRepairDates { get; set; }

    public virtual DbSet<NextRepairDate> NextRepairDates { get; set; }

    public virtual DbSet<Registration> Registrations { get; set; }

    public virtual DbSet<Repair> Repairs { get; set; }

    public virtual DbSet<RepairFrequency> RepairFrequencies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-KOFUHBU;Database=RepairsDB;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.EquipmentId).HasName("Unique_Identifier1");

            entity.Property(e => e.EquipmentId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(16);
        });

        modelBuilder.Entity<EquipmentBreakDownDate>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("EquipmentBreakDownDates");

            entity.Property(e => e.EndDateTime).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(16);
            entity.Property(e => e.StartDateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<EquipmentInRepair>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("EquipmentInRepair");

            entity.Property(e => e.EquipmentInRepair1)
                .HasColumnType("datetime")
                .HasColumnName("EquipmentInRepair");
            entity.Property(e => e.Name).HasMaxLength(16);
        });

        modelBuilder.Entity<LastRepairDate>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("LastRepairDates");

            entity.Property(e => e.LastRepairDate1)
                .HasColumnType("datetime")
                .HasColumnName("LastRepairDate");
            entity.Property(e => e.Name).HasMaxLength(16);
        });

        modelBuilder.Entity<NextRepairDate>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("NextRepairDates");

            entity.Property(e => e.Name).HasMaxLength(16);
            entity.Property(e => e.NextRepairDate1)
                .HasColumnType("datetime")
                .HasColumnName("NextRepairDate");
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => new { e.StartDateTime, e.EquipmentId }).HasName("Unique_Identifier2");

            entity.ToTable("Registration");

            entity.Property(e => e.StartDateTime).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("date");

            entity.HasOne(d => d.Equipment).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.EquipmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Registered");
        });

        modelBuilder.Entity<Repair>(entity =>
        {
            entity.HasKey(e => new { e.StartDateTime, e.EquipmentId }).HasName("Unique_Identifier3");

            entity.ToTable("Repair");

            entity.Property(e => e.StartDateTime).HasColumnType("datetime");
            entity.Property(e => e.EndDateTime).HasColumnType("datetime");

            entity.HasOne(d => d.Equipment).WithMany(p => p.Repairs)
                .HasForeignKey(d => d.EquipmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Being Repaired");
        });

        modelBuilder.Entity<RepairFrequency>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("RepairFrequency");

            entity.Property(e => e.Name).HasMaxLength(16);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
