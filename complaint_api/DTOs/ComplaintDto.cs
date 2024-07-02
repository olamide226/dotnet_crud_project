using System;
using System.Collections.Generic;

namespace complaint_api.Dtos
{
    public class CreateComplaintDto
    {
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateComplaintDto
    {
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class ComplaintResponseDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string[] ImageUrls { get; set; } = [];
    }

    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}