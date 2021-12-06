using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhatsAppAPI.Models
{
    public class Module
    {
        public int Id { get; set; }

        public string ModuleName { get; set; }

        public string Description { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }
        public ICollection<Topic> Topics = new HashSet<Topic>();
    }
}
