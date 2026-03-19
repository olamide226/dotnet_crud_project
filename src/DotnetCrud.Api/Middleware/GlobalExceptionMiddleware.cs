using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DotnetCrud.Core.DTOs;

namespace DotnetCrud.Api.Middleware
{
    /// <summary>
    /// Global exception handling middleware
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionMiddleware(
            RequestDelegate next, 
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred");

            context.Response.ContentType = "application/json";
            
            var response = GetErrorResponse(exception);
            context.Response.StatusCode = response.StatusCode;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(json);
        }

        private ErrorResponse GetErrorResponse(Exception exception)
        {
            var response = new ErrorResponse
            {
                Message = "An error occurred processing your request"
            };

            if (_environment.IsDevelopment())
            {
                response.Message = exception.Message;
                response.Details = exception.ToString();
            }

            response.StatusCode = exception switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                ValidationException => (int)HttpStatusCode.BadRequest,
                UnauthorizedException => (int)HttpStatusCode.Unauthorized,
                ForbiddenException => (int)HttpStatusCode.Forbidden,
                ConflictException => (int)HttpStatusCode.Conflict,
                _ => (int)HttpStatusCode.InternalServerError
            };

            response.Type = exception.GetType().Name;
            response.TraceId = Guid.NewGuid().ToString();

            return response;
        }

        private class ErrorResponse
        {
            public string Type { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public string? Details { get; set; }
            public int StatusCode { get; set; }
            public string TraceId { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        }
    }

    // Custom Exception Classes
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string entity, object key) 
            : base($"{entity} with key {key} was not found") { }
    }

    public class ValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(string message) : base(message) 
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(Dictionary<string, string[]> errors) 
            : base("One or more validation errors occurred")
        {
            Errors = errors;
        }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message = "Unauthorized access") : base(message) { }
    }

    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "Access forbidden") : base(message) { }
    }

    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}