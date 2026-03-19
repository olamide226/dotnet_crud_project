#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Sets up a new .NET CRUD API project from the starter template
.DESCRIPTION
    This script initializes a new .NET CRUD API project by:
    - Creating project structure
    - Generating entity models and controllers
    - Setting up configuration files
    - Creating initial migrations
.PARAMETER EntityName
    The name of your main entity (singular, e.g., "Product")
.PARAMETER PluralName
    The plural form of your entity name (e.g., "Products")
.PARAMETER Namespace
    The root namespace for your project (e.g., "MyCompany.Api")
.PARAMETER SkipRestore
    Skip running dotnet restore
.PARAMETER SkipMigration
    Skip creating initial migration
.EXAMPLE
    ./setup.ps1 -EntityName "Product" -PluralName "Products" -Namespace "MyStore.Api"
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$EntityName,
    
    [Parameter(Mandatory=$false)]
    [string]$PluralName = "${EntityName}s",
    
    [Parameter(Mandatory=$true)]
    [string]$Namespace,
    
    [switch]$SkipRestore,
    [switch]$SkipMigration
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "🚀 .NET CRUD API Starter Kit Setup" -ForegroundColor Cyan
Write-Host "===================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Entity Name: $EntityName" -ForegroundColor Green
Write-Host "Plural Name: $PluralName" -ForegroundColor Green
Write-Host "Namespace: $Namespace" -ForegroundColor Green
Write-Host ""

# Validate inputs
if ($EntityName -match '\s') {
    throw "Entity name cannot contain spaces"
}

if ($Namespace -match '\s') {
    throw "Namespace cannot contain spaces"
}

# Function to replace placeholders in templates
function Replace-Placeholders {
    param(
        [string]$Content
    )
    
    $Content = $Content -replace '{{EntityName}}', $EntityName
    $Content = $Content -replace '{{PluralName}}', $PluralName
    $Content = $Content -replace '{{Namespace}}', $Namespace
    $Content = $Content -replace '{{entityName}}', ($EntityName.Substring(0,1).ToLower() + $EntityName.Substring(1))
    $Content = $Content -replace '{{pluralName}}', ($PluralName.Substring(0,1).ToLower() + $PluralName.Substring(1))
    
    return $Content
}

# Create directory structure
Write-Host "📁 Creating project structure..." -ForegroundColor Yellow

$directories = @(
    "src/$Namespace.Api/Controllers",
    "src/$Namespace.Api/Middleware",
    "src/$Namespace.Api/Filters",
    "src/$Namespace.Core/Models",
    "src/$Namespace.Core/DTOs",
    "src/$Namespace.Core/Interfaces",
    "src/$Namespace.Core/Constants",
    "src/$Namespace.Infrastructure/Data",
    "src/$Namespace.Infrastructure/Repositories",
    "src/$Namespace.Infrastructure/Services",
    "src/$Namespace.Application/Services",
    "src/$Namespace.Application/Validators",
    "src/$Namespace.Application/Mappings",
    "tests/$Namespace.UnitTests",
    "tests/$Namespace.IntegrationTests",
    "docs",
    ".github/workflows"
)

foreach ($dir in $directories) {
    New-Item -ItemType Directory -Force -Path $dir | Out-Null
}

Write-Host "✅ Directory structure created" -ForegroundColor Green

# Generate Entity Model
Write-Host "📝 Generating entity model..." -ForegroundColor Yellow

$entityTemplate = @'
using System;
using System.ComponentModel.DataAnnotations;
using DotnetCrud.Core.Models;

namespace {{Namespace}}.Core.Models
{
    public class {{EntityName}} : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        // TODO: Add your entity-specific properties here
        // Example:
        // public decimal Price { get; set; }
        // public int Quantity { get; set; }
        // public string Category { get; set; }
    }
}
'@

$entityContent = Replace-Placeholders -Content $entityTemplate
Set-Content -Path "src/$Namespace.Core/Models/$EntityName.cs" -Value $entityContent

# Generate DTOs
$createDtoTemplate = @'
using System.ComponentModel.DataAnnotations;

