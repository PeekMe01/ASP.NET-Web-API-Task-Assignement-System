using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class TrackingTask
    {
        public int userid { get; set; }
        public User User { get; set; }
        public int taskid { get; set; }
        public Tasks Task { get; set; }
        public string content { get; set; }

    }
}
