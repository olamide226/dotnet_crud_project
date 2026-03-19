using System.Collections.Generic;

namespace DotnetCrud.Core.DTOs
{
    /// <summary>
    /// Generic paginated response wrapper
    /// </summary>
    /// <typeparam name="T">Type of items in the response</typeparam>
    public class PaginatedResponse<T>
    {
        /// <summary>
        /// List of items for the current page
        /// </summary>
        public List<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Indicates if there is a previous page
        /// </summary>
        public bool HasPrevious { get; set; }

        /// <summary>
        /// Indicates if there is a next page
        /// </summary>
        public bool HasNext { get; set; }

        /// <summary>
        /// URL to the first page (optional)
        /// </summary>
        public string? FirstPageUrl { get; set; }

        /// <summary>
        /// URL to the last page (optional)
        /// </summary>
        public string? LastPageUrl { get; set; }

        /// <summary>
        /// URL to the next page (optional)
        /// </summary>
        public string? NextPageUrl { get; set; }

        /// <summary>
        /// URL to the previous page (optional)
        /// </summary>
        public string? PreviousPageUrl { get; set; }
    }
}