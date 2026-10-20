using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QlsvApi.Models;

namespace QlsvApi.Data;

public partial class StudentDbContext : DbContext
{
    public StudentDbContext(DbContextOptions<StudentDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Major> Majors { get; set; }

    public virtual DbSet<Student> Students { get; set; }

// hàm này được gọi khi tạo các bảng trong DbContext
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Major>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Majors_Name").IsUnique();

            entity.Property(e => e.MajorCode).HasMaxLength(10);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(e => e.CardId, "UQ_Students_CardId").IsUnique();

            entity.Property(e => e.CardId).HasMaxLength(20);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Major).WithMany(p => p.Students)
                .HasForeignKey(d => d.MajorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
