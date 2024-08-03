using System;
using System.Collections.Generic;
namespace WebApplication1.Models
{
    public class Course
    {
        public int courseid { get; set; }
        public string coursename { get; set; }
        public int instructorid { get; set; }
        public User Instructor { get; set; }

        public List<Tasks> task { get; set; }

        public List<CoursesUsers> CoursesUsers { get; set; }
        public Course(string coursename, int instructorid)
        {
            this.coursename = coursename;
            this.instructorid = instructorid;
        }

        public Course()
        {
            CoursesUsers = new List<CoursesUsers>();
        }
    }
}
