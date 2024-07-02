using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;

namespace complaint_api.Services
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public AzureBlobService(IConfiguration configuration)
        {
            _blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage"));
            _containerName = configuration["AzureBlobStorage:ContainerName"] ?? "complaints";
        }

        public async Task<List<string>> UploadFilesAsync(Guid complaintId, List<IFormFile> files)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobNames = new List<string>();

            foreach (var file in files)
            {
                var blobName = $"{complaintId}/{Guid.NewGuid()}_{file.FileName}";
                var blobClient = containerClient.GetBlobClient(blobName);

                await using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, true);

                blobNames.Add(blobClient.Name);
            }

            return blobNames;
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<Stream> GetBlobAsync(string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var response = await blobClient.DownloadAsync();
        return response.Value.Content;
    }
    }
}