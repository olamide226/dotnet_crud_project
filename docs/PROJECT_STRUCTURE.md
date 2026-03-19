# Project Structure

## Overview

The .NET CRUD Starter Kit follows Clean Architecture principles with clear separation of concerns across multiple projects.

```
dotnet-crud-starter/
├── src/                              # Source code
│   ├── DotnetCrud.Api/              # Web API project
│   ├── DotnetCrud.Core/             # Core business logic and entities
│   ├── DotnetCrud.Infrastructure/   # External concerns (DB, File Storage, etc.)
│   └── DotnetCrud.Application/      # Application services and use cases
├── tests/                           # Test projects
│   ├── DotnetCrud.UnitTests/       # Unit tests
│   └── DotnetCrud.IntegrationTests/ # Integration tests
├── scripts/                         # Setup and utility scripts
│   ├── setup.ps1                    # PowerShell setup script
│   ├── setup.sh                     # Bash setup script
│   └── templates/                   # Code generation templates
├── docs/                            # Documentation
├── .github/                         # GitHub specific files
│   └── workflows/                   # CI/CD workflows
└── docker/                          # Docker related files
```

## Project Details

### DotnetCrud.Api

The entry point of the application. Contains:

- **Controllers/** - API endpoints
  - `Base/` - Base controller with CRUD operations
  - Entity-specific controllers
- **Middleware/** - Custom middleware
  - Global exception handling
  - Request logging
  - Authentication
- **Filters/** - Action filters
- **Program.cs** - Application startup and configuration
- **appsettings.json** - Configuration files

### DotnetCrud.Core

The heart of the application. Contains:

- **Models/** - Domain entities
  - `BaseEntity.cs` - Base class for all entities
  - Entity-specific models
- **DTOs/** - Data Transfer Objects
  - `BaseDtos.cs` - Base DTO interfaces and classes
  - `PaginatedResponse.cs` - Pagination wrapper
  - Entity-specific DTOs
- **Interfaces/** - Core interfaces
  - `IRepository.cs` - Repository pattern interface
  - `IFileStorageService.cs` - File storage abstraction
  - Service interfaces
- **Constants/** - Application constants
- **Enums/** - Enumerations

### DotnetCrud.Infrastructure

Implementation of external concerns:

- **Data/** - Database related
  - `ApplicationDbContext.cs` - EF Core context
  - `Configurations/` - Entity configurations
  - `Migrations/` - Database migrations
- **Repositories/** - Repository implementations
  - `GenericRepository.cs` - Generic repository base
  - Entity-specific repositories
- **Services/** - External service implementations
  - `FileStorage/` - File storage providers
    - `LocalFileStorageService.cs`
    - `AzureBlobStorageService.cs`
    - `S3StorageService.cs` (optional)
  - Email, SMS, etc. services

### DotnetCrud.Application

Application-specific business logic:

- **Services/** - Business services
  - Entity-specific services
  - Cross-cutting services
- **Validators/** - Input validation
  - FluentValidation validators
- **Mappings/** - AutoMapper profiles
  - Entity to DTO mappings
- **Handlers/** - CQRS handlers (optional)

## Key Files

### Base Classes

#### BaseEntity.cs
```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
```

#### BaseApiController.cs
```csharp
public abstract class BaseApiController<TEntity, TCreateDto, TUpdateDto, TResponseDto>
{
    // Generic CRUD operations
    // GET, GET/{id}, POST, PUT/{id}, DELETE/{id}
}
```

#### GenericRepository.cs
```csharp
public class GenericRepository<T> : IRepository<T> where T : BaseEntity
{
    // Generic data access methods
    // Supports pagination, filtering, sorting
}
```

## Configuration Files

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "connection-string"
  },
  "JWT": {
    "Secret": "secret-key",
    "Issuer": "issuer",
    "Audience": "audience"
  },
  "FileStorage": {
    "Provider": "Local|AzureBlob|S3",
    "Local": { ... },
    "AzureBlob": { ... }
  }
}
```

### Entity Configuration Example
```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
```

## Adding New Features

### 1. Add New Entity

1. Create model in `Core/Models/`
2. Create DTOs in `Core/DTOs/`
3. Create repository interface in `Core/Interfaces/`
4. Implement repository in `Infrastructure/Repositories/`
5. Create controller in `Api/Controllers/`
6. Add DbSet to ApplicationDbContext
7. Create and run migration

### 2. Add New Service

1. Create interface in `Core/Interfaces/`
2. Implement in `Infrastructure/Services/` or `Application/Services/`
3. Register in Program.cs
4. Inject and use in controllers

### 3. Add New Middleware

1. Create middleware class in `Api/Middleware/`
2. Register in Program.cs pipeline
3. Configure order carefully

## Best Practices

1. **Separation of Concerns**
   - Keep business logic in Core
   - Keep infrastructure concerns in Infrastructure
   - Keep API concerns in Api

2. **Dependency Direction**
   - Api → Application → Core ← Infrastructure
   - Core should have no dependencies

3. **Testing**
   - Unit test Core and Application
   - Integration test Api and Infrastructure

4. **Configuration**
   - Use strongly typed configuration
   - Keep secrets out of source control
   - Use environment-specific settings

5. **Naming Conventions**
   - Entities: Singular (Product, Order)
   - Controllers: Plural (ProductsController)
   - DTOs: Purpose-specific (CreateProductDto)

## Common Patterns

### Repository Pattern
- Abstracts data access
- Enables unit testing
- Supports multiple data sources

### Unit of Work Pattern
- Manages transactions
- Ensures data consistency
- Coordinates multiple repositories

### CQRS Pattern (Optional)
- Separates read and write operations
- Enables optimization
- Supports event sourcing

### Mediator Pattern (Optional)
- Decouples controllers from services
- Enables cross-cutting concerns
- Simplifies testing