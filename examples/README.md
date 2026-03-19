# Example Projects

This directory contains example implementations using the .NET CRUD Starter Kit.

## 1. E-Commerce API

A complete e-commerce backend with:
- Product management
- Categories
- Inventory tracking
- Featured products
- Price filtering
- Stock checking

### Features Demonstrated
- Custom search endpoints
- Business logic in controllers
- Complex filtering
- Public endpoints (featured products)
- Stock validation

### Key Endpoints
- `GET /api/products` - Browse products with pagination
- `GET /api/products/search` - Advanced search with filters
- `GET /api/products/featured` - Get featured products (no auth)
- `GET /api/products/{id}/check-stock` - Check stock availability
- `GET /api/categories` - Product categories

### Try It Out
```bash
cd ecommerce-api
dotnet restore
dotnet ef database update
dotnet run
```

## 2. Blog API (Coming Soon)

A blogging platform with:
- Posts with rich content
- Comments system
- Author profiles
- Tags and categories
- Draft/Published states

## 3. Task Management API (Coming Soon)

Project management system with:
- Projects and tasks
- User assignments
- Status tracking
- Due dates
- File attachments

## Common Patterns

Each example demonstrates:
- Clean architecture principles
- Repository pattern usage
- Custom business logic
- API best practices
- Comprehensive testing
- Documentation

## Creating Your Own

To create a new API based on these examples:

1. Run the setup script:
   ```bash
   ./scripts/setup.sh -e YourEntity -p YourEntities -n YourNamespace
   ```

2. Add your domain-specific properties

3. Implement custom endpoints as needed

4. Add business logic and validation

5. Write tests

6. Document your API

## Learning Resources

- [Architecture Guide](../docs/ARCHITECTURE.md)
- [Best Practices](../docs/BEST_PRACTICES.md)
- [Testing Guide](../docs/TESTING.md)
- [Deployment Guide](../docs/DEPLOYMENT.md)