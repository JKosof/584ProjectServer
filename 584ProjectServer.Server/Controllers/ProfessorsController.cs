using _584ProjectServer.Server.DTOs;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolModel.Data;
using SchoolModel.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _584ProjectServer.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessorsController : ControllerBase
    {
        private readonly Project584DbContext _context;

        public ProfessorsController(Project584DbContext context)
        {
            _context = context;
        }

        // GET: api/Professors
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Professor>>> GetProfessors()
        {
            return await _context.Professors
                .Include(p => p.Department)
                .Select(p => new Professor
                {
                    Id = p.Id,
                    DepartmentId = p.DepartmentId,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    PartTime = p.PartTime,
                    WorkloadStatus = p.WorkloadStatus,
                    Department = new Department
                    {
                        Id = p.Department.Id,
                        Name = p.Department.Name
                    }
                })
                .ToListAsync();
        }

        // GET: api/Professors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProfessorUpdate>> GetProfessor(int id)
        {
            var professor = await _context.Professors
                .Include(p => p.Department)
                .Where(p => p.Id == id)
                .Select(p => new ProfessorUpdate
                {
                    Id = p.Id,
                    DepartmentId = p.DepartmentId,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    PartTime = p.PartTime,
                    WorkloadStatus = p.WorkloadStatus,
                    Courses = (p.Courses != null ? p.Courses.Select(c => (int?)c.Id).ToArray() : Array.Empty<int?>()),
                    Department = new Department
                    {
                        Id = p.Department.Id,
                        Name = p.Department.Name,
                        DeptCode = p.Department.DeptCode
                    }
                })
                .SingleOrDefaultAsync();

            if (professor == null)
            {
                return NotFound();
            }

            return professor ;
        }

        // PUT: api/Professors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProfessor(int id, ProfessorUpdate professorDto)
        {
            if (id != professorDto.Id)
            {
                return BadRequest();
            }

            Professor? professor = await _context.Professors
                .Include(p => p.Courses)
                .FirstOrDefaultAsync(p => p.Id == id);

            //_context.Entry(professorDto).State = EntityState.Modified;
            professor.DepartmentId = professorDto.DepartmentId;
            professor.FirstName = professorDto.FirstName;
            professor.LastName = professorDto.LastName;
            professor.PartTime = professorDto.PartTime;
            professor.WorkloadStatus = professorDto.WorkloadStatus;
            professor.Courses.Clear();

            if (professorDto.Courses != null)
            {

                var selectedCourses = await _context.Courses
                    .Where(c => professorDto.Courses != null && professorDto.Courses.Contains(c.Id))
                    .ToListAsync();

                
                foreach (var course in selectedCourses)
                {
                    _context.Courses.Attach(course);
                    professor.Courses.Add(course);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfessorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPut("bulkupdate")]
        [Authorize]
        public async Task<IActionResult> BulkUpdate()
        {
            var professors = await _context.Professors.Include(professor => professor.Courses).ToListAsync();
            //update all professors' workload status based on total credits of their courses
            //if professor is part-time, their workload counts as one tier higher
            foreach (var professor in professors)
            {
                int totalCredits = professor.Courses?.Sum(c => c.Credits) ?? 0;
                if (totalCredits <= 6)
                {
                    professor.WorkloadStatus = professor.PartTime ? "Heavy Schedule" : "Light Schedule";
                }
                else if (totalCredits <= 12)
                {
                    professor.WorkloadStatus = professor.PartTime ? "Burnout!" : "Heavy Schedule";
                }
                else
                {
                    professor.WorkloadStatus = "Burnout!";
                }
            }
            //await _context.SaveChangesAsync();

            var courses = await _context.Courses.Include(course => course.Professors).ToListAsync();
            //update all courses' availability based on total workload of their professors relative to credits
            //workload counts for double if professor is in same department as course
            foreach (var course in courses)
            {
                int totalWorkPower = 0;
                if (course.Professors != null)
                {
                    foreach (var prof in course.Professors)
                    {
                        if (prof.DepartmentId == course.DepartmentId)
                        {
                            if (prof.WorkloadStatus == "Light Schedule")
                            {
                                totalWorkPower += 4;
                            }
                            else if (prof.WorkloadStatus == "Heavy Schedule")
                            {
                                totalWorkPower += 2;
                            }
                            else if (prof.WorkloadStatus == "Burnout!")
                            {
                                totalWorkPower += 0;
                            }
                        }
                        else
                        {
                            if (prof.WorkloadStatus == "Light Schedule")
                            {
                                totalWorkPower += 2;
                            }
                            else if (prof.WorkloadStatus == "Heavy Schedule")
                            {
                                totalWorkPower += 1;
                            }
                            else if (prof.WorkloadStatus == "Burnout!")
                            {
                                totalWorkPower += 0;
                            }
                        }
                    }
                    if (totalWorkPower == 0)
                    {
                        course.Availability = "Unavailable";
                    }
                    else if (totalWorkPower <= course.Credits)
                    {
                        course.Availability = "Rare";
                    }
                    else if (totalWorkPower <= course.Credits * 2)
                    {
                        course.Availability = "Limited";
                    }
                    else
                    {
                        course.Availability = "Full";
                    }
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        // POST: api/Professors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Professor>> PostProfessor(ProfessorAdd professorDto)
        {
            Professor professor = new Professor
            {
                FirstName = professorDto.FirstName,
                LastName = professorDto.LastName,
                PartTime = professorDto.PartTime,
                WorkloadStatus = professorDto.WorkloadStatus,
                DepartmentId = professorDto.DepartmentId,
                Courses = new List<Course>()
            };

            if (professorDto.Courses != null)
            {

                var selectedCourses = await _context.Courses
                    .Where(c => professorDto.Courses != null && professorDto.Courses.Contains(c.Id))
                    .ToListAsync();


                foreach (var course in selectedCourses)
                {
                    professor.Courses.Add(course);
                }
            }

            _context.Professors.Add(professor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProfessor", new { id = professor.Id }, professorDto);
        }

        // DELETE: api/Professors/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProfessor(int id)
        {
            var professor = await _context.Professors.FindAsync(id);
            if (professor == null)
            {
                return NotFound();
            }

            _context.Professors.Remove(professor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProfessorExists(int id)
        {
            return _context.Professors.Any(e => e.Id == id);
        }
    }
}
