using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Lib.Models
{
    public partial class PRN231_ProjectContext : DbContext
    {
        public PRN231_ProjectContext()
        {
        }

        public PRN231_ProjectContext(DbContextOptions<PRN231_ProjectContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bill> Bills { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<CourseAttempt> CourseAttempts { get; set; } = null!;
        public virtual DbSet<Option> Options { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=TUANNM\\SQLEXPRESS; database=PRN231_Project;user=sa;password=12345;Integrated Security=true;TrustServerCertificate=Yes");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("bills");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TotalPayment)
                    .HasColumnType("money")
                    .HasColumnName("total_payment");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bills_users");

                entity.HasMany(d => d.Courses)
                    .WithMany(p => p.Bills)
                    .UsingEntity<Dictionary<string, object>>(
                        "BillDetail",
                        l => l.HasOne<Course>().WithMany().HasForeignKey("CourseId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_bill_details_courses"),
                        r => r.HasOne<Bill>().WithMany().HasForeignKey("BillId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_bill_details_bills"),
                        j =>
                        {
                            j.HasKey("BillId", "CourseId");

                            j.ToTable("bill_details");

                            j.IndexerProperty<int>("BillId").HasColumnName("bill_id");

                            j.IndexerProperty<int>("CourseId").HasColumnName("course_id");
                        });
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(255)
                    .HasColumnName("category_name")
                    .IsFixedLength();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description")
                    .IsFixedLength();
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("courses");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Category).HasColumnName("category");

                entity.Property(e => e.CourseName)
                    .HasMaxLength(255)
                    .HasColumnName("course_name");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("date")
                    .HasColumnName("created_at");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.Image)
                    .HasColumnType("image")
                    .HasColumnName("image");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((10000))");

                entity.Property(e => e.Publish).HasColumnName("publish");

                entity.Property(e => e.TotalJoined).HasColumnName("total_joined");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.Category)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__courses__categor__48CFD27E");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__courses__created__49C3F6B7");
            });

            modelBuilder.Entity<CourseAttempt>(entity =>
            {
                entity.ToTable("course_attempts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AttemptDate)
                    .HasColumnType("date")
                    .HasColumnName("attempt_date");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.CourseAttempts)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_course_attempts_courses");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CourseAttempts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_course_attempts_users");
            });

            modelBuilder.Entity<Option>(entity =>
            {
                entity.ToTable("options");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IsCorrect).HasColumnName("isCorrect");

                entity.Property(e => e.OptionText)
                    .HasMaxLength(500)
                    .HasColumnName("option_text");

                entity.Property(e => e.QuestionId).HasColumnName("question_id");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Options)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__options__questio__4AB81AF0");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("questions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Course).HasColumnName("course");

                entity.Property(e => e.QuestionText)
                    .HasMaxLength(1000)
                    .HasColumnName("question_text");

                entity.HasOne(d => d.CourseNavigation)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.Course)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__questions__cours__4BAC3F29");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("date")
                    .HasColumnName("created");

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(255)
                    .HasColumnName("display_name")
                    .IsFixedLength();

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email")
                    .IsFixedLength();

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password")
                    .IsFixedLength();

                entity.Property(e => e.Role).HasColumnName("role");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.Username)
                    .HasMaxLength(255)
                    .HasColumnName("username")
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
