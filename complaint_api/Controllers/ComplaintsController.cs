using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using complaint_api.Data;
using complaint_api.Models;
using complaint_api.Services;
using complaint_api.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Web;

namespace complaint_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ComplaintsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAzureBlobService _blobService;
        private readonly ILogger<ComplaintsController> _logger;

        public ComplaintsController(AppDbContext context, IAzureBlobService blobService, ILogger<ComplaintsController> logger)
        {
            _context = context;
            _blobService = blobService;
            _logger = logger;
        }

        // GET: api/Complaints
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<ComplaintResponseDto>>> GetComplaints([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var totalComplaints = await _context.Complaints.CountAsync();
            var complaints = await _context.Complaints
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var complaintDtos = complaints.Select(MapToDTO).ToList();

            var paginatedResponse = new PaginatedResponse<ComplaintResponseDto>
            {
                Items = complaintDtos,
                TotalCount = totalComplaints,
                PageNumber = page,
                PageSize = pageSize
            };

            return Ok(paginatedResponse);
        }

        // GET: api/Complaints/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ComplaintResponseDto>> GetComplaint(Guid id)
        {
            var complaint = await _context.Complaints.FindAsync(id);

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
                Id = Guid.NewGuid(),
                Description = createComplaintDto.Description,
                Name = createComplaintDto.Name,
                CreatedTime = DateTime.UtcNow
            };

            await _context.Complaints.AddAsync(complaint);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComplaint), new { id = complaint.Id }, MapToDTO(complaint));
        }

        // PUT: api/Complaints/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComplaint(Guid id, UpdateComplaintDto updateComplaintDto)
        {
            var complaint = await _context.Complaints.FindAsync(id);

            if (complaint == null)
            {
                return NotFound();
            }

            complaint.Description = updateComplaintDto.Description;
            complaint.Name = updateComplaintDto.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComplaintExists(id))
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
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null)
            {
                return NotFound();
            }

            foreach (var imageUrl in complaint.ImageUrls)
            {
                await _blobService.DeleteFileAsync(imageUrl);
            }
            _context.Complaints.Remove(complaint);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Complaints/5/upload
        [HttpPost("{id}/upload")]
        public async Task<ActionResult<List<string>>> UploadImages(Guid id, List<IFormFile> files)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null)
            {
                return NotFound();
            }

            try
            {
                var uploadedUrls = await _blobService.UploadFilesAsync(id, files);
                complaint.ImageUrls = uploadedUrls.ToArray();
                await _context.SaveChangesAsync();

                return Ok(uploadedUrls);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading files for complaint {ComplaintId}", id);
                return StatusCode(500, "An error occurred while uploading the files.");
            }
        }

        // Get: api/Complaints/5/upload
        [HttpGet("{id}/upload/{fileName}")]
        public async Task<ActionResult<List<string>>> GetImages(Guid id, string fileName)
        {
            // url decode the filename
            fileName = HttpUtility.UrlDecode(fileName);
            var complaint = await _context.Complaints.FindAsync(id);
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

        private bool ComplaintExists(Guid id)
        {
            return _context.Complaints.Any(e => e.Id == id);
        }

        private static ComplaintResponseDto MapToDTO(Complaint complaint)
        {
            return new ComplaintResponseDto
            {
                Id = complaint.Id,
                Description = complaint.Description,
                Name = complaint.Name,
                CreatedTime = complaint.CreatedTime,
                ImageUrls = complaint.ImageUrls
            };
        }
    }
}