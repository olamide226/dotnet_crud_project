using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DotnetCrud.Core.Interfaces;

namespace DotnetCrud.Infrastructure.Services.FileStorage
{
    /// <summary>
    /// Local file system implementation of file storage service
    /// </summary>
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _basePath;
        private readonly ILogger<LocalFileStorageService> _logger;
        private readonly string _baseUrl;

        public LocalFileStorageService(
            IConfiguration configuration,
            ILogger<LocalFileStorageService> logger)
        {
            _basePath = configuration["FileStorage:Local:BasePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            _baseUrl = configuration["FileStorage:Local:BaseUrl"] ?? "/uploads";
            _logger = logger;

            // Ensure base directory exists
            Directory.CreateDirectory(_basePath);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string containerName, string? fileName = null)
        {
            try
            {
                var containerPath = Path.Combine(_basePath, containerName);
                Directory.CreateDirectory(containerPath);

                fileName ??= $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(containerPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                var fileUrl = $"{_baseUrl}/{containerName}/{fileName}";
                _logger.LogInformation($"File uploaded successfully: {fileUrl}");

                return fileUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading file: {file.FileName}");
                throw;
            }
        }

        public async Task<IList<string>> UploadFilesAsync(IList<IFormFile> files, string containerName)
        {
            var uploadedUrls = new List<string>();

            foreach (var file in files)
            {
                var url = await UploadFileAsync(file, containerName);
                uploadedUrls.Add(url);
            }

            return uploadedUrls;
        }

        public async Task<Stream> DownloadFileAsync(string fileUrl)
        {
            try
            {
                var relativePath = fileUrl.Replace(_baseUrl, "").TrimStart('/');
                var filePath = Path.Combine(_basePath, relativePath);

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File not found: {fileUrl}");
                }

                return await Task.FromResult(new FileStream(filePath, FileMode.Open, FileAccess.Read));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading file: {fileUrl}");
                throw;
            }
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            try
            {
                var relativePath = fileUrl.Replace(_baseUrl, "").TrimStart('/');
                var filePath = Path.Combine(_basePath, relativePath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation($"File deleted: {fileUrl}");
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file: {fileUrl}");
                throw;
            }
        }

        public async Task DeleteFilesAsync(IList<string> fileUrls)
        {
            foreach (var fileUrl in fileUrls)
            {
                await DeleteFileAsync(fileUrl);
            }
        }

        public async Task<bool> FileExistsAsync(string fileUrl)
        {
            try
            {
                var relativePath = fileUrl.Replace(_baseUrl, "").TrimStart('/');
                var filePath = Path.Combine(_basePath, relativePath);
                return await Task.FromResult(File.Exists(filePath));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking file existence: {fileUrl}");
                return false;
            }
        }

        public async Task<FileMetadata> GetFileMetadataAsync(string fileUrl)
        {
            try
            {
                var relativePath = fileUrl.Replace(_baseUrl, "").TrimStart('/');
                var filePath = Path.Combine(_basePath, relativePath);

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File not found: {fileUrl}");
                }

                var fileInfo = new FileInfo(filePath);
                var metadata = new FileMetadata
                {
                    FileName = fileInfo.Name,
                    Size = fileInfo.Length,
                    ContentType = GetContentType(fileInfo.Extension),
                    LastModified = fileInfo.LastWriteTimeUtc
                };

                return await Task.FromResult(metadata);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting file metadata: {fileUrl}");
                throw;
            }
        }

        public async Task<string> GenerateTemporaryUrlAsync(string fileUrl, TimeSpan expiration)
        {
            // For local storage, we'll return the same URL
            // In a real implementation, you might want to implement token-based access
            _logger.LogWarning("Temporary URLs are not implemented for local storage. Returning permanent URL.");
            return await Task.FromResult(fileUrl);
        }

        private string GetContentType(string extension)
        {
            return extension.ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                ".json" => "application/json",
                ".xml" => "application/xml",
                ".zip" => "application/zip",
                _ => "application/octet-stream"
            };
        }
    }
}