using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhatsAppAPI.Models
{
    public class UserCourseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public string CourseCode { get; set; }

        public string Description { get; set; }

    }
}
