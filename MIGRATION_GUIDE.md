# Migration Guide - Complaint API to Generic Starter Kit

## Overview

This guide will help you transform the existing Complaint API into a generic, reusable starter kit that can be adapted for any domain.

## Phase 1: Project Restructuring

### 1.1 Create New Solution Structure

```bash
# Create new directory structure
mkdir -p src/DotnetCrud.Api
mkdir -p src/DotnetCrud.Core
mkdir -p src/DotnetCrud.Infrastructure
mkdir -p src/DotnetCrud.Application

# Move existing files
mv complaint_api/* src/DotnetCrud.Api/
mv complaint_api.Tests tests/DotnetCrud.UnitTests
mv complaint_api.IntegrationTests tests/DotnetCrud.IntegrationTests
```

### 1.2 Update Project Files

1. **Rename project files:**
   ```bash
   mv src/DotnetCrud.Api/complaint_api.csproj src/DotnetCrud.Api/DotnetCrud.Api.csproj
   mv src/DotnetCrud.Api/complaint_api.sln DotnetCrud.sln
   ```

2. **Update namespaces** in all .cs files:
   - `complaint_api` → `DotnetCrud.Api`
   - `complaint_api.Models` → `DotnetCrud.Core.Models`
   - `complaint_api.Services` → `DotnetCrud.Infrastructure.Services`

### 1.3 Create Core Project

```xml
<!-- src/DotnetCrud.Core/DotnetCrud.Core.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

### 1.4 Create Infrastructure Project

```xml
<!-- src/DotnetCrud.Infrastructure/DotnetCrud.Infrastructure.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\DotnetCrud.Core\DotnetCrud.Core.csproj" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
    <PackageReference Include="Azure.Storage.Blobs" Version="12.19.1" />
  </ItemGroup>
</Project>
```

## Phase 2: Extract Base Classes

### 2.1 Create BaseEntity

1. Move common properties from `Complaint.cs` to `BaseEntity.cs`
2. Update `Complaint` to inherit from `BaseEntity`

```csharp
// Before (Complaint.cs)
public class Complaint
{
    public Guid Id { get; set; }
    public DateTime CreatedTime { get; set; }
    // ...
}

// After (BaseEntity.cs in Core)
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}

// After (Complaint.cs)
public class Complaint : BaseEntity
{
    public string Description { get; set; }
    public string Name { get; set; }
    public string[] ImageUrls { get; set; }
}
```

### 2.2 Create Generic Repository

1. Extract interface from existing data access code
2. Create `GenericRepository<T>` implementation
3. Update `ComplaintsController` to use repository

```csharp
// Move data access from controller to repository
public class ComplaintRepository : GenericRepository<Complaint>, IComplaintRepository
{
    public ComplaintRepository(ApplicationDbContext context) : base(context)
    {
    }
}
```

### 2.3 Create Base Controller

1. Extract common CRUD operations from `ComplaintsController`
2. Create `BaseApiController<TEntity, TCreateDto, TUpdateDto, TResponseDto>`
3. Refactor `ComplaintsController` to inherit from base

```csharp
// Before
public class ComplaintsController : ControllerBase
{
    // All CRUD methods inline
}

// After
public class ComplaintsController : BaseApiController<Complaint, CreateComplaintDto, UpdateComplaintDto, ComplaintResponseDto>
{
    // Only custom methods (upload, etc.)
}
```

## Phase 3: Make Services Generic

### 3.1 File Storage Service

1. Rename `IAzureBlobService` to `IFileStorageService`
2. Create provider-agnostic interface
3. Implement multiple providers (Local, Azure, AWS)

```csharp
// Before
public interface IAzureBlobService
{
    Task<List<string>> UploadFilesAsync(Guid complaintId, List<IFormFile> files);
}

// After
public interface IFileStorageService
{
    Task<string> UploadFileAsync(IFormFile file, string containerName, string fileName);
    Task<Stream> DownloadFileAsync(string fileUrl);
    // ...
}
```

### 3.2 Update Dependency Injection

```csharp
// Before
services.AddSingleton<IAzureBlobService, AzureBlobService>();

