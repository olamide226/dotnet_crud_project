using System;
using System.Collections.Generic;
using DotnetCrud.Core.Models;

namespace complaint_api.Models
{
    public class Complaint : BaseEntity
    {
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string[] ImageUrls { get; set; } = [];
    }
}