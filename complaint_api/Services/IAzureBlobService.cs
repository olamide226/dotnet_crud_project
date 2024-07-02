using Microsoft.AspNetCore.Http;

namespace complaint_api.Services
{
    public interface IAzureBlobService
    {
        Task<List<string>> UploadFilesAsync(Guid complaintId, List<IFormFile> files);
        Task<Stream> GetBlobAsync(string blobName);
        Task DeleteFileAsync(string fileName);
    }
}