// After
services.AddScoped<IFileStorageService>(provider =>
{
    var storageType = configuration["FileStorage:Provider"];
    return storageType switch
    {
        "AzureBlob" => new AzureBlobStorageService(configuration),
        "Local" => new LocalFileStorageService(configuration),
        _ => throw new NotSupportedException($"Storage provider {storageType} not supported")
    };
});
```

## Phase 4: Create Setup Scripts

### 4.1 PowerShell Script

Create `scripts/setup.ps1` that:
- Accepts entity name, plural name, namespace
- Generates entity model
- Generates DTOs
- Generates controller
- Updates DbContext

### 4.2 Bash Script

Create `scripts/setup.sh` with same functionality for Linux/Mac users

## Phase 5: Update Configuration

### 5.1 Make Configuration Generic

```json
// Before (appsettings.json)
{
  "AzureBlobStorage": {
    "ContainerName": "complaints"
  }
}

// After
{
  "FileStorage": {
    "Provider": "Local",
    "Local": {
      "BasePath": "./uploads"
    },
    "AzureBlob": {
      "ConnectionString": "...",
      "ContainerPrefix": "myapp"
    }
  }
}
```

## Phase 6: Documentation

### 6.1 Create Generic Documentation

1. Update README.md to be domain-agnostic
2. Create QUICKSTART.md guide
3. Create CUSTOMIZATION.md for extending the kit
4. Add API_CONVENTIONS.md

### 6.2 Add Examples

Create example implementations:
- E-commerce (Product, Order, Customer)
- Blog (Post, Comment, Author)
- Task Management (Project, Task, User)

## Phase 7: Testing

### 7.1 Create Test Templates

1. Base test classes for controllers
2. Base test classes for repositories
3. Integration test base class
4. Test data builders

### 7.2 Update Existing Tests

Refactor existing tests to use base classes and be more generic

## Migration Checklist

- [ ] **Project Structure**
  - [ ] Create new solution structure
  - [ ] Move files to appropriate projects
  - [ ] Update namespaces
  - [ ] Update project references

- [ ] **Base Classes**
  - [ ] Create BaseEntity
  - [ ] Create IRepository interface
  - [ ] Create GenericRepository
  - [ ] Create BaseApiController
  - [ ] Create base DTOs

- [ ] **Refactor Existing Code**
  - [ ] Update Complaint to inherit BaseEntity
  - [ ] Extract repository from controller
  - [ ] Refactor controller to use base
  - [ ] Make file service generic

- [ ] **Setup Automation**
  - [ ] Create PowerShell script
  - [ ] Create Bash script
  - [ ] Create templates
  - [ ] Test script functionality

- [ ] **Configuration**
  - [ ] Update appsettings.json structure
  - [ ] Add provider selection
  - [ ] Environment-specific configs

- [ ] **Documentation**
  - [ ] Update README
  - [ ] Create quick start guide
  - [ ] Add customization guide
  - [ ] Create examples

- [ ] **Testing**
  - [ ] Create test base classes
  - [ ] Update existing tests
  - [ ] Add integration tests
  - [ ] Verify all tests pass

## Common Issues and Solutions

### Issue: Namespace conflicts
**Solution:** Use global search/replace with regex patterns

### Issue: Breaking changes in API
**Solution:** Create migration guide for existing APIs

### Issue: Database migrations
**Solution:** Create fresh migration after restructuring

### Issue: Lost Git history
**Solution:** Use `git mv` commands to preserve history

## Next Steps After Migration

1. **Test the starter kit** by creating a new project
2. **Create a demo video** showing setup process
3. **Publish as NuGet template** (optional)
4. **Set up CI/CD** for the starter kit
5. **Create community guidelines** for contributions

## Rollback Plan

If issues arise during migration:
1. Keep original `complaint_api` branch
2. Document all changes in commits
3. Test each phase independently
4. Have backup of database
5. Maintain compatibility layer if needed