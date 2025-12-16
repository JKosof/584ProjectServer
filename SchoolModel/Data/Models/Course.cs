using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolModel.Data.Models
{
    [Table("Course")]
    public partial class Course
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("departmentId")]
        public int DepartmentId { get; set; }

        [Column("courseName")]
        [StringLength(50)]
        [Unicode(false)]
        public string Name { get; set; } = null!;

        [Column("credits")]
        public int Credits { get; set; }

        [Column("courseNumber")]
        public int CourseNumber { get; set; }

        [Column("availability")]
        [StringLength(50)]
        [Unicode(false)]
        public string? Availability { get; set; }

        [ForeignKey("DepartmentId")]
        [InverseProperty("Courses")]
        public virtual Department Department { get; set; } = null!;

        public virtual ICollection<Professor>? Professors { get; set; }
    }
}
