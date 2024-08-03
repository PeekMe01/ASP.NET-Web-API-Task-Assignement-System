using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingTaskController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<TrackingTaskController> _logger;
        public TrackingTaskController(AppDbContext appDbContext, ILogger<TrackingTaskController> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }
        [HttpGet("GetUsersByTask/{taskId}")]
        [Authorize(Roles = "1,0")]
        public async Task<IActionResult> GetUsersByTask(int taskId)
        {
            try
            {
                // Retrieve all users who submitted the task
                    var users = await _appDbContext.TrackingTask
                                 .Where(tt => tt.taskid == taskId)
                                 .Select(tt => new
                                 {
                                     User = tt.User,
                                     Content = tt.content
                                 })
                                 .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("SubmitTask")]
        [Authorize(Roles = "0")]
        public async Task<IActionResult> SubmitTask(int userId, int taskId,string content)
        {
            try
            {
                // Check if the user exists
                var user = await _appDbContext.User.FindAsync(userId);
                if (user == null)
                    return NotFound("User not found");
                if (user.priviledge != "0")
                {
                    return BadRequest("instructor cannot  submit task.");
                }
                // Check if the task exists
                var task = await _appDbContext.Tasks.FindAsync(taskId);
                if (task == null)
                    return NotFound("Task not found");

                // Create a new TrackingTask entry to represent the submission
                var trackingTask = new TrackingTask { userid = userId, taskid = taskId ,content=content};
                _appDbContext.TrackingTask.Add(trackingTask);
                await _appDbContext.SaveChangesAsync();

                return Ok("Task submitted successfully");
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "An error occurred while saving the entity changes");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("RemoveTaskSubmission")]
        [Authorize(Roles = "0")]
        public async Task<IActionResult> RemoveTaskSubmission(int userId, int taskId)
        {
            try
            {
                var user = await _appDbContext.User.FindAsync(userId);
                if (user == null)
                    return NotFound("User not found");
                var task = await _appDbContext.Tasks.FindAsync(taskId);
                if (task == null)
                    return NotFound("Task not found");
                var trackingTask = await _appDbContext.TrackingTask.FirstOrDefaultAsync(tt => tt.userid == userId && tt.taskid == taskId);
                if (trackingTask == null)
                    return NotFound("Task submission not found for the user");
                _appDbContext.TrackingTask.Remove(trackingTask);
                await _appDbContext.SaveChangesAsync();
                return Ok("Task submission removed successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
