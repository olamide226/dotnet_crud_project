# E-Commerce API Example

A complete e-commerce backend built with the .NET CRUD Starter Kit demonstrating real-world patterns and features.

## Features

- **Product Management**: Full CRUD operations for products
- **Categories**: Organize products by categories
- **Inventory Tracking**: Real-time stock management
- **Search & Filtering**: Advanced product search with multiple filters
- **Featured Products**: Showcase special products
- **Price Management**: Support for price ranges and discounts
- **Image Management**: Multiple images per product

## Project Structure

```
src/
├── ECommerce.Api.Api/           # Web API controllers
├── ECommerce.Api.Core/          # Domain models and interfaces  
├── ECommerce.Api.Infrastructure/# Data access and external services
└── ECommerce.Api.Application/   # Business logic and mappings
```

## Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL or SQL Server
- Optional: Docker for containerized database

### Setup

1. **Update Connection String**
   
   Edit `src/ECommerce.Api.Api/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=ecommerce;Username=postgres;Password=yourpassword"
     }
   }
   ```

2. **Install Dependencies**
   ```bash
   cd src/ECommerce.Api.Api
   dotnet restore
   ```

3. **Create Database**
   ```bash
   dotnet ef migrations add InitialCreate -p ../ECommerce.Api.Infrastructure -s .
   dotnet ef database update
   ```

4. **Run the API**
   ```bash
   dotnet run
   ```

5. **Access Swagger**
   
   Open: https://localhost:5001/swagger

## API Endpoints

### Products

#### Get All Products
```http
GET /api/products?page=1&pageSize=20&sortBy=price&sortDesc=true
```

#### Search Products
```http
GET /api/products/search?term=laptop&category=electronics&minPrice=500&maxPrice=2000
```

#### Get Featured Products
```http
GET /api/products/featured
```
*Note: This endpoint doesn't require authentication*

#### Create Product
```http
POST /api/products
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Gaming Laptop Pro",
  "description": "High-performance laptop for gaming and development",
  "price": 1499.99,
  "stockQuantity": 25,
  "sku": "LAPTOP-001",
  "category": "Electronics",
  "isFeatured": true
}
```

#### Check Stock
```http
GET /api/products/{id}/check-stock?quantity=5
```

Response:
```json
{
  "available": true,
  "currentStock": 25,
  "requestedQuantity": 5
}
```

### Categories

#### Get All Categories
```http
GET /api/categories
```

#### Create Category
```http
POST /api/categories
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Electronics",
  "description": "Electronic devices and accessories"
}
```

## Advanced Features

### Custom Search Implementation

The ProductsController demonstrates advanced search with multiple filters:

```csharp
// Search by term, category, and price range
GET /api/products/search?term=gaming&category=electronics&minPrice=1000&maxPrice=2000
```

### Stock Management

Real-time stock checking prevents overselling:

```csharp
// Check if quantity is available before order
GET /api/products/{id}/check-stock?quantity=10
```

### Featured Products

Public endpoint for showcasing products:

```csharp
// No authentication required
GET /api/products/featured
```

## Testing

### Unit Tests
```bash
cd tests/ECommerce.Api.UnitTests
dotnet test
```

### Integration Tests
```bash
cd tests/ECommerce.Api.IntegrationTests
dotnet test
```

### Manual Testing with Postman

Import the Postman collection from `postman/ECommerce.Api.postman_collection.json`

## Deployment

### Docker

```bash
docker build -t ecommerce-api .
docker run -p 8080:80 -e ConnectionStrings__DefaultConnection="..." ecommerce-api
```

### Azure App Service

```bash
az webapp up --name my-ecommerce-api --resource-group mygroup
```

## Extending the API

### Add Reviews System

1. Create Review entity:
   ```bash
   ./scripts/setup.sh -e Review -p Reviews -n ECommerce.Api
   ```

2. Add relationship to Product:
   ```csharp
   public class Product : BaseEntity
   {
       // ... existing properties
       public virtual ICollection<Review> Reviews { get; set; }
   }
   ```

3. Add custom endpoints in ReviewsController

### Add Shopping Cart

1. Create Cart and CartItem entities
2. Implement session-based or user-based carts
3. Add checkout process

### Add Order Management

1. Create Order entity
2. Implement order workflow
3. Add payment integration

## Performance Considerations

- **Pagination**: Always use pagination for list endpoints
- **Eager Loading**: Use Include() for related data
- **Caching**: Implement caching for frequently accessed data
- **Indexing**: Add database indexes for search fields

## Security

- JWT authentication for protected endpoints
- Role-based access control ready
- Input validation on all DTOs
- SQL injection prevention through EF Core

## Monitoring

- Health check endpoint: `/health`
- Structured logging with Serilog
- Request/response logging
- Performance metrics

## Contributing

Feel free to extend this example and submit PRs to showcase more patterns!