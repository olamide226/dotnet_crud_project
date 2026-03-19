# ECommerce.Api

A RESTful API built with .NET 8 using the CRUD Starter Kit.

## Quick Start

1. Update database connection string in `appsettings.json`
2. Run migrations: `dotnet ef database update`
3. Run the API: `dotnet run`
4. Access Swagger UI: https://localhost:5001/swagger

## API Endpoints

- `GET /api/categories` - Get all categories (paginated)
- `GET /api/categories/{id}` - Get category by ID
- `POST /api/categories` - Create new category
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category

## Features

- JWT Authentication
- Pagination
- Soft Delete
- Audit Fields (CreatedAt, UpdatedAt)
- Swagger Documentation
- Global Error Handling
- Request/Response Logging

## Development

To add new properties to Category:

1. Update the model in `src/ECommerce.Api.Core/Models/Category.cs`
2. Update DTOs in `src/ECommerce.Api.Core/DTOs/CategoryDto.cs`
3. Update AutoMapper profile if needed
4. Create a new migration: `dotnet ef migrations add AddNewProperties`
5. Update the database: `dotnet ef database update`
