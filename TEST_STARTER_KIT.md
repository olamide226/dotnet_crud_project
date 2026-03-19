# Testing the .NET CRUD Starter Kit

## Test Plan for Starter Kit Validation

### 1. Create a New Project - E-commerce Example

Let's test the starter kit by creating a complete e-commerce API:

```bash
# Test 1: Product Management API
./scripts/setup.sh -e Product -p Products -n ECommerce.Api

# Test 2: Add Category entity
./scripts/setup.sh -e Category -p Categories -n ECommerce.Api

# Test 3: Add Order entity
./scripts/setup.sh -e Order -p Orders -n ECommerce.Api
```

### 2. Verify Generated Files

Check that all files are created correctly:

```bash
# Check structure
tree src/ECommerce.Api -L 3
tree src/ECommerce.Core -L 3
tree src/ECommerce.Infrastructure -L 3

# Verify key files exist
ls -la src/ECommerce.Core/Models/Product.cs
ls -la src/ECommerce.Api/Controllers/ProductsController.cs
ls -la src/ECommerce.Core/DTOs/ProductDto.cs
```

### 3. Test Compilation

```bash
cd src/ECommerce.Api
dotnet restore
dotnet build

# Should compile without errors
```

### 4. Test Database Operations

```bash
# Create and apply migration
dotnet ef migrations add InitialCreate -p ../ECommerce.Infrastructure -s .
dotnet ef database update

# Verify tables created
```

### 5. Test API Operations

```bash
# Run the API
dotnet run

# In another terminal, test endpoints
# Create a product
curl -X POST https://localhost:5001/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Gaming Laptop",
    "description": "High-performance laptop"
  }'

# Get all products
curl https://localhost:5001/api/products

# Get specific product
curl https://localhost:5001/api/products/{id}

# Update product
curl -X PUT https://localhost:5001/api/products/{id} \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Gaming Laptop Pro",
    "description": "Updated description"
  }'

# Delete product
curl -X DELETE https://localhost:5001/api/products/{id}
```

### 6. Test Advanced Features

#### 6.1 Pagination
```bash
curl "https://localhost:5001/api/products?page=1&pageSize=10&sortBy=name&sortDesc=false"
```

#### 6.2 Search
```bash
# After implementing search in ProductsController
curl "https://localhost:5001/api/products?search=laptop"
```

#### 6.3 File Upload
```bash
# Test file upload endpoint
curl -X POST https://localhost:5001/api/products/{id}/upload \
  -F "files=@/path/to/image.jpg" \
  -F "files=@/path/to/document.pdf"
```

### 7. Test Authentication

```bash
# Register user
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "Test123!@#"
  }'

# Login
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "Test123!@#"
  }'

# Use token in subsequent requests
curl https://localhost:5001/api/products \
  -H "Authorization: Bearer {token}"
```

### 8. Test Multiple Entities

Create a complete e-commerce system:

```bash
# Already created Product

# Add Category
./scripts/setup.sh -e Category -p Categories -n ECommerce.Api

# Add Customer
./scripts/setup.sh -e Customer -p Customers -n ECommerce.Api

# Add Order
./scripts/setup.sh -e Order -p Orders -n ECommerce.Api

# Update models to add relationships
```

### 9. Performance Testing

```bash
# Install Apache Bench
apt-get install apache2-utils

# Test concurrent requests
ab -n 1000 -c 100 https://localhost:5001/api/products

# Monitor response times and throughput
```

### 10. Test Docker Support

```bash
# Build Docker image
docker build -t ecommerce-api .

# Run container
docker run -p 8080:80 -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;..." ecommerce-api

# Test API through Docker
curl http://localhost:8080/api/products
```

## Validation Checklist

### Basic Functionality
- [ ] Project generates without errors
- [ ] All files created in correct locations
- [ ] Project compiles successfully
- [ ] Database migrations work
- [ ] All CRUD operations function correctly
- [ ] Pagination works
- [ ] Sorting works
- [ ] Soft delete works
- [ ] Audit fields are populated

### Advanced Features
- [ ] Authentication works
- [ ] File upload/download works
- [ ] Search functionality works
- [ ] Custom endpoints can be added
- [ ] Multiple entities can coexist
- [ ] Relationships can be configured

### Code Quality
- [ ] No compiler warnings
- [ ] Follows naming conventions
- [ ] Proper separation of concerns
- [ ] Dependency injection works
- [ ] Logging functions correctly

### Documentation
- [ ] README is accurate
- [ ] Setup instructions work
- [ ] API documentation generates
- [ ] Examples compile and run

### Testing
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Can add custom tests
- [ ] Test coverage is adequate

## Test Scenarios

### Scenario 1: Blog Platform
```bash
./scripts/setup.sh -e Post -p Posts -n Blog.Api
./scripts/setup.sh -e Author -p Authors -n Blog.Api
./scripts/setup.sh -e Comment -p Comments -n Blog.Api
./scripts/setup.sh -e Tag -p Tags -n Blog.Api
```

### Scenario 2: Task Management
```bash
./scripts/setup.sh -e Project -p Projects -n TaskManager.Api
./scripts/setup.sh -e Task -p Tasks -n TaskManager.Api
./scripts/setup.sh -e User -p Users -n TaskManager.Api
./scripts/setup.sh -e Team -p Teams -n TaskManager.Api
```

### Scenario 3: Inventory System
```bash
./scripts/setup.sh -e Item -p Items -n Inventory.Api
./scripts/setup.sh -e Warehouse -p Warehouses -n Inventory.Api
./scripts/setup.sh -e Supplier -p Suppliers -n Inventory.Api
./scripts/setup.sh -e StockMovement -p StockMovements -n Inventory.Api
```

## Edge Cases to Test

1. **Entity with Complex Name**
   ```bash
   ./scripts/setup.sh -e ProductCategory -p ProductCategories -n Test.Api
   ```

2. **Entity with Reserved Keyword**
   ```bash
   ./scripts/setup.sh -e System -p Systems -n Test.Api  # Should handle gracefully
   ```

3. **Very Long Entity Name**
   ```bash
   ./scripts/setup.sh -e VeryLongEntityNameThatMightCauseIssues -p VeryLongEntityNames -n Test.Api
   ```

4. **Special Characters in Namespace**
   ```bash
   ./scripts/setup.sh -e Product -p Products -n "My-Company.Api"  # Should validate
   ```

## Performance Benchmarks

Expected performance metrics:
- Setup time: < 5 seconds
- Build time: < 10 seconds
- API startup: < 3 seconds
- CRUD operation: < 100ms
- Pagination (1000 records): < 200ms

## Success Criteria

The starter kit is considered successful if:
1. New API can be created in under 5 minutes
2. All basic CRUD operations work out of the box
3. Can extend with custom properties easily
4. Can add business logic without modifying base classes
5. Tests can be added and run successfully
6. Can deploy to production environments
7. Documentation is clear and accurate