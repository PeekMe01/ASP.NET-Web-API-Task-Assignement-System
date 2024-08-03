using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using WebApplication1.Data;
using WebApplication1.Models;
namespace WebApplication1.Controllers
{
    public class CoursesUsersController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public CoursesUsersController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpPost("AddStudents")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> AddStudentsToCourse(int courseId, List<int> studentIds)
        {
            var course = await _appDbContext.Course
                .Include(c => c.CoursesUsers)
                .FirstOrDefaultAsync(c => c.courseid == courseId);

            if (course == null)
                return NotFound("Course not found");

            foreach (var studentId in studentIds)
            {
                if (course.CoursesUsers.Any(cu => cu.userid == studentId))
                    continue;
                var student = await _appDbContext.User.FindAsync(studentId);
                if (student != null && student.priviledge == "0")
                {
                    course.CoursesUsers.Add(new CoursesUsers { courseid = courseId, userid = studentId });
                }
            }

            await _appDbContext.SaveChangesAsync();

            return Ok("Students added to the course successfully");
        }
        /*
          const response = await axios.post(`http://your-api-url/api/Course/${courseId}/AddStudents`, studentIds);
          console.log(response.data);*/

        [HttpGet("GetCoursesFromUser/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetCoursesFromUser(int userId)
        {
            try
            {
                // Retrieve the user by ID
                var user = await _appDbContext.User.FindAsync(userId);

                if (user == null)
                    return NotFound("User not found");

                // Retrieve courses associated with the user
                var courses = await _appDbContext.CoursesUsers
                    .Where(cu => cu.userid == userId)
                    .Select(cu => cu.Course)
                    .ToListAsync();

                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("GetStudentsFromCourse/{courseId}")]
        [Authorize]
        public async Task<IActionResult> GetStudentsFromCourse(int courseId)
        {
            try
            {
                // Retrieve the course by ID
                var course = await _appDbContext.Course.FindAsync(courseId);

                if (course == null)
                    return NotFound("Course not found");

                // Retrieve students enrolled in the course
                var students = await _appDbContext.CoursesUsers
                    .Where(cu => cu.courseid == courseId)
                    .Select(cu => cu.User)
                    .ToListAsync();

                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("RemoveStudentFromCourse")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> RemoveStudentFromCourse(int courseId, int userId)
        {
            try
            {
                // Retrieve the association between the course and the student
                var courseUser = await _appDbContext.CoursesUsers
                    .FirstOrDefaultAsync(cu => cu.courseid == courseId && cu.userid == userId);

                if (courseUser == null)
                    return NotFound("Association between course and student not found");

                // Remove the association from the database
                _appDbContext.CoursesUsers.Remove(courseUser);
                await _appDbContext.SaveChangesAsync();

                return Ok("Student removed from the course successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
