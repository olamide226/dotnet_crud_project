# Implementation Guide - Transform to .NET CRUD Starter Kit

## Immediate Actions

### 1. Create Base Infrastructure

First, let's create the core base classes that will make this generic:

#### BaseEntity.cs
```csharp
namespace DotnetCrudApi.Core.Models
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
```

#### IRepository.cs
```csharp
namespace DotnetCrudApi.Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<(IEnumerable<T> Items, int TotalCount)> GetPaginatedAsync(int page, int pageSize);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
```

### 2. Create Generic Controller

#### BaseApiController.cs
```csharp
namespace DotnetCrudApi.Api.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public abstract class BaseApiController<TEntity, TCreateDto, TUpdateDto, TResponseDto> : ControllerBase
        where TEntity : BaseEntity
    {
        protected readonly IRepository<TEntity> _repository;
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;

        protected BaseApiController(IRepository<TEntity> repository, IMapper mapper, ILogger logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public virtual async Task<ActionResult<PaginatedResponse<TResponseDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var (items, totalCount) = await _repository.GetPaginatedAsync(page, pageSize);
            var dtos = _mapper.Map<List<TResponseDto>>(items);
            
            return Ok(new PaginatedResponse<TResponseDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TResponseDto>> GetById(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            
            return Ok(_mapper.Map<TResponseDto>(entity));
        }

        [HttpPost]
        public virtual async Task<ActionResult<TResponseDto>> Create(TCreateDto createDto)
        {
            var entity = _mapper.Map<TEntity>(createDto);
            var created = await _repository.AddAsync(entity);
            var responseDto = _mapper.Map<TResponseDto>(created);
            
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, responseDto);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(Guid id, TUpdateDto updateDto)
        {
            if (!await _repository.ExistsAsync(id)) return NotFound();
            
            var entity = await _repository.GetByIdAsync(id);
            _mapper.Map(updateDto, entity);
            await _repository.UpdateAsync(entity);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            if (!await _repository.ExistsAsync(id)) return NotFound();
            
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
```

### 3. Setup Script

Create a PowerShell script to transform the project:

#### setup.ps1
```powershell
param(
    [string]$EntityName = "Item",
    [string]$PluralName = "Items",
    [string]$Namespace = "MyCompany.Api"
)

Write-Host "🚀 Setting up .NET CRUD API for entity: $EntityName" -ForegroundColor Green

# Create directories
$directories = @(
    "src/Api/Controllers",
    "src/Core/Models",
    "src/Core/DTOs",
    "src/Core/Interfaces",
    "src/Infrastructure/Data",
    "src/Infrastructure/Services",
    "src/Application/Services",
    "tests/Unit",
    "tests/Integration"
)

foreach ($dir in $directories) {
    New-Item -ItemType Directory -Force -Path $dir | Out-Null
}

# Generate entity model
$entityContent = @"
using System;
using $Namespace.Core.Models;

namespace $Namespace.Core.Models
{
    public class $EntityName : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Add your custom properties here
    }
}
"@

# Generate DTOs
$createDtoContent = @"
namespace $Namespace.Core.DTOs
{
    public class Create${EntityName}Dto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
"@

# Write files
Set-Content -Path "src/Core/Models/$EntityName.cs" -Value $entityContent
Set-Content -Path "src/Core/DTOs/${EntityName}Dto.cs" -Value $createDtoContent

Write-Host "✅ Project structure created successfully!" -ForegroundColor Green
Write-Host "📝 Next steps:" -ForegroundColor Yellow
Write-Host "  1. Run 'dotnet restore'" -ForegroundColor White
Write-Host "  2. Update appsettings.json with your configuration" -ForegroundColor White
Write-Host "  3. Run 'dotnet ef migrations add Initial'" -ForegroundColor White
Write-Host "  4. Run 'dotnet run' to start the API" -ForegroundColor White
```

### 4. Quick Start Guide

#### QUICKSTART.md
```markdown
# Quick Start Guide

## Prerequisites
- .NET 8 SDK
- PostgreSQL or SQL Server
- Azure account (optional, for blob storage)

## Setup Steps

### 1. Clone and Configure

```bash
# Clone the starter kit
git clone https://github.com/yourusername/dotnet-crud-starter.git my-api
cd my-api

# Run setup script (PowerShell)
./scripts/setup.ps1 -EntityName "Product" -PluralName "Products" -Namespace "MyStore.Api"

# Or on Linux/Mac
./scripts/setup.sh Product Products MyStore.Api
```

### 2. Configure Database

Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=mystore;Username=postgres;Password=yourpassword"
  }
}
```

### 3. Run Migrations

```bash
dotnet ef migrations add Initial
dotnet ef database update
```

### 4. Run the API

```bash
dotnet run
```

Visit https://localhost:5001/swagger

## Customization

### Add Properties to Your Entity

Edit `src/Core/Models/Product.cs`:
```csharp
public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }  // Added
    public int Stock { get; set; }      // Added
}
```

### Add Validation

Edit `src/Core/DTOs/ProductDto.cs`:
```csharp
public class CreateProductDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }
}
```

## Features

- ✅ JWT Authentication
- ✅ File Upload (Azure Blob)
- ✅ Pagination
- ✅ Swagger Documentation
- ✅ Global Error Handling
- ✅ Logging (Serilog)
- ✅ Health Checks
- ✅ Docker Support
```

### 5. Migration Steps for Current Project

1. **Create new branch**
   ```bash
   git checkout -b feature/generic-starter-kit
   ```

2. **Restructure folders**
   ```
   complaint_api/ → src/Api/
   Models/ → src/Core/Models/
   DTOs/ → src/Core/DTOs/
   Services/ → src/Infrastructure/Services/
   ```

3. **Create base classes**
   - BaseEntity.cs
   - BaseApiController.cs
   - GenericRepository.cs

4. **Refactor Complaint to use base classes**
   - Complaint : BaseEntity
   - ComplaintsController : BaseApiController<Complaint, CreateComplaintDto, UpdateComplaintDto, ComplaintResponseDto>

5. **Create setup automation**
   - PowerShell script for Windows
   - Bash script for Linux/Mac
   - Template configuration file

6. **Update documentation**
   - Generic README
   - Setup guide
   - Customization guide

## Benefits of This Approach

1. **Rapid Development**: New APIs in minutes
2. **Consistency**: All APIs follow same patterns
3. **Best Practices**: Built-in from start
4. **Flexibility**: Easy to customize
5. **Maintainability**: Clear structure
6. **Testability**: Test templates included

## Next Steps

1. Start with creating the base infrastructure
2. Refactor one controller to use the generic base
3. Create the setup script
4. Test with a new entity type
5. Document the process