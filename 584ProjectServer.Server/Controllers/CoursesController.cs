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
    public class CoursesController : ControllerBase
    {
        private readonly Project584DbContext _context;

        public CoursesController(Project584DbContext context)
        {
            _context = context;
        }

        // GET: api/Courses
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            return await _context.Courses
                .Include(c => c.Department)
                .Select(c => new Course
                {
                    Id = c.Id,
                    DepartmentId = c.DepartmentId,
                    Name = c.Name,
                    Credits = c.Credits,
                    CourseNumber = c.CourseNumber,
                    Availability = c.Availability,
                    Department = new Department
                    {
                        Id = c.Department.Id,
                        Name = c.Department.Name,
                        DeptCode = c.Department.DeptCode
                    }
                })
                .ToListAsync();
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Department)
                .Where(c => c.Id == id)
                .Select(c => new Course
                {
                    Id = c.Id,
                    DepartmentId = c.DepartmentId,
                    Name = c.Name,
                    Credits = c.Credits,
                    CourseNumber = c.CourseNumber,
                    Availability = c.Availability,
                    Department = new Department
                    {
                        Id = c.Department.Id,
                        Name = c.Department.Name,
                        DeptCode = c.Department.DeptCode
                    }
                })
                .SingleOrDefaultAsync();

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }

        // PUT: api/Courses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCourse(int id, Course course)
        {
            if (id != course.Id)
            {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
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

        // POST: api/Courses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Course>> PostCourse(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourse", new { id = course.Id }, course);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
