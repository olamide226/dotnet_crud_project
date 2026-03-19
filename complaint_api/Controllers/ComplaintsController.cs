using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using complaint_api.Data;
using complaint_api.Models;
using complaint_api.Services;
using complaint_api.Dtos;
using complaint_api.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Web;
using DotnetCrud.Core.Models;
using DotnetCrud.Core.Interfaces;

namespace complaint_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ComplaintsController : ControllerBase
    {
        private readonly IComplaintRepository _repository;
        private readonly IAzureBlobService _blobService;
        private readonly ILogger<ComplaintsController> _logger;

        public ComplaintsController(IComplaintRepository repository, IAzureBlobService blobService, ILogger<ComplaintsController> logger)
        {
            _repository = repository;
            _blobService = blobService;
            _logger = logger;
        }

        // GET: api/Complaints
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<ComplaintResponseDto>>> GetComplaints([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var (complaints, totalCount) = await _repository.GetPaginatedAsync(page, pageSize);
            
            var complaintDtos = complaints.Select(MapToDTO).ToList();

            var paginatedResponse = new PaginatedResponse<ComplaintResponseDto>
            {
                Items = complaintDtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };

            return Ok(paginatedResponse);
        }

        // GET: api/Complaints/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ComplaintResponseDto>> GetComplaint(Guid id)
        {
            var complaint = await _repository.GetByIdAsync(id);

            if (complaint == null)
            {
                return NotFound();
            }

            return MapToDTO(complaint);
        }

        // POST: api/Complaints
        [HttpPost]
        public async Task<ActionResult<ComplaintResponseDto>> PostComplaint(CreateComplaintDto createComplaintDto)
        {
            var complaint = new Complaint
            {
                Description = createComplaintDto.Description,
                Name = createComplaintDto.Name
            };

            await _repository.AddAsync(complaint);
            await _repository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComplaint), new { id = complaint.Id }, MapToDTO(complaint));
        }

        // PUT: api/Complaints/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComplaint(Guid id, UpdateComplaintDto updateComplaintDto)
        {
            var complaint = await _repository.GetByIdAsync(id);

            if (complaint == null)
            {
                return NotFound();
            }

            complaint.Description = updateComplaintDto.Description;
            complaint.Name = updateComplaintDto.Name;
            complaint.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _repository.UpdateAsync(complaint);
                await _repository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _repository.ExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Complaints/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComplaint(Guid id)
        {
            var complaint = await _repository.GetByIdAsync(id);
            if (complaint == null)
            {
                return NotFound();
            }

            foreach (var imageUrl in complaint.ImageUrls)
            {
                await _blobService.DeleteFileAsync(imageUrl);
            }
            
            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Complaints/5/upload
        [HttpPost("{id}/upload")]
        public async Task<ActionResult<List<string>>> UploadImages(Guid id, List<IFormFile> files)
        {
            var complaint = await _repository.GetByIdAsync(id);
            if (complaint == null)
            {
                return NotFound();
            }

            try
            {
                var uploadedUrls = await _blobService.UploadFilesAsync(id, files);
                complaint.ImageUrls = uploadedUrls.ToArray();
                complaint.UpdatedAt = DateTime.UtcNow;
                
                await _repository.UpdateAsync(complaint);
                await _repository.SaveChangesAsync();

                return Ok(uploadedUrls);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading files for complaint {ComplaintId}", id);
                return StatusCode(500, "An error occurred while uploading the files.");
            }
        }

        // Get: api/Complaints/5/upload/my-file-name.jpg
        [HttpGet("{id}/upload/{fileName}")]
        public async Task<ActionResult<List<string>>> GetImages(Guid id, string fileName)
        {
            // url decode the filename
            fileName = HttpUtility.UrlDecode(fileName);
            var complaint = await _repository.GetByIdAsync(id);
            if (complaint == null)
            {
                return NotFound();
            }
            if (!complaint.ImageUrls.Contains(fileName))
            {
                return NotFound(new { message = "File not found in " + complaint.ImageUrls[0] });
            }
            
            var fileStream = await _blobService.GetBlobAsync(fileName);
            var contentType = fileName.EndsWith(".pdf") ? "application/pdf" : "image/jpeg";

            return File(fileStream, contentType);
        }



        private static ComplaintResponseDto MapToDTO(Complaint complaint)
        {
            return new ComplaintResponseDto
            {
                Id = complaint.Id,
                Description = complaint.Description,
                Name = complaint.Name,
                CreatedTime = complaint.CreatedAt,
                ImageUrls = complaint.ImageUrls
            };
        }
    }
}