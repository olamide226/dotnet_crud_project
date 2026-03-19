# 🚀 Introducing .NET CRUD Starter Kit

## Create Production-Ready APIs in Under 5 Minutes!

I'm excited to share the **.NET CRUD Starter Kit** - an open-source project that helps you bootstrap production-ready .NET 8 APIs with clean architecture and best practices built-in.

### 🎯 The Problem

Starting a new .NET API project often means:
- Setting up the same boilerplate code
- Implementing basic CRUD operations from scratch
- Configuring authentication, logging, error handling
- Establishing project structure and patterns
- Writing pagination, filtering, and sorting logic

This takes hours or even days before you can focus on your actual business logic.

### 💡 The Solution

The .NET CRUD Starter Kit generates a complete API structure with a single command:

```bash
./scripts/setup.sh -e Product -p Products -n MyStore.Api
```

In seconds, you get:
- ✅ Clean Architecture (Api, Core, Infrastructure, Application layers)
- ✅ Generic CRUD operations with pagination
- ✅ JWT authentication ready to use
- ✅ Entity Framework Core with repository pattern
- ✅ Global error handling and logging
- ✅ Swagger documentation
- ✅ Unit and integration test projects
- ✅ Docker support

### 🔥 Key Features

**1. Smart Code Generation**
- Generates models, DTOs, controllers, and repositories
- Includes validation attributes
- Sets up AutoMapper profiles
- Creates proper project references

**2. Production-Ready Infrastructure**
- Structured logging with Serilog
- Health checks endpoint
- CORS configuration
- Rate limiting
- Response compression

**3. Flexible and Extensible**
- Easy to add custom properties
- Simple to implement business logic
- Multiple file storage providers (Local, Azure Blob)
- Database provider agnostic

**4. Best Practices Built-In**
- Repository pattern
- Dependency injection
- Async/await throughout
- Soft delete support
- Audit fields (CreatedAt, UpdatedAt, CreatedBy)

### 📖 Real Example: E-Commerce API

The kit includes a complete e-commerce example showing:
- Product management with categories
- Advanced search with filters
- Stock checking
- Featured products
- Price range queries

```csharp
// Custom endpoint added to generated controller
[HttpGet("search")]
public async Task<IActionResult> SearchProducts(
    [FromQuery] string? term,
    [FromQuery] decimal? minPrice,
    [FromQuery] decimal? maxPrice)
{
    // Implementation with complex filtering
}
```

### 🚦 Getting Started

1. **Clone the starter kit**
   ```bash
   git clone https://github.com/yourusername/dotnet-crud-starter.git
   cd dotnet-crud-starter
   ```

2. **Generate your API**
   ```bash
   ./scripts/setup.sh -e Product -p Products -n MyStore.Api
   ```

3. **Run it**
   ```bash
   cd src/MyStore.Api
   dotnet run
   ```

4. **Access Swagger**
   
   Navigate to: https://localhost:5001/swagger

### 🎥 See It In Action

[Watch the demo video](https://youtube.com/watch?v=demo) showing how to create a complete API in under 5 minutes.

### 🤝 Community

This is an open-source project and contributions are welcome!

- ⭐ [Star on GitHub](https://github.com/yourusername/dotnet-crud-starter)
- 🐛 [Report Issues](https://github.com/yourusername/dotnet-crud-starter/issues)
- 💬 [Join Discord](https://discord.gg/starter-kit)
- 📚 [Read the Docs](https://github.com/yourusername/dotnet-crud-starter/docs)

### 🎯 Who Is This For?

- Developers starting new .NET projects
- Teams wanting consistent project structure
- Anyone learning .NET best practices
- Developers who value their time

### 🚀 Future Plans

- NuGet package distribution
- Visual Studio extension
- More example projects (Blog, Task Management, etc.)
- GraphQL support
- Microservices templates

### 💭 Feedback Welcome

I'd love to hear your thoughts and suggestions. What features would you like to see? What patterns do you use in your APIs?

Try it out and let me know what you think!

---

**Tags:** #dotnet #csharp #api #opensource #cleanarchitecture #aspnetcore #webapi #crud #starter #boilerplate

**Links:**
- GitHub: https://github.com/yourusername/dotnet-crud-starter
- Documentation: https://github.com/yourusername/dotnet-crud-starter/docs
- Issues: https://github.com/yourusername/dotnet-crud-starter/issues