# Quick Start Guide - .NET CRUD Starter Kit

## 🚀 Get Started in 5 Minutes

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- PostgreSQL or SQL Server
- Git
- IDE (Visual Studio, VS Code, or Rider)

### 1. Clone and Setup

#### Using PowerShell (Windows)

```powershell
# Clone the starter kit
git clone https://github.com/yourusername/dotnet-crud-starter.git
cd dotnet-crud-starter

# Run setup for a Product API
./scripts/setup.ps1 -EntityName "Product" -PluralName "Products" -Namespace "MyStore.Api"
```

#### Using Bash (Linux/macOS)

```bash
# Clone the starter kit
git clone https://github.com/yourusername/dotnet-crud-starter.git
cd dotnet-crud-starter

# Make script executable and run setup
chmod +x scripts/setup.sh
./scripts/setup.sh -e Product -p Products -n MyStore.Api
```

### 2. Configure Database

Update `src/MyStore.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=mystoredb;Username=postgres;Password=yourpassword"
  }
}
```

### 3. Create Database

```bash
# Install EF Core tools if not already installed
dotnet tool install --global dotnet-ef

# Create initial migration
cd src/MyStore.Api
dotnet ef migrations add Initial -p ../MyStore.Infrastructure -s .

# Update database
dotnet ef database update
```

### 4. Run the API

```bash
dotnet run
```

Visit: https://localhost:5001/swagger

## 📝 Adding Custom Properties

### 1. Update Your Entity Model

Edit `src/MyStore.Core/Models/Product.cs`:

```csharp
public class Product : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    // Add your custom properties
    [Required]
    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
    
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
}
```

### 2. Update DTOs

Edit `src/MyStore.Core/DTOs/ProductDto.cs`:

```csharp
public class CreateProductDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
    
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;
}
```

### 3. Update AutoMapper Profile

Edit `src/MyStore.Application/Mappings/ProductProfile.cs`:

```csharp
public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductResponseDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}
```

### 4. Create Migration

```bash
dotnet ef migrations add AddProductProperties -p ../MyStore.Infrastructure -s .
dotnet ef database update
```

## 🔧 Common Customizations

### Enable File Upload

1. Add to your entity:
```csharp
public string[] ImageUrls { get; set; } = [];
```

2. Add to your controller:
```csharp
[HttpPost("{id}/upload")]
public async Task<IActionResult> UploadImages(Guid id, List<IFormFile> files)
{
    // Implementation using IFileStorageService
}
```

### Add Search Functionality

Override the search method in your controller:

```csharp
protected override Expression<Func<Product, bool>>? GetSearchFilter(string search)
{
    return p => p.Name.Contains(search) || 
                p.Description.Contains(search) || 
                p.SKU.Contains(search);
}
```

### Add Custom Sorting

Override the sorting method:

```csharp
protected override Func<IQueryable<Product>, IOrderedQueryable<Product>>? GetOrderByExpression(
    string sortBy, 
    bool descending)
{
    return sortBy?.ToLower() switch
    {
        "name" => descending ? q => q.OrderByDescending(p => p.Name) : q => q.OrderBy(p => p.Name),
        "price" => descending ? q => q.OrderByDescending(p => p.Price) : q => q.OrderBy(p => p.Price),
        "stock" => descending ? q => q.OrderByDescending(p => p.StockQuantity) : q => q.OrderBy(p => p.StockQuantity),
        _ => descending ? q => q.OrderByDescending(p => p.CreatedAt) : q => q.OrderBy(p => p.CreatedAt)
    };
}
```

### Add Business Logic

Create a service in `src/MyStore.Application/Services/`:

```csharp
public interface IProductService
{
    Task<bool> CheckStockAvailability(Guid productId, int quantity);
    Task<decimal> CalculateDiscountPrice(Guid productId, decimal discountPercentage);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    
    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<bool> CheckStockAvailability(Guid productId, int quantity)
    {
        var product = await _repository.GetByIdAsync(productId);
        return product?.StockQuantity >= quantity ?? false;
    }
}
```

## 🎯 Example API Requests

### Create Product
```http
POST /api/products
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Gaming Laptop",
  "description": "High-performance laptop for gaming",
  "price": 1299.99,
  "stockQuantity": 50,
  "sku": "LAPTOP-GAME-001"
}
```

### Get Products (Paginated)
```http
GET /api/products?page=1&pageSize=10&sortBy=price&sortDesc=true&search=laptop
```

### Update Product
```http
PUT /api/products/{id}
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Gaming Laptop Pro",
  "description": "Updated description",
  "price": 1499.99,
  "stockQuantity": 45,
  "sku": "LAPTOP-GAME-001"
}
```

## 🐳 Docker Support

Create a `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/MyStore.Api/MyStore.Api.csproj", "src/MyStore.Api/"]
COPY ["src/MyStore.Core/MyStore.Core.csproj", "src/MyStore.Core/"]
COPY ["src/MyStore.Infrastructure/MyStore.Infrastructure.csproj", "src/MyStore.Infrastructure/"]
COPY ["src/MyStore.Application/MyStore.Application.csproj", "src/MyStore.Application/"]
RUN dotnet restore "src/MyStore.Api/MyStore.Api.csproj"
COPY . .
WORKDIR "/src/src/MyStore.Api"
RUN dotnet build "MyStore.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyStore.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyStore.Api.dll"]
```

## 📚 Next Steps

- [Authentication Setup](./AUTHENTICATION.md)
- [File Upload Configuration](./FILE_UPLOAD.md)
- [API Best Practices](./API_CONVENTIONS.md)
- [Deployment Guide](./DEPLOYMENT.md)
- [Testing Guide](./TESTING.md)

## 🆘 Troubleshooting

### Database Connection Issues
- Ensure PostgreSQL/SQL Server is running
- Check connection string format
- Verify database user permissions

### Migration Errors
- Ensure all project references are correct
- Check for pending model changes
- Run `dotnet ef migrations remove` to undo last migration

### Build Errors
- Run `dotnet restore` to restore packages
- Check .NET SDK version with `dotnet --version`
- Clear obj and bin folders if needed

## 🤝 Getting Help

- Check the [documentation](./README.md)
- Search [existing issues](https://github.com/yourusername/dotnet-crud-starter/issues)
- Create a new issue with details
- Join our [Discord community](#)