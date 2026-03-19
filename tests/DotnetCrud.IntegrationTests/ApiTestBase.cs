using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DotnetCrud.Infrastructure.Data;

namespace DotnetCrud.IntegrationTests
{
    public abstract class ApiTestBase : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        protected readonly WebApplicationFactory<Program> _factory;
        protected readonly HttpClient _client;
        protected readonly JsonSerializerOptions _jsonOptions;

        protected ApiTestBase(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the existing DbContext registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add in-memory database for testing
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                    });

                    // Build the service provider
                    var sp = services.BuildServiceProvider();

                    // Create a scope to obtain a reference to the database
                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                        var logger = scopedServices.GetRequiredService<ILogger<ApiTestBase>>();

                        // Ensure the database is created
                        db.Database.EnsureCreated();

                        try
                        {
                            // Seed the database with test data
                            SeedDatabase(db);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "An error occurred seeding the database with test data");
                        }
                    }
                });

                builder.UseEnvironment("Testing");
            });

            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Set up authentication if needed
            SetupAuthentication();
        }

        protected virtual void SeedDatabase(ApplicationDbContext context)
        {
            // Override in derived classes to seed test data
        }

        protected virtual void SetupAuthentication()
        {
            // Override in derived classes to set up authentication
            // For example, add a test JWT token to the client headers
        }

        protected async Task<T?> GetAsync<T>(string url)
        {
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        protected async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            return await _client.PostAsync(url, content);
        }

        protected async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            return await _client.PutAsync(url, content);
        }

        protected async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return await _client.DeleteAsync(url);
        }

        protected async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        protected void AddAuthToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}