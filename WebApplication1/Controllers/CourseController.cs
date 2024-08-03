using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;
namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public CourseController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        [HttpPost]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> AddCourse(string coursename,int instructorid)
        {
           var priviledge = await _appDbContext.User
                            .Where(u => u.userid == instructorid)
                            .Select(u => u.priviledge)
                            .FirstOrDefaultAsync();

            if (priviledge == null)
            { 
                return BadRequest("Instructor not found.");
            }
            if (priviledge == "0" )
            {
                return BadRequest("student cannot  add course.");
            }

            Course course = new Course(coursename, instructorid);
            _appDbContext.Course.Add(course);
            await _appDbContext.SaveChangesAsync();

            return Ok(course);
        }
       
        [HttpGet("GetCoursesByInstructorId/{instructorId}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetCoursesByInstructorId(int instructorId)
        {
            var courses = await _appDbContext.Course
                .Where(c => c.instructorid == instructorId)
                .ToListAsync();

            return Ok(courses);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _appDbContext.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _appDbContext.Course.Remove(course);
            await _appDbContext.SaveChangesAsync();

            return Ok("Course Deleted");
        }

        [HttpPut("UpdateCourseName/{courseId}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateCourseName(int courseId, string newCourseName)
        {
            var course = await _appDbContext.Course.FindAsync(courseId);

            if (course == null)
            {
                return NotFound("Course not found");
            }

            course.coursename = newCourseName;
            _appDbContext.Update(course);
            await _appDbContext.SaveChangesAsync();

            return Ok("Course name updated successfully");
        }


    }      
}
