using System;
using System.Collections.Generic;
namespace WebApplication1.Models
{
    public class User
    {
        public int userid { get; set; }
        public string firstname { get;set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string priviledge { get; set; }
        public List<CoursesUsers> CoursesUsers { get; set; }
        public List<TrackingTask> TrackingTask { get; set; }
        public List<Course> course { get; set; }

        
        public User(string firstname, string lastname, string email, string password)
        {
            this.firstname = firstname;
            this.lastname = lastname;
            this.email = email;
            this.password = password;
        }
        public User(string firstname, string lastname, string email, string password, string priviledge)
        {
            this.firstname = firstname;
            this.lastname = lastname;
            this.email = email;
            this.password = password;
            this.priviledge = priviledge;
        }
    }
}
