# Starter Dotnet CRUD Project Templates - (Complaint API usecase)

This is a simple .NET 8 Web API starter boilerplate adapted for managing complaints. It provides endpoints for creating, retrieving, updating, and deleting complaints, as well as uploading images associated with complaints. This can be adapted to any domain that requires a similar CRUD functionality.

## Features

- CRUD operations for complaints
- Image upload to Azure Blob Storage
- Image retrieval from Azure Blob Storage
- JWT Authentication
- Pagination for complaint retrieval
- PostgreSQL database integration using Entity Framework Core
- Swagger/OpenAPI documentation
- Serilog logging
- CORS configuration

## Prerequisites

- .NET 8 SDK
- PostgreSQL database
- Azure Blob Storage account
- JWT secret key

## Setup

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/complaint_api.git
   cd complaint_api
   ```

2. Update the `appsettings.Development.json` file with your configuration:
   - set your local environment `export ASPNETCORE_ENVIRONMENT=Development` to use the `appsettings.Development.json` file
   - Set the PostgreSQL connection string in `ConnectionStrings:DefaultConnection`
   - Set the Azure Blob Storage connection string in `ConnectionStrings:AzureBlobStorage`
   - Configure the JWT settings in the `JWT` section
   - Adjust the CORS policy in `Program.cs` if needed

3. Run database migrations:
   ```
   dotnet ef database update
   ```

4. Run the application:
   ```
   dotnet run
   ```

5. Access the Swagger UI at `https://localhost:5001/swagger` (the port may vary)

## API Endpoints

- `GET /api/complaints`: Retrieve all complaints (paginated)
- `GET /api/complaints/{id}`: Retrieve a specific complaint
- `POST /api/complaints`: Create a new complaint
- `PUT /api/complaints/{id}`: Update an existing complaint
- `DELETE /api/complaints/{id}`: Delete a complaint
- `POST /api/complaints/{id}/upload`: Upload images/pdf for a complaint
- `GET /api/complaints/{id}/{fileName}`: Retrieve images/pdf for a complaint
- `GET /api/health`: Check the API health status

## Authentication

This API uses JWT for authentication. Include the JWT token in the Authorization header of your requests:

```
Authorization: Bearer your_jwt_token_here
```

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.