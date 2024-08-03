using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Tasks
    {
        [Key]
        public int taskid { get; set; }
        public string taskname { get; set; }
        public string taskdescription { get; set; }
        public DateTime duedate { get; set; }

        public int courseid { get; set; }

        public Course course { get; set; }

        public List<TrackingTask> TrackingTask { get; set; }

        public Tasks(string taskname, string taskdescription,DateTime duedate, int courseid)
        {
            this.taskname = taskname;
            this.taskdescription = taskdescription;
            this.duedate = duedate;
            this.courseid = courseid;
           
        }
    }
}
