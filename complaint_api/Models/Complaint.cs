using System;
using System.Collections.Generic;

namespace complaint_api.Models
{
    public class Complaint
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string[] ImageUrls { get; set; } = [];
    }
}