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

## Running Tests

To run the unit tests for this project, follow these steps:

1. Navigate to the test project directory:
   ```
   cd complaint_api.Tests
   ```

2. Run the tests using the dotnet test command:
   ```
   dotnet test
   ```

This will execute all the unit tests in the project and display the results in the console. Make sure all tests pass before making any changes to the codebase or deploying the application.

## API Endpoints

- `GET /api/complaints`: Retrieve all complaints (paginated)
- `GET /api/complaints/{id}`: Retrieve a specific complaint
- `POST /api/complaints`: Create a new complaint
- `PUT /api/complaints/{id}`: Update an existing complaint
- `DELETE /api/complaints/{id}`: Delete a complaint
- `POST /api/complaints/{id}/upload`: Upload images/pdf for a complaint
- `GET /api/complaints/{id}/{fileName}`: Retrieve images/pdf for a complaint
- `GET /api/health`: Check the API health status

## Complaint Model

The Complaint model has the following structure:

```csharp
public class Complaint
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public DateTime CreatedTime { get; set; }
    public string[] ImageUrls { get; set; }
}
```

## API Request and Response Examples

### Create a new complaint

Request:
```http
POST /api/complaints
Content-Type: application/json

{
  "description": "Issue with product delivery",
  "name": "John Doe"
}
```

Response:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "description": "Issue with product delivery",
  "name": "John Doe",
  "createdTime": "2023-04-21T10:30:00Z",
  "imageUrls": []
}
```

### Retrieve a specific complaint

Request:
```http
GET /api/complaints/3fa85f64-5717-4562-b3fc-2c963f66afa6
```

Response:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "description": "Issue with product delivery",
  "name": "John Doe",
  "createdTime": "2023-04-21T10:30:00Z",
  "imageUrls": ["https://example.blob.core.windows.net/complaints/image1.jpg"]
}
```

### Upload images for a complaint

Request:
```http
POST /api/complaints/3fa85f64-5717-4562-b3fc-2c963f66afa6/upload
Content-Type: multipart/form-data

[Binary file data]
```

Response:
```json
[
  "https://example.blob.core.windows.net/complaints/image1.jpg",
  "https://example.blob.core.windows.net/complaints/image2.jpg"
]
```

## Authentication

This API uses JWT for authentication. Include the JWT token in the Authorization header of your requests:

```
Authorization: Bearer your_jwt_token_here
```

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.