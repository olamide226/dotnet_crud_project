using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using complaint_api.Controllers;
using complaint_api.Data;
using complaint_api.Models;
using complaint_api.Services;
using complaint_api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace complaint_api.Tests
{
    public class ComplaintsControllerTests
    {
        private readonly Mock<AppDbContext> _mockContext;
        private readonly Mock<IAzureBlobService> _mockBlobService;
        private readonly Mock<ILogger<ComplaintsController>> _mockLogger;

        public ComplaintsControllerTests()
        {
            _mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            _mockBlobService = new Mock<IAzureBlobService>();
            _mockLogger = new Mock<ILogger<ComplaintsController>>();
        }

        [Fact]
        public async Task GetComplaints_ReturnsOkResult_WithPaginatedResponse()
        {
            // Arrange
            var complaints = new List<Complaint>
            {
                new Complaint { Id = Guid.NewGuid(), Description = "Test1", Name = "User1", CreatedTime = DateTime.UtcNow },
                new Complaint { Id = Guid.NewGuid(), Description = "Test2", Name = "User2", CreatedTime = DateTime.UtcNow }
            };

            var mockSet = new Mock<DbSet<Complaint>>();
            mockSet.As<IQueryable<Complaint>>().Setup(m => m.Provider).Returns(complaints.AsQueryable().Provider);
            mockSet.As<IQueryable<Complaint>>().Setup(m => m.Expression).Returns(complaints.AsQueryable().Expression);
            mockSet.As<IQueryable<Complaint>>().Setup(m => m.ElementType).Returns(complaints.AsQueryable().ElementType);
            mockSet.As<IQueryable<Complaint>>().Setup(m => m.GetEnumerator()).Returns(complaints.GetEnumerator());

            _mockContext.Setup(c => c.Complaints).Returns(mockSet.Object);

            var controller = new ComplaintsController(_mockContext.Object, _mockBlobService.Object, _mockLogger.Object);

            // Act
            var result = await controller.GetComplaints();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PaginatedResponse<ComplaintResponseDto>>(okResult.Value);
            Assert.Equal(2, returnValue.TotalCount);
            Assert.Equal(2, returnValue.Items.Count);
        }

        [Fact]
        public async Task GetComplaint_ReturnsOkResult_WhenComplaintExists()
        {
            // Arrange
            var complaintId = Guid.NewGuid();
            var complaint = new Complaint { Id = complaintId, Description = "Test", Name = "User", CreatedTime = DateTime.UtcNow };

            _mockContext.Setup(c => c.Complaints.FindAsync(complaintId)).ReturnsAsync(complaint);

            var controller = new ComplaintsController(_mockContext.Object, _mockBlobService.Object, _mockLogger.Object);

            // Act
            var result = await controller.GetComplaint(complaintId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ComplaintResponseDto>(okResult.Value);
            Assert.Equal(complaintId, returnValue.Id);
            Assert.Equal("Test", returnValue.Description);
            Assert.Equal("User", returnValue.Name);
        }

        [Fact]
        public async Task GetComplaint_ReturnsNotFound_WhenComplaintDoesNotExist()
        {
            // Arrange
            var complaintId = Guid.NewGuid();

            _mockContext.Setup(c => c.Complaints.FindAsync(complaintId)).ReturnsAsync((Complaint)null);

            var controller = new ComplaintsController(_mockContext.Object, _mockBlobService.Object, _mockLogger.Object);

            // Act
            var result = await controller.GetComplaint(complaintId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostComplaint_ReturnsCreatedAtAction_WithNewComplaint()
        {
            // Arrange
            var createComplaintDto = new CreateComplaintDto { Description = "New Complaint", Name = "New User" };

            _mockContext.Setup(c => c.Complaints.AddAsync(It.IsAny<Complaint>(), default))
                .ReturnsAsync((Complaint complaint, CancellationToken token) => null);

            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            var controller = new ComplaintsController(_mockContext.Object, _mockBlobService.Object, _mockLogger.Object);

            // Act
            var result = await controller.PostComplaint(createComplaintDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<ComplaintResponseDto>(createdAtActionResult.Value);
            Assert.Equal("New Complaint", returnValue.Description);
            Assert.Equal("New User", returnValue.Name);
            Assert.NotEqual(Guid.Empty, returnValue.Id);
        }

        [Fact]
        public async Task PutComplaint_ReturnsNoContent_WhenComplaintExists()
        {
            // Arrange
            var complaintId = Guid.NewGuid();
            var complaint = new Complaint { Id = complaintId, Description = "Old Description", Name = "Old Name" };
            var updateComplaintDto = new UpdateComplaintDto { Description = "Updated Description", Name = "Updated Name" };

            _mockContext.Setup(c => c.Complaints.FindAsync(complaintId)).ReturnsAsync(complaint);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            var controller = new ComplaintsController(_mockContext.Object, _mockBlobService.Object, _mockLogger.Object);

            // Act
            var result = await controller.PutComplaint(complaintId, updateComplaintDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Updated Description", complaint.Description);
            Assert.Equal("Updated Name", complaint.Name);
        }

        [Fact]
        public async Task PutComplaint_ReturnsNotFound_WhenComplaintDoesNotExist()
        {
            // Arrange
            var complaintId = Guid.NewGuid();
            var updateComplaintDto = new UpdateComplaintDto { Description = "Updated Description", Name = "Updated Name" };

            _mockContext.Setup(c => c.Complaints.FindAsync(complaintId)).ReturnsAsync((Complaint)null);

            var controller = new ComplaintsController(_mockContext.Object, _mockBlobService.Object, _mockLogger.Object);

            // Act
            var result = await controller.PutComplaint(complaintId, updateComplaintDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteComplaint_ReturnsNoContent_WhenComplaintExists()
        {
            // Arrange
            var complaintId = Guid.NewGuid();
            var complaint = new Complaint { Id = complaintId, Description = "Test", Name = "User", ImageUrls = new[] { "image1.jpg", "image2.jpg" } };

            _mockContext.Setup(c => c.Complaints.FindAsync(complaintId)).ReturnsAsync(complaint);
            _mockContext.Setup(c => c.Complaints.Remove(complaint));
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            _mockBlobService.Setup(b => b.DeleteFileAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            var controller = new ComplaintsController(_mockContext.Object, _mockBlobService.Object, _mockLogger.Object);

            // Act
            var result = await controller.DeleteComplaint(complaintId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockBlobService.Verify(b => b.DeleteFileAsync(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public async Task DeleteComplaint_ReturnsNotFound_WhenComplaintDoesNotExist()
        {
            // Arrange
            var complaintId = Guid.NewGuid();

            _mockContext.Setup(c => c.Complaints.FindAsync(complaintId)).ReturnsAsync((Complaint)null);

            var controller = new ComplaintsController(_mockContext.Object, _mockBlobService.Object, _mockLogger.Object);

            // Act
            var result = await controller.DeleteComplaint(complaintId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UploadImages_ReturnsOkResult_WithUploadedUrls()
        {
            // Arrange
            var complaintId = Guid.NewGuid();
            var complaint = new Complaint { Id = complaintId, Description = "Test", Name = "User" };
            var files = new List<IFormFile> { Mock.Of<IFormFile>(), Mock.Of<IFormFile>() };
            var uploadedUrls = new List<string> { "url1", "url2" };

            _mockContext.Setup(c => c.Complaints.FindAsync(complaintId)).ReturnsAsync(complaint);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            _mockBlobService.Setup(b => b.UploadFilesAsync(complaintId, files)).ReturnsAsync(uploadedUrls);

            var controller = new ComplaintsController(_mockContext.Object, _mockBlobService.Object, _mockLogger.Object);

            // Act
            var result = await controller.UploadImages(complaintId, files);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<string>>(okResult.Value);
            Assert.Equal(uploadedUrls, returnValue);
            Assert.Equal(uploadedUrls, complaint.ImageUrls);
        }

        [Fact]
        public async Task UploadImages_ReturnsNotFound_WhenComplaintDoesNotExist()
        {
            // Arrange
            var complaintId = Guid.NewGuid();
            var files = new List<IFormFile> { Mock.Of<IFormFile>(), Mock.Of<IFormFile>() };

            _mockContext.Setup(c => c.Complaints.FindAsync(complaintId)).ReturnsAsync((Complaint)null);

            var controller = new ComplaintsController(_mockContext.Object, _mockBlobService.Object, _mockLogger.Object);

            // Act
            var result = await controller.UploadImages(complaintId, files);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}