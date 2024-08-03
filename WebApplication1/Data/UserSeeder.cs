using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WebApplication1.Models;
using System.Collections.Generic;
namespace WebApplication1.Data
{
    public class UserSeeder
    {
        private readonly AppDbContext _context;

        public UserSeeder(AppDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            // Check if Users table is already populated
            if (!_context.User.Any())
            {
                
                // Add sample user dataa
                var usersToAdd = new List<User>
                {
                        new User ( "antoine", "kamel","antoinekamel@gmail.com","antoine","0" ),
                        new User (  "ralph",  "daher", "ralphdaher@gmail.com","ralph", "0" ),
                        new User (  "theo", "lteif","theolteif@gmail.com","theo","0" ),
                        new User ("james","abi khalil","jamesabikhalil@gmail.com","james","0" ),
                        new User ( "adib", "haddad","adibhaddad@gmail.com","adib","1" ),
                        new User (  "charbel","gemayel","charbelgemayel@gmail.com","charbel", "1" ),
                        new User ( "admin", "admin","admin@gmail.com","admin","2" )
                };

                // Add users to the Users table
                _context.User.AddRange(usersToAdd);
                _context.SaveChanges();
            }
        }
    }
}
