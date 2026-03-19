# .NET CRUD Starter Kit 🚀

A production-ready .NET 8 Web API starter template with best practices, clean architecture, and common features pre-configured.

## Features ✨

- **Clean Architecture** - Organized into Api, Core, Infrastructure, and Application layers
- **Generic CRUD Operations** - Base controller and repository for standard operations
- **Authentication & Authorization** - JWT-based authentication ready to use
- **Database Support** - Entity Framework Core with PostgreSQL/SQL Server
- **File Storage** - Abstracted file storage with Local and Azure Blob providers
- **API Documentation** - Swagger/OpenAPI integration
- **Error Handling** - Global exception handling middleware
- **Logging** - Structured logging with Serilog
- **Testing** - Unit and integration test projects included
- **Docker Support** - Dockerfile and docker-compose ready
- **CI/CD** - GitHub Actions workflow included

## Quick Start 🏃‍♂️

### Prerequisites
- .NET 8 SDK
- PostgreSQL or SQL Server
- IDE (Visual Studio, VS Code, or Rider)

### Setup in 5 Minutes

#### Windows (PowerShell)
```powershell
git clone https://github.com/yourusername/dotnet-crud-starter.git
cd dotnet-crud-starter
./scripts/setup.ps1 -EntityName "Product" -PluralName "Products" -Namespace "MyStore.Api"
```

#### Linux/macOS (Bash)
```bash
git clone https://github.com/yourusername/dotnet-crud-starter.git
cd dotnet-crud-starter
chmod +x scripts/setup.sh
./scripts/setup.sh -e Product -p Products -n MyStore.Api
```

### Configure and Run
1. Update database connection in `appsettings.json`
2. Run migrations: `dotnet ef database update`
3. Start the API: `dotnet run`
4. Open: https://localhost:5001/swagger

## Project Structure 📁

```
src/
├── YourNamespace.Api/           # Web API layer
├── YourNamespace.Core/          # Domain models and interfaces
├── YourNamespace.Infrastructure/ # Data access and external services
└── YourNamespace.Application/   # Business logic and services
```

## What Gets Generated 🛠️

When you run the setup script, you get:

- ✅ Complete project structure
- ✅ Base entity with audit fields
- ✅ Generic repository pattern
- ✅ CRUD controller with pagination
- ✅ DTOs for requests/responses
- ✅ AutoMapper profiles
- ✅ Entity Framework configuration
- ✅ Swagger documentation
- ✅ Authentication setup
- ✅ Global error handling
- ✅ Logging configuration

## Customization 🎨

### Add Custom Properties
```csharp
public class Product : BaseEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    // Your properties here
}
```

### Add Business Logic
```csharp
public interface IProductService
{
    Task<bool> CheckAvailability(Guid productId, int quantity);
}
```

### Custom Endpoints
```csharp
[HttpGet("search")]
public async Task<IActionResult> Search([FromQuery] string term)
{
    // Custom implementation
}
```

## Built-in Features 🔧

### Pagination
```http
GET /api/products?page=1&pageSize=10&sortBy=name&sortDesc=false
```

### Search & Filter
```http
GET /api/products?search=laptop&minPrice=500&maxPrice=1500
```

### File Upload
```csharp
[HttpPost("{id}/images")]
public async Task<IActionResult> UploadImages(Guid id, List<IFormFile> files)
```

### Soft Delete
All entities support soft delete by default. Records are marked as deleted but not removed.

### Audit Fields
- CreatedAt
- UpdatedAt
- CreatedBy
- UpdatedBy

## Configuration Options ⚙️

### Database Providers
- PostgreSQL (default)
- SQL Server
- MySQL
- SQLite

### File Storage Providers
- Local Storage
- Azure Blob Storage
- AWS S3 (coming soon)

### Authentication
- JWT Bearer tokens
- Role-based authorization
- Claims-based policies

## Testing 🧪

### Unit Tests
```bash
dotnet test tests/YourNamespace.UnitTests
```

### Integration Tests
```bash
dotnet test tests/YourNamespace.IntegrationTests
```

## Deployment 🚀

### Docker
```bash
docker build -t myapi .
docker run -p 8080:80 myapi
```

### Azure App Service
```bash
az webapp up --name myapi --resource-group mygroup
```

### Kubernetes
```bash
kubectl apply -f k8s/deployment.yaml
```

## Best Practices Included 📚

- ✅ SOLID Principles
- ✅ Repository Pattern
- ✅ Dependency Injection
- ✅ Async/Await
- ✅ Input Validation
- ✅ Response Caching
- ✅ Rate Limiting
- ✅ CORS Configuration
- ✅ Health Checks
- ✅ API Versioning

## Documentation 📖

- [Quick Start Guide](./docs/QUICKSTART.md)
- [Project Structure](./docs/PROJECT_STRUCTURE.md)
- [Authentication Setup](./docs/AUTHENTICATION.md)
- [File Upload Guide](./docs/FILE_UPLOAD.md)
- [Deployment Guide](./docs/DEPLOYMENT.md)
- [Testing Guide](./docs/TESTING.md)

## Contributing 🤝

We welcome contributions! Please see our [Contributing Guide](./CONTRIBUTING.md) for details.

## License 📄

This project is licensed under the MIT License - see the [LICENSE](./LICENSE) file for details.

## Support 💬

- 📧 Email: support@example.com
- 💬 Discord: [Join our community](#)
- 🐛 Issues: [GitHub Issues](https://github.com/yourusername/dotnet-crud-starter/issues)

---

Built with ❤️ by the community. Star ⭐ this repo if you find it helpful!