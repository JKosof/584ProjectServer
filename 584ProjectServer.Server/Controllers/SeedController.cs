using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Collections.Generic;
using SchoolModel;
using SchoolModel.Data;
using _584ProjectServer.Server.Data;
using SchoolModel.Data.Models;

namespace _584ProjectServer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController(Project584DbContext context, IHostEnvironment environment,
        RoleManager<IdentityRole> roleManager, UserManager<SchoolModelUser> userManager, IConfiguration configuration) : ControllerBase
    {
        string _pathName = Path.Combine(environment.ContentRootPath, "Data/Courses.csv");
        string _pathName2 = Path.Combine(environment.ContentRootPath, "Data/Professors.csv");
        [HttpPost("Department")]
        public async Task<ActionResult> PostDepartments()
        {
            Dictionary<string, Department> departments = await context.Departments.AsNoTracking()
                .ToDictionaryAsync(d => d.Name, StringComparer.OrdinalIgnoreCase);
            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null
            };
            using StreamReader reader = new(_pathName);
            using CsvReader csv = new(reader, config);
            List<CourseCsv> records = csv.GetRecords<CourseCsv>().ToList();
            foreach (CourseCsv record in records)
            {
                if (!departments.ContainsKey(record.deptName))
                {
                    Department newDepartment = new()
                    {
                        Name = record.deptName,
                        DeptCode = record.deptCode
                    };
                    departments.Add(newDepartment.Name, newDepartment);
                    await context.Departments.AddAsync(newDepartment);
                }
            }
            await context.SaveChangesAsync();

            Dictionary<string, Department> departments2 = await context.Departments.AsNoTracking()
                .ToDictionaryAsync(d => d.Name, StringComparer.OrdinalIgnoreCase);
            CsvConfiguration config2 = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null
            };
            using StreamReader reader2 = new(_pathName2);
            using CsvReader csv2 = new(reader2, config2);
            List<ProfessorCsv> records2 = csv2.GetRecords<ProfessorCsv>().ToList();
            foreach (ProfessorCsv record in records2)
            {
                if (!departments2.ContainsKey(record.deptName))
                {
                    Department newDepartment = new()
                    {
                        Name = record.deptName,
                        DeptCode = record.deptCode
                    };
                    departments2.Add(newDepartment.Name, newDepartment);
                    await context.Departments.AddAsync(newDepartment);
                }
            }
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Courses")]
        public async Task<ActionResult> PostCourses()
        {
            Dictionary<string, Department> departments = await context.Departments.AsNoTracking()
                .ToDictionaryAsync(d => d.Name, StringComparer.OrdinalIgnoreCase);
            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null
            };
            using StreamReader reader = new(_pathName);
            using CsvReader csv = new(reader, config);
            List<CourseCsv> records = csv.GetRecords<CourseCsv>().ToList();
            int count = 0;
            foreach (CourseCsv record in records)
            {
                if (departments.TryGetValue(record.deptName, out Department? dept))
                {
                    Course newCourse = new()
                    {
                        Name = record.courseName,
                        Credits = record.credits,
                        CourseNumber = record.courseNumber,
                        DepartmentId = dept.Id
                    };
                    await context.Courses.AddAsync(newCourse);
                    count++;
                }
            }
            await context.SaveChangesAsync();
            return new JsonResult(count);
        }

        [HttpPost("Professors")]
        public async Task<ActionResult> PostProfessors()
        {
            Dictionary<string, Department> departments = await context.Departments.AsNoTracking()
                .ToDictionaryAsync(d => d.Name, StringComparer.OrdinalIgnoreCase);
            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null
            };
            using StreamReader reader = new(_pathName2);
            using CsvReader csv = new(reader, config);
            List<ProfessorCsv> records = csv.GetRecords<ProfessorCsv>().ToList();
            int count = 0;
            foreach (ProfessorCsv record in records)
            {
                if (departments.TryGetValue(record.deptName, out Department? dept))
                {
                    Professor newProfessor = new()
                    {
                        FirstName = record.firstName,
                        LastName = record.lastName,
                        PartTime = (record.partTime == 1),
                        WorkloadStatus = null,
                        DepartmentId = dept.Id
                    };
                    await context.Professors.AddAsync(newProfessor);
                    count++;
                }
            }
            await context.SaveChangesAsync();
            return new JsonResult(count);
        }

        [HttpPost("Users")]
        public async Task<ActionResult> PostUsers()
        {
            string administrator = "admin";
            string registeredUser = "user";
            if (!await roleManager.RoleExistsAsync(administrator))
            {
                await roleManager.CreateAsync(new IdentityRole(administrator));
            }
            if (!await roleManager.RoleExistsAsync(registeredUser))
            {
                await roleManager.CreateAsync(new IdentityRole(registeredUser));
            }

            SchoolModelUser adminUser = new()
            {
                UserName = "admin",
                Email = "joshkosof@gmail.com",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            await userManager.CreateAsync(adminUser, configuration["DefaultPasswords:admin"]!);
            await userManager.AddToRoleAsync(adminUser, administrator);

            SchoolModelUser normalUser = new()
            {
                UserName = "user",
                Email = "joshkosof@aol.com",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            await userManager.CreateAsync(normalUser, configuration["DefaultPasswords:user"]!);
            await userManager.AddToRoleAsync(normalUser, registeredUser);
            return Ok();
        }
    }
}