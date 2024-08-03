using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;
namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        // dependency injection
        private IEmailNotification _emailNotificationService;

        private readonly AppDbContext _appDbContext;
        public TasksController(AppDbContext appDbContext, IEmailNotification emailNotificationService)
        {
            _appDbContext = appDbContext;
            _emailNotificationService = emailNotificationService;
        }
        [HttpPost("{courseId}/AddTask")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> AddTaskToCourse(string taskname, string taskdescription,DateTime duedate ,int courseId)
        {
            try
            {
                var course = await _appDbContext.Course.FindAsync(courseId);

                if (course == null)
                    return NotFound("Course not found");
                Tasks tasks = new Tasks(taskname, taskdescription, duedate, courseId);
                _appDbContext.Tasks.Add(tasks);
                await _appDbContext.SaveChangesAsync();

                // Retrieve students enrolled in the course
                var students = await _appDbContext.CoursesUsers
                    .Where(cu => cu.courseid == courseId)
                    .Select(cu => cu.User)
                    .ToListAsync();

                // Collect email addresses
                var emailAddresses = students.Select(u => u.email).ToList();

                // Compose email message
                string subject = "New Task Added to Course";
                string body = $"A new task '{taskname}' has been added to the course '{course.coursename}'.\n\nDescription: {taskdescription}\nDue Date: {duedate}";

                // Send email to each recipient
                foreach (var emailAddress in emailAddresses)
                {
                    _emailNotificationService.SendEmail(emailAddress, subject, body);
                }

                return Ok("task created ");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, returning a BadRequest with the exception message
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{taskId}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> RemoveTask(int taskId)
        {
            try
            {
                var task = await _appDbContext.Tasks.FindAsync(taskId);

                if (task == null)
                    return NotFound("Task not found");

                _appDbContext.Tasks.Remove(task);
                await _appDbContext.SaveChangesAsync();

                return Ok("Task deleted successfully");
            }
            catch (Exception ex)
            {
              
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("GetTasksFromCourse/{courseId}")]
        [Authorize]
        public async Task<IActionResult> GetTasksFromCourse(int courseId)
        {
            try
            {
                var course = await _appDbContext.Course.FindAsync(courseId);
                if (course == null)
                    return NotFound("Course not found");

                // Retrieve tasks associated with the course
                var tasks = await _appDbContext.Tasks
                    .Where(t => t.courseid == courseId)
                    .Select(t => new
                    {
                        t.taskid,
                        t.taskname,
                        t.taskdescription,
                        t.duedate,
                        t.courseid
                    })
                    .ToListAsync();

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateTaskName/{taskId}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateTaskName(int taskId, string newTaskName)
        {
            var task = await _appDbContext.Tasks.FindAsync(taskId);

            if (task == null)
            {
                return NotFound("Task not found");
            }

            task.taskname = newTaskName;
            _appDbContext.Update(task);
            await _appDbContext.SaveChangesAsync();

            return Ok("Task name updated successfully");
        }
    }
}
