using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WebApplication1.Models;
using System.Collections.Generic;
namespace WebApplication1.Data
{
    public class CourseSeeder
    {
        private readonly AppDbContext _context;

        public CourseSeeder(AppDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            // Check if Users table is already populated
            if (!_context.Course.Any())
            {

                // Add sample user dataa
                var coursesToAdd = new List<Course>
                {
                        new Course ("database design",6 ),
                        new Course ("data analysis",6 ),
                        new Course ("computer security",6 ),
                        new Course ("advanced programming",5 ),
                        new Course ("data structure",5 ),
                        new Course ("cloud computing",5 ),
                        new Course ("operating system",5 )
                };

                // Add users to the Users table
                _context.Course.AddRange(coursesToAdd);
                _context.SaveChanges();
            }
        }
    }
}
