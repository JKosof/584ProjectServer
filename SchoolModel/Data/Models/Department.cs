using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolModel.Data.Models
{
    [Table("Department")]
    public partial class Department
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("deptName")]
        [StringLength(50)]
        [Unicode(false)]
        public string Name { get; set; } = null!;

        [Column("deptCode")]
        [StringLength(2)]
        [Unicode(false)]
        public string DeptCode { get; set; } = null!;

        [InverseProperty("Department")]
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

        [InverseProperty("Department")]
        public virtual ICollection<Professor> Professors { get; set; } = new List<Professor>();
    }
}
