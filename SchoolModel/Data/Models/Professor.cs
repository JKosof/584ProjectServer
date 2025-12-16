using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolModel.Data.Models
{
    [Table("Professor")]
    public partial class Professor
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("departmentId")]
        public int DepartmentId { get; set; }

        [Column("firstName")]
        [StringLength(50)]
        [Unicode(false)]
        public string FirstName { get; set; } = null!;

        [Column("lastName")]
        [StringLength(50)]
        [Unicode(false)]
        public string LastName { get; set; } = null!;

        [Column("partTime")]
        public bool PartTime { get; set; }

        [Column("workloadStatus")]
        [StringLength(20)]
        [Unicode(false)]
        public string? WorkloadStatus { get; set; } = null!;


        [ForeignKey("DepartmentId")]
        [InverseProperty("Professors")]
        public virtual Department Department { get; set; } = null!;

        public virtual ICollection<Course>? Courses { get; set; }
    }
}
