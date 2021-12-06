using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhatsAppAPI.Models
{
    public class Course
    {
        public int Id { get; set; }

        public string CourseName { get; set; }

        public string CourseCode { get; set; }

        public string Description { get; set; }
        public ICollection<Module> Modules = new HashSet<Module>();

        public ICollection<UserCourse> UserCourses = new HashSet<UserCourse>();
    }
}
