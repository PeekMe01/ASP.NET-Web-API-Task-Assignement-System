using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System.Buffers.Text;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // dependency injection
        private INotificationService _notificationService;
        private IEmailNotification _emailNotificationService;

        private readonly AppDbContext _appDbContext;

        private IConfiguration _config;

        public UserController(AppDbContext appDbContext, INotificationService notificationService, IConfiguration config, IEmailNotification emailNotificationService)
        {
            _appDbContext = appDbContext;
            _notificationService = notificationService;
            _config = config;
            _emailNotificationService = emailNotificationService;
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddUser(string firstname,string lastname,string email,string password)
        {
            User user = new User(firstname, lastname, email, password);
            user.priviledge="0";
            _appDbContext.User.Add(user);
           await _appDbContext.SaveChangesAsync();

            return Ok("user added successfully");
        }

        [HttpPost("AddInstructor")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> AddInstructor(string firstname, string lastname, string email, string password)
        {
            User user = new User(firstname, lastname, email, password);
            user.priviledge = "1"; 
            _appDbContext.User.Add(user);
            await _appDbContext.SaveChangesAsync();

            // Compose email message
            string subject = "New Instructor Account";
            //string body = $"You credentials are: \nFirst Name: '{user.firstname}'\nLast Name: '{user.lastname}'\nEmail: '{user.email}'\nPassword: '{user.password}'\n\nClick <a href='http://localhost:3000/'>here</a> to login to your new instructor account.";
            string body = $"<p>Your credentials are:</p>" +
                          $"<p>First Name: '{user.firstname}'</p>" +
                          $"<p>Last Name: '{user.lastname}'</p>" +
                          $"<p>Email: '{user.email}'</p>" +
                          $"<p>Password: '{user.password}'</p>" +
                          $"<p>Click <a href='http://localhost:3000/'>here</a> to login to your new instructor account.</p>";

            _emailNotificationService.SendEmail(user.email, subject, body);

            return Ok("instructor added successfully");
        }
       
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email,string password)
        {
          
            var user = await _appDbContext.User.FirstOrDefaultAsync(u => u.email == email);

            if (user == null)
                return Ok("incorrect email/password");

            if (user.password != password)
                return Ok("incorrect email/password");

            var token = Generate(user);
            string role = "";
            switch (user.priviledge)
            {
                case "0":
                    role = "user";
                    break;
                case "1":
                    role = "instructor";
                    break;
                case "2":
                    role = "admin";
                    break;
            }
            var response = new
            {
                message = $"Logged in as {role}",
                user=user,
                token = token
            };
            return Ok(response);
        }

        private string Generate(User user)
        {
            Console.WriteLine(_config["Jwt:Key"]);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.firstname),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.GivenName, user.password),
                new Claim(ClaimTypes.Surname, user.lastname),
                new Claim(ClaimTypes.Role, user.priviledge)
            };

            Console.WriteLine(claims);
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.UtcNow,
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var users = await _appDbContext.User.ToListAsync();
            return Ok(users);
        }

        [HttpGet("GetStudents")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetStudents()
        {
            var users = await _appDbContext.User
                .Where(u => u.priviledge == "0")
                .ToListAsync();

            return Ok(users);
        }

        //Authentication and Authorization
        [HttpGet("GetInstructors")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> GetInstructors()
        {
            var instructors = await _appDbContext.User
                .Where(u => u.priviledge == "1")
                .ToListAsync();

            return Ok(instructors);
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> RemoveInstructor(int userId)
        {
            try
            {
                var user = await _appDbContext.User.FindAsync(userId);

                if (user == null)
                    return NotFound("User not found");

                _appDbContext.User.Remove(user);
                await _appDbContext.SaveChangesAsync();

                return Ok("User deleted successfully");
            }
            catch (Exception ex)
            {

                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
        [HttpPut("UpdateUsername")]
        [Authorize]
        public async Task<IActionResult> UpdateUserFirstName(string newFirstName)
        {
            // Retrieve the user's email from the JWT token claims
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            // Retrieve the user from the database based on the email
            var user = await _appDbContext.User.FirstOrDefaultAsync(u => u.email == email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.firstname = newFirstName;
            _appDbContext.Update(user);
            await _appDbContext.SaveChangesAsync();

            // using the dep inj here:
            _notificationService.NotifyUsernameChanged(user);

            return Ok("User first name updated successfully");
        }

    }
}
