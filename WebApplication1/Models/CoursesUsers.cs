using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class CoursesUsers
    {
        public int courseid { get; set; }
        public Course Course { get; set; }
        public int userid { get; set; }
        public User User { get; set; }

       
    }
   
}
