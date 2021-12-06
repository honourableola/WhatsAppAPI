using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhatsAppAPI.Models
{
    public class Topic
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int ModuleId { get; set; }

        public Module Module { get; set; }
    }
}
