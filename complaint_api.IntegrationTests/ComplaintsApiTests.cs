using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using complaint_api.Dtos;

namespace complaint_api.IntegrationTests
{
    public class ComplaintsApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ComplaintsApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_Complaints_ReturnsOkResult()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/complaints");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_Complaint_ReturnsCreatedResult()
        {
            // Arrange
            var client = _factory.CreateClient();
            var newComplaint = new CreateComplaintDto
            {
                Description = "Test Complaint",
                Name = "Test User"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/complaints", newComplaint);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var returnedComplaint = await response.Content.ReadFromJsonAsync<ComplaintResponseDto>();
            Assert.NotNull(returnedComplaint);
            Assert.Equal(newComplaint.Description, returnedComplaint.Description);
            Assert.Equal(newComplaint.Name, returnedComplaint.Name);
        }

        [Fact]
        public async Task Put_Complaint_ReturnsNoContent()
        {
            // Arrange
            var client = _factory.CreateClient();
            var newComplaint = await CreateComplaint(client);
            var updateComplaint = new UpdateComplaintDto
            {
                Description = "Updated Test Complaint",
                Name = "Updated Test User"
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/complaints/{newComplaint.Id}", updateComplaint);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the update
            var getResponse = await client.GetAsync($"/api/complaints/{newComplaint.Id}");
            getResponse.EnsureSuccessStatusCode();
            var updatedComplaint = await getResponse.Content.ReadFromJsonAsync<ComplaintResponseDto>();
            Assert.NotNull(updatedComplaint);
            Assert.Equal(updateComplaint.Description, updatedComplaint.Description);
            Assert.Equal(updateComplaint.Name, updatedComplaint.Name);
        }

        [Fact]
        public async Task Delete_Complaint_ReturnsNoContent()
        {
            // Arrange
            var client = _factory.CreateClient();
            var newComplaint = await CreateComplaint(client);

            // Act
            var response = await client.DeleteAsync($"/api/complaints/{newComplaint.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the deletion
            var getResponse = await client.GetAsync($"/api/complaints/{newComplaint.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task Upload_Images_ReturnsOkResult()
        {
            // Arrange
            var client = _factory.CreateClient();
            var newComplaint = await CreateComplaint(client);

            using var content = new MultipartFormDataContent();
            var fileContent1 = new ByteArrayContent(new byte[] { 0x12, 0x34, 0x56, 0x78 }); // Dummy file content
            fileContent1.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(fileContent1, "files", "test1.jpg");

            var fileContent2 = new ByteArrayContent(new byte[] { 0x9A, 0xBC, 0xDE, 0xF0 }); // Dummy file content
            fileContent2.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
            content.Add(fileContent2, "files", "test2.png");

            // Act
            var response = await client.PostAsync($"/api/complaints/{newComplaint.Id}/upload", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var uploadedUrls = await response.Content.ReadFromJsonAsync<List<string>>();
            Assert.NotNull(uploadedUrls);
            Assert.Equal(2, uploadedUrls.Count);
        }

        private async Task<ComplaintResponseDto> CreateComplaint(HttpClient client)
        {
            var newComplaint = new CreateComplaintDto
            {
                Description = "Test Complaint",
                Name = "Test User"
            };
            var response = await client.PostAsJsonAsync("/api/complaints", newComplaint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ComplaintResponseDto>();
        }
    }
}