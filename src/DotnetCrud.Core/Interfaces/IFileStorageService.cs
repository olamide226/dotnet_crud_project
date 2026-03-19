using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DotnetCrud.Core.Interfaces
{
    /// <summary>
    /// Interface for file storage operations
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Upload a single file
        /// </summary>
        /// <param name="file">File to upload</param>
        /// <param name="containerName">Storage container/bucket name</param>
        /// <param name="fileName">Optional custom file name</param>
        /// <returns>URL of the uploaded file</returns>
        Task<string> UploadFileAsync(IFormFile file, string containerName, string? fileName = null);

        /// <summary>
        /// Upload multiple files
        /// </summary>
        /// <param name="files">Files to upload</param>
        /// <param name="containerName">Storage container/bucket name</param>
        /// <returns>List of URLs for uploaded files</returns>
        Task<IList<string>> UploadFilesAsync(IList<IFormFile> files, string containerName);

        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="fileUrl">URL or path of the file</param>
        /// <returns>File stream</returns>
        Task<Stream> DownloadFileAsync(string fileUrl);

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="fileUrl">URL or path of the file to delete</param>
        Task DeleteFileAsync(string fileUrl);

        /// <summary>
        /// Delete multiple files
        /// </summary>
        /// <param name="fileUrls">URLs or paths of files to delete</param>
        Task DeleteFilesAsync(IList<string> fileUrls);

        /// <summary>
        /// Check if a file exists
        /// </summary>
        /// <param name="fileUrl">URL or path of the file</param>
        /// <returns>True if file exists, false otherwise</returns>
        Task<bool> FileExistsAsync(string fileUrl);

        /// <summary>
        /// Get file metadata
        /// </summary>
        /// <param name="fileUrl">URL or path of the file</param>
        /// <returns>File metadata</returns>
        Task<FileMetadata> GetFileMetadataAsync(string fileUrl);

        /// <summary>
        /// Generate a temporary download URL with expiration
        /// </summary>
        /// <param name="fileUrl">URL or path of the file</param>
        /// <param name="expiration">Expiration time for the URL</param>
        /// <returns>Temporary download URL</returns>
        Task<string> GenerateTemporaryUrlAsync(string fileUrl, TimeSpan expiration);
    }

    /// <summary>
    /// File metadata information
    /// </summary>
    public class FileMetadata
    {
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
        public Dictionary<string, string> CustomMetadata { get; set; } = new();
    }
}