using SchoolModel.Data.Models;

namespace _584ProjectServer.Server.DTOs
{
    public class ProfessorAdd
    {
        public required int DepartmentId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public required bool PartTime { get; set; }
        public string? WorkloadStatus { get; set; }
        public required int?[] Courses { get; set; }

        //public Department? Department { get; set; }
    }
}