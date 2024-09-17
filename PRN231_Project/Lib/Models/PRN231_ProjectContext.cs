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
        public virtual DbSet<BillDetail> BillDetails { get; set; } = null!;
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

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.TotalPayment)
                    .HasColumnType("money")
                    .HasColumnName("total_payment");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bills_users");
            });

            modelBuilder.Entity<BillDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("bill_details");

                entity.Property(e => e.BillId).HasColumnName("bill_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.HasOne(d => d.Bill)
                    .WithMany()
                    .HasForeignKey(d => d.BillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bill_details_bills");

                entity.HasOne(d => d.Course)
                    .WithMany()
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bill_details_courses");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(20)
                    .HasColumnName("category_name")
                    .IsFixedLength();

                entity.Property(e => e.Description)
                    .HasMaxLength(10)
                    .HasColumnName("description")
                    .IsFixedLength();
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("courses");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Category).HasColumnName("category");

                entity.Property(e => e.CourseName)
                    .HasMaxLength(20)
                    .HasColumnName("course_name");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("date")
                    .HasColumnName("created_at");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.Image)
                    .HasMaxLength(20)
                    .HasColumnName("image")
                    .IsFixedLength();

                entity.Property(e => e.Publish).HasColumnName("publish");

                entity.Property(e => e.TotalJoined).HasColumnName("total_joined");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.Category)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__courses__categor__3F466844");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__courses__created__3E52440B");
            });

            modelBuilder.Entity<CourseAttempt>(entity =>
            {
                entity.ToTable("course_attempts");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

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

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.OptionText)
                    .HasMaxLength(500)
                    .HasColumnName("option_text");

                entity.Property(e => e.QuestionId).HasColumnName("question_id");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.OptionsNavigation)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__options__questio__44FF419A");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("questions");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Course).HasColumnName("course");

                entity.Property(e => e.QuestionText)
                    .HasMaxLength(1000)
                    .HasColumnName("question_text");

                entity.HasOne(d => d.CourseNavigation)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.Course)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__questions__cours__4222D4EF");

                entity.HasMany(d => d.Options)
                    .WithMany(p => p.Questions)
                    .UsingEntity<Dictionary<string, object>>(
                        "CorrectOption",
                        l => l.HasOne<Option>().WithMany().HasForeignKey("OptionId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__correct_o__optio__48CFD27E"),
                        r => r.HasOne<Question>().WithMany().HasForeignKey("QuestionId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__correct_o__quest__47DBAE45"),
                        j =>
                        {
                            j.HasKey("QuestionId", "OptionId").HasName("PK__correct___818CB9A84BE97C85");

                            j.ToTable("correct_option");

                            j.IndexerProperty<int>("QuestionId").HasColumnName("question_id");

                            j.IndexerProperty<int>("OptionId").HasColumnName("option_id");
                        });
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("date")
                    .HasColumnName("created");

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(20)
                    .HasColumnName("display_name")
                    .IsFixedLength();

                entity.Property(e => e.Email)
                    .HasMaxLength(10)
                    .HasColumnName("email")
                    .IsFixedLength();

                entity.Property(e => e.Password)
                    .HasMaxLength(25)
                    .HasColumnName("password")
                    .IsFixedLength();

                entity.Property(e => e.Role).HasColumnName("role");

                entity.Property(e => e.Username)
                    .HasMaxLength(10)
                    .HasColumnName("username")
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
