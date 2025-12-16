using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SchoolModel.Data.Models
{
    public class CourseEntityTypeConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("Course");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.HasOne(d => d.Department)
                .WithMany(p => p.Courses)
                .HasForeignKey(x => x.DepartmentId);
            builder.HasIndex(e => new { e.CourseNumber, e.DepartmentId })
                   .IsUnique();
        }
    }
}
