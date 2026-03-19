using complaint_api.Data;
using complaint_api.Models;
using Microsoft.EntityFrameworkCore;

namespace complaint_api.IntegrationTests
{
    public class TestDatabaseContext : AppDbContext
    {
        public TestDatabaseContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public void SeedTestData()
        {
            Complaints.AddRange(new List<Complaint>
            {
                new Complaint
                {
                    Id = Guid.Parse("12345678-1234-5678-1234-567812345678"),
                    Description = "Test Complaint 1",
                    Name = "Test User 1",
                    CreatedTime = DateTime.UtcNow.AddDays(-1),
                    ImageUrls = new[] { "http://test.com/image1.jpg", "http://test.com/image2.jpg" }
                },
                new Complaint
                {
                    Id = Guid.Parse("87654321-8765-4321-8765-432187654321"),
                    Description = "Test Complaint 2",
                    Name = "Test User 2",
                    CreatedTime = DateTime.UtcNow,
                    ImageUrls = new[] { "http://test.com/image3.jpg" }
                }
            });

            SaveChanges();
        }
    }
}