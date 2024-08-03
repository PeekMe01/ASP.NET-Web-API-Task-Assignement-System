using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WebApplication1.Models;
using System.Collections.Generic;
namespace WebApplication1.Data
{
    public class TasksSeeders
    {
        private readonly AppDbContext _context;

        public TasksSeeders(AppDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            // Check if Users table is already populated
            if (!_context.Tasks.Any())
            {

                // Add sample user dataa
                var tasksToAdd = new List<Tasks>
                {
                        new Tasks ("task1","this is the description of task 1",DateTime.Now,1 ),
                        new Tasks ("task1","this is the description of task 1",DateTime.Now,2 ),
                       new Tasks ("task1","this is the description of task 1",DateTime.Now,3 ),
                         new Tasks ("task2","this is the description of task 2",DateTime.Now,1 ),
                        new Tasks ("task3","this is the description of task 3",DateTime.Now,1 ),
                         new Tasks ("task2","this is the description of task 2",DateTime.Now,2 ),
                        new Tasks ("task1","this is the description of task 1",DateTime.Now,4 ),
                };

                // Add users to the Users table
                _context.Tasks.AddRange(tasksToAdd);
                _context.SaveChanges();
            }
        }
    }
}