namespace {{Namespace}}.Core.DTOs
{
    public class Create{{EntityName}}Dto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        // TODO: Add properties that should be provided when creating
    }

    public class Update{{EntityName}}Dto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        // TODO: Add properties that can be updated
    }

    public class {{EntityName}}ResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // TODO: Add properties that should be returned in responses
    }
}
'@

$dtoContent = Replace-Placeholders -Content $createDtoTemplate
Set-Content -Path "src/$Namespace.Core/DTOs/${EntityName}Dto.cs" -Value $dtoContent

# Generate Controller
$controllerTemplate = @'
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using {{Namespace}}.Core.Models;
using {{Namespace}}.Core.DTOs;
using {{Namespace}}.Core.Interfaces;
using DotnetCrud.Api.Controllers.Base;
using AutoMapper;

namespace {{Namespace}}.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class {{PluralName}}Controller : BaseApiController<{{EntityName}}, Create{{EntityName}}Dto, Update{{EntityName}}Dto, {{EntityName}}ResponseDto>
    {
        public {{PluralName}}Controller(
            IRepository<{{EntityName}}> repository, 
            IMapper mapper, 
            ILogger<{{PluralName}}Controller> logger) 
            : base(repository, mapper, logger)
        {
        }
        
        // The base controller provides:
        // GET    /api/{{pluralName}}          - Get all (paginated)
        // GET    /api/{{pluralName}}/{id}     - Get by ID
        // POST   /api/{{pluralName}}          - Create new
        // PUT    /api/{{pluralName}}/{id}     - Update
        // DELETE /api/{{pluralName}}/{id}     - Delete
        
        // TODO: Add any custom endpoints specific to {{EntityName}} here
        
        // Example custom endpoint:
        // [HttpGet("by-name/{name}")]
        // public async Task<ActionResult<{{EntityName}}ResponseDto>> GetByName(string name)
        // {
        //     var entity = await _repository.FindAsync(e => e.Name == name);
        //     if (entity == null) return NotFound();
        //     return Ok(_mapper.Map<{{EntityName}}ResponseDto>(entity));
        // }
    }
}
'@

$controllerContent = Replace-Placeholders -Content $controllerTemplate
Set-Content -Path "src/$Namespace.Api/Controllers/${PluralName}Controller.cs" -Value $controllerContent

# Generate Repository
$repositoryTemplate = @'
using {{Namespace}}.Core.Models;
using {{Namespace}}.Core.Interfaces;
using {{Namespace}}.Infrastructure.Data;
using DotnetCrud.Infrastructure.Repositories;

namespace {{Namespace}}.Infrastructure.Repositories
{
    public class {{EntityName}}Repository : GenericRepository<{{EntityName}}>, I{{EntityName}}Repository
    {
        public {{EntityName}}Repository(ApplicationDbContext context) : base(context)
        {
        }
        
        // TODO: Add any custom repository methods specific to {{EntityName}} here
    }
}
'@

$repositoryContent = Replace-Placeholders -Content $repositoryTemplate
Set-Content -Path "src/$Namespace.Infrastructure/Repositories/${EntityName}Repository.cs" -Value $repositoryContent

# Generate Repository Interface
$repositoryInterfaceTemplate = @'
using {{Namespace}}.Core.Models;
using DotnetCrud.Core.Interfaces;

namespace {{Namespace}}.Core.Interfaces
{
    public interface I{{EntityName}}Repository : IRepository<{{EntityName}}>
    {
        // TODO: Add any custom repository method signatures here
    }
}
'@

$repositoryInterfaceContent = Replace-Placeholders -Content $repositoryInterfaceTemplate
Set-Content -Path "src/$Namespace.Core/Interfaces/I${EntityName}Repository.cs" -Value $repositoryInterfaceContent

# Update DbContext
Write-Host "📝 Updating DbContext..." -ForegroundColor Yellow

$dbContextTemplate = @'
using Microsoft.EntityFrameworkCore;
using {{Namespace}}.Core.Models;

namespace {{Namespace}}.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<{{EntityName}}> {{PluralName}} { get; set; }
        // TODO: Add other DbSets here

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // {{EntityName}} configuration
            modelBuilder.Entity<{{EntityName}}>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                
                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
                
                // TODO: Add additional entity configuration here
            });

            // TODO: Add other entity configurations here
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
'@

