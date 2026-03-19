using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using DotnetCrud.Core.Interfaces;

namespace DotnetCrud.Infrastructure.Services.FileStorage
{
    /// <summary>
    /// Azure Blob Storage implementation of file storage service
    /// </summary>
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<AzureBlobStorageService> _logger;
        private readonly string _containerPrefix;

        public AzureBlobStorageService(
            IConfiguration configuration,
            ILogger<AzureBlobStorageService> logger)
        {
            var connectionString = configuration["FileStorage:AzureBlob:ConnectionString"];
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerPrefix = configuration["FileStorage:AzureBlob:ContainerPrefix"] ?? "";
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string containerName, string? fileName = null)
        {
            try
            {
                containerName = GetContainerName(containerName);
                var containerClient = await GetOrCreateContainerAsync(containerName);
                
                fileName ??= $"{Guid.NewGuid()}_{file.FileName}";
                var blobClient = containerClient.GetBlobClient(fileName);

                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                };

                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, new BlobUploadOptions 
                { 
                    HttpHeaders = blobHttpHeaders 
                });

                _logger.LogInformation($"File uploaded to Azure Blob Storage: {blobClient.Uri}");
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading file to Azure Blob Storage: {file.FileName}");
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
                var blobClient = new BlobClient(new Uri(fileUrl));
                var response = await blobClient.DownloadStreamingAsync();
                
                return response.Value.Content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading file from Azure Blob Storage: {fileUrl}");
                throw;
            }
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            try
            {
                var blobClient = new BlobClient(new Uri(fileUrl));
                await blobClient.DeleteIfExistsAsync();
                
                _logger.LogInformation($"File deleted from Azure Blob Storage: {fileUrl}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file from Azure Blob Storage: {fileUrl}");
                throw;
            }
        }

        public async Task DeleteFilesAsync(IList<string> fileUrls)
        {
            var tasks = fileUrls.Select(DeleteFileAsync);
            await Task.WhenAll(tasks);
        }

        public async Task<bool> FileExistsAsync(string fileUrl)
        {
            try
            {
                var blobClient = new BlobClient(new Uri(fileUrl));
                var response = await blobClient.ExistsAsync();
                
                return response.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking file existence in Azure Blob Storage: {fileUrl}");
                return false;
            }
        }

        public async Task<FileMetadata> GetFileMetadataAsync(string fileUrl)
        {
            try
            {
                var blobClient = new BlobClient(new Uri(fileUrl));
                var properties = await blobClient.GetPropertiesAsync();
                
                var metadata = new FileMetadata
                {
                    FileName = blobClient.Name,
                    Size = properties.Value.ContentLength,
                    ContentType = properties.Value.ContentType,
                    LastModified = properties.Value.LastModified.UtcDateTime,
                    CustomMetadata = properties.Value.Metadata
                };

                return metadata;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting file metadata from Azure Blob Storage: {fileUrl}");
                throw;
            }
        }

        public async Task<string> GenerateTemporaryUrlAsync(string fileUrl, TimeSpan expiration)
        {
            try
            {
                var blobClient = new BlobClient(new Uri(fileUrl));
                
                if (blobClient.CanGenerateSasUri)
                {
                    var sasBuilder = new BlobSasBuilder
                    {
                        BlobContainerName = blobClient.BlobContainerName,
                        BlobName = blobClient.Name,
                        Resource = "b",
                        ExpiresOn = DateTimeOffset.UtcNow.Add(expiration)
                    };

                    sasBuilder.SetPermissions(BlobSasPermissions.Read);
                    
                    return blobClient.GenerateSasUri(sasBuilder).ToString();
                }

                // Fallback to permanent URL if SAS generation is not available
                _logger.LogWarning("SAS URI generation not available. Returning permanent URL.");
                return await Task.FromResult(fileUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating temporary URL for Azure Blob Storage: {fileUrl}");
                throw;
            }
        }

        private async Task<BlobContainerClient> GetOrCreateContainerAsync(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
            
            return containerClient;
        }

        private string GetContainerName(string containerName)
        {
            if (!string.IsNullOrEmpty(_containerPrefix))
            {
                return $"{_containerPrefix}-{containerName}".ToLower();
            }
            
            return containerName.ToLower();
        }
    }
}