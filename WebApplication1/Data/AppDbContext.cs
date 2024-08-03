using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
namespace WebApplication1.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions options):base(options) {}
        public DbSet <User> User { get; set;}
        public DbSet<CoursesUsers> CoursesUsers { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<TrackingTask> TrackingTask { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .HasIndex(u => u.email)
            .IsUnique();
          
            modelBuilder.Entity<Course>()
                        .HasOne(c => c.Instructor)
                        .WithMany(u => u.course)
                        .HasForeignKey(c => c.instructorid);
            modelBuilder.Entity<Course>()
                        .HasIndex(c => c.coursename)
                        .IsUnique();

            modelBuilder.Entity<Tasks>()
                        .HasOne(t => t.course)
                        .WithMany(c => c.task)
                        .HasForeignKey(t => t.courseid);
                        

            modelBuilder.Entity<TrackingTask>()
                .HasKey(ut => new { ut.userid, ut.taskid });

            modelBuilder.Entity<TrackingTask>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.TrackingTask)
                .HasForeignKey(ut => ut.userid);

            modelBuilder.Entity<TrackingTask>()
                .HasOne(ut => ut.Task)
                .WithMany(t => t.TrackingTask)
                .HasForeignKey(ut => ut.taskid);

            modelBuilder.Entity<CoursesUsers>()
               .HasKey(cu => new { cu.courseid, cu.userid });

            modelBuilder.Entity<CoursesUsers>()
                .HasOne(cu => cu.Course)
                .WithMany(c => c.CoursesUsers)
                .HasForeignKey(cu => cu.courseid);

            modelBuilder.Entity<CoursesUsers>()
                .HasOne(cu => cu.User)
                .WithMany(u => u.CoursesUsers)
                .HasForeignKey(cu => cu.userid);
        }


    }
}
