using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SchoolModel.Data.Models;

namespace SchoolModel.Data
{
    public partial class Project584DbContext : IdentityDbContext<SchoolModelUser>
    {
        public Project584DbContext()
        {
        }

        public Project584DbContext(DbContextOptions<Project584DbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Course> Courses { get; set; }

        public virtual DbSet<Department> Departments { get; set; }

        //public virtual DbSet<ProfessorsCourses> ProfessorsCourses { get; set; }

        public virtual DbSet<Professor> Professors { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddJsonFile("appsettings.Development.json", optional: true);
            IConfigurationRoot configuration = builder.Build();
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // add the EntityTypeConfiguration classes
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(Project584DbContext).Assembly
                );

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasOne(d => d.Department).WithMany(p => p.Courses)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Course_Department");
                entity.HasIndex(e => new {e.CourseNumber, e.DepartmentId}).IsUnique();
                entity.HasMany(p => p.Professors)
                .WithMany(c => c.Courses)
                .UsingEntity(j => j.ToTable("ProfessorsCourses"));
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasOne(d => d.Department).WithMany(p => p.Professors)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Professor_Department");
                entity.HasMany(c => c.Courses)
                .WithMany(p => p.Professors)
                .UsingEntity(j => j.ToTable("ProfessorsCourses"));
            });

            modelBuilder.Entity<Department>(entity => {
                entity.Property(e => e.Name).IsUnicode(false);
                entity.Property(e => e.DeptCode).IsUnicode(false);
                entity.Property(e => e.DeptCode).IsFixedLength();
            });

            /*modelBuilder.Entity<ProfessorsCourses>(entity =>
            {
                entity.HasKey(e => new { e.ProfessorId, e.CourseId });
                entity.HasOne(d => d.Professor).WithMany(p => p.ProfessorsCourses)
                    .HasForeignKey(d => d.ProfessorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProfessorsCourses_Professor");
                entity.HasOne(d => d.Course).WithMany(p => p.ProfessorsCourses)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProfessorsCourses_Course");
            });
            OnModelCreatingPartial(modelBuilder);*/
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
