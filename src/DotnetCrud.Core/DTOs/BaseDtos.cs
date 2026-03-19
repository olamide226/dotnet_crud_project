using System;

namespace DotnetCrud.Core.DTOs
{
    /// <summary>
    /// Base interface for create DTOs
    /// </summary>
    public interface ICreateDto
    {
    }

    /// <summary>
    /// Base interface for update DTOs
    /// </summary>
    public interface IUpdateDto
    {
    }

    /// <summary>
    /// Base interface for response DTOs
    /// </summary>
    public interface IResponseDto
    {
        Guid Id { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Base response DTO with common properties
    /// </summary>
    public abstract class BaseResponseDto : IResponseDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Generic API response wrapper
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }

    /// <summary>
    /// Request parameters for filtering and sorting
    /// </summary>
    public class QueryParameters
    {
        private const int MaxPageSize = 100;
        private int _pageSize = 20;

        public int Page { get; set; } = 1;
        
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public bool SortDesc { get; set; } = false;
        public Dictionary<string, string> Filters { get; set; } = new();
    }
}