$dbContextContent = Replace-Placeholders -Content $dbContextTemplate
Set-Content -Path "src/$Namespace.Infrastructure/Data/ApplicationDbContext.cs" -Value $dbContextContent

# Create AutoMapper Profile
$mapperTemplate = @'
using AutoMapper;
using {{Namespace}}.Core.Models;
using {{Namespace}}.Core.DTOs;

namespace {{Namespace}}.Application.Mappings
{
    public class {{EntityName}}Profile : Profile
    {
        public {{EntityName}}Profile()
        {
            CreateMap<{{EntityName}}, {{EntityName}}ResponseDto>();
            CreateMap<Create{{EntityName}}Dto, {{EntityName}}>();
            CreateMap<Update{{EntityName}}Dto, {{EntityName}}>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
        }
    }
}
'@

$mapperContent = Replace-Placeholders -Content $mapperTemplate
Set-Content -Path "src/$Namespace.Application/Mappings/${EntityName}Profile.cs" -Value $mapperContent

# Create appsettings.json
Write-Host "📝 Creating configuration files..." -ForegroundColor Yellow

$appSettingsContent = @'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database={{Namespace}};Username=postgres;Password=yourpassword"
  },
  "JWT": {
    "Secret": "your-very-long-secret-key-at-least-32-characters",
    "Issuer": "{{Namespace}}",
    "Audience": "{{Namespace}}",
    "ExpirationInMinutes": 60
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000", "http://localhost:4200"]
  }
}
'@

$appSettingsJson = Replace-Placeholders -Content $appSettingsContent
Set-Content -Path "src/$Namespace.Api/appsettings.json" -Value $appSettingsJson

# Create README
$readmeTemplate = @'
# {{Namespace}}

A RESTful API built with .NET 8 using the CRUD Starter Kit.

## Quick Start

1. Update database connection string in `appsettings.json`
2. Run migrations: `dotnet ef database update`
3. Run the API: `dotnet run`
4. Access Swagger UI: https://localhost:5001/swagger

## API Endpoints

- `GET /api/{{pluralName}}` - Get all {{pluralName}} (paginated)
- `GET /api/{{pluralName}}/{id}` - Get {{entityName}} by ID
- `POST /api/{{pluralName}}` - Create new {{entityName}}
- `PUT /api/{{pluralName}}/{id}` - Update {{entityName}}
- `DELETE /api/{{pluralName}}/{id}` - Delete {{entityName}}

## Features

- JWT Authentication
- Pagination
- Soft Delete
- Audit Fields (CreatedAt, UpdatedAt)
- Swagger Documentation
- Global Error Handling
- Request/Response Logging

## Development

To add new properties to {{EntityName}}:

1. Update the model in `src/{{Namespace}}.Core/Models/{{EntityName}}.cs`
2. Update DTOs in `src/{{Namespace}}.Core/DTOs/{{EntityName}}Dto.cs`
3. Update AutoMapper profile if needed
4. Create a new migration: `dotnet ef migrations add AddNewProperties`
5. Update the database: `dotnet ef database update`
'@

$readmeContent = Replace-Placeholders -Content $readmeTemplate
Set-Content -Path "README.md" -Value $readmeContent

# Summary
Write-Host ""
Write-Host "✅ Setup completed successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Next Steps:" -ForegroundColor Cyan
Write-Host "1. Navigate to the project directory" -ForegroundColor White
Write-Host "2. Update the connection string in appsettings.json" -ForegroundColor White

if (-not $SkipRestore) {
    Write-Host "3. Running dotnet restore..." -ForegroundColor Yellow
    dotnet restore
}

if (-not $SkipMigration) {
    Write-Host "4. Create initial migration with:" -ForegroundColor White
    Write-Host "   dotnet ef migrations add Initial" -ForegroundColor Gray
}

Write-Host "5. Run the API with: dotnet run" -ForegroundColor White
Write-Host ""
Write-Host "🎉 Happy coding!" -ForegroundColor Magenta