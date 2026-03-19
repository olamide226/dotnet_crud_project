# Demo Video Script - .NET CRUD Starter Kit

## Video Title: "Create a Production-Ready .NET API in 5 Minutes"

### Duration: ~10 minutes

---

## 1. Introduction (30 seconds)

**Scene:** VS Code with terminal open

**Script:**
"Hi developers! Today I'm going to show you how to create a production-ready .NET 8 API in just 5 minutes using our CRUD Starter Kit. This isn't just a basic template - it includes authentication, file uploads, pagination, and everything you need for a real-world API. Let's dive in!"

**Actions:**
- Show the GitHub repository
- Highlight key features on screen

---

## 2. Setup Process (2 minutes)

**Scene:** Terminal

**Script:**
"First, let's clone the starter kit and create our e-commerce API. I'll create a product management system with full CRUD operations."

**Commands:**
```bash
# Clone the repository
git clone https://github.com/yourusername/dotnet-crud-starter.git
cd dotnet-crud-starter

# Run the setup script
./scripts/setup.sh -e Product -p Products -n MyStore.Api

# Show the generated structure
tree src/ -L 3
```

**Highlight:**
- Show files being created
- Explain the clean architecture
- Point out key files generated

---

## 3. Configuration (1 minute)

**Scene:** VS Code - appsettings.json

**Script:**
"Now let's configure our database connection. The starter kit supports PostgreSQL, SQL Server, and MySQL out of the box."

**Actions:**
- Open appsettings.json
- Update connection string
- Show JWT configuration
- Mention environment-specific configs

---

## 4. Adding Custom Properties (2 minutes)

**Scene:** VS Code - Product.cs

**Script:**
"Let's make this a real product API by adding custom properties like price, stock, and category."

**Code:**
```csharp
public class Product : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    // Adding custom properties
    [Required]
    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }
    
    [Required]
    public int StockQuantity { get; set; }
    
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;
    
    public bool IsFeatured { get; set; }
}
```

**Actions:**
- Update DTOs to match
- Show AutoMapper profile
- Explain validation attributes

---

## 5. Database Setup (1 minute)

**Scene:** Terminal

**Script:**
"Let's create our database. The starter kit uses Entity Framework Core with code-first migrations."

**Commands:**
```bash
cd src/MyStore.Api

# Create migration
dotnet ef migrations add InitialCreate -p ../MyStore.Infrastructure -s .

# Update database
dotnet ef database update

# Show created tables
```

---

## 6. Running the API (2 minutes)

**Scene:** Terminal + Browser

**Script:**
"Time to see our API in action! Let's start it up and explore the Swagger documentation."

**Commands:**
```bash
dotnet run
```

**Browser Actions:**
- Navigate to https://localhost:5001/swagger
- Show all endpoints
- Explain each endpoint briefly
- Show the schemas

---

## 7. Testing CRUD Operations (2 minutes)

**Scene:** Swagger UI / Postman

**Script:**
"Let's test our CRUD operations. I'll create a product, retrieve it, update the price, and then query with pagination."

**API Calls:**

1. **Create Product**
```json
{
  "name": "Gaming Laptop",
  "description": "High-performance laptop for gaming and development",
  "price": 1299.99,
  "stockQuantity": 50,
  "sku": "LAPTOP-GAME-001",
  "isFeatured": true
}
```

2. **Get All Products with Pagination**
```
GET /api/products?page=1&pageSize=10&sortBy=price&sortDesc=true
```

3. **Update Product**
```json
{
  "name": "Gaming Laptop Pro",
  "price": 1499.99
}
```

4. **Search Products**
```
GET /api/products?search=gaming
```

---

## 8. Advanced Features (1.5 minutes)

**Scene:** VS Code + Swagger

**Script:**
"The starter kit includes advanced features out of the box. Let me show you file uploads and authentication."

**Actions:**

1. **Show Authentication Controller**
   - Register endpoint
   - Login endpoint
   - JWT token generation

2. **Add File Upload to Product**
```csharp
[HttpPost("{id}/images")]
public async Task<IActionResult> UploadImages(Guid id, List<IFormFile> files)
{
    // File upload implementation
}
```

3. **Show Health Check Endpoint**
```
GET /health
```

---

## 9. Adding Another Entity (30 seconds)

**Scene:** Terminal

**Script:**
"Need to add categories? Just run the setup script again!"

**Commands:**
```bash
./scripts/setup.sh -e Category -p Categories -n MyStore.Api
```

**Show:**
- New files created
- How entities work together

---

## 10. Closing (30 seconds)

**Scene:** VS Code with running API

**Script:**
"And that's it! In just 5 minutes, we've created a production-ready API with:
- Complete CRUD operations
- JWT authentication
- File uploads
- Pagination and sorting
- Search functionality
- Clean architecture
- Full test coverage

The starter kit is open source and available on GitHub. Star the repo if you find it helpful, and feel free to contribute! 

Check the description for links to the repository, documentation, and more examples. Happy coding!"

**End Screen:**
- GitHub repository link
- Documentation link
- Discord/community link

---

## Visual Elements to Include

1. **Intro Animation:** Logo/title card
2. **Code Highlights:** Syntax highlighting for important parts
3. **Terminal Recording:** Clear, readable font
4. **Annotations:** Arrows pointing to important elements
5. **Transitions:** Smooth cuts between scenes
6. **Background Music:** Subtle, upbeat

## Recording Tips

1. **Resolution:** 1920x1080 minimum
2. **Font Size:** Increase for readability
3. **Theme:** Use high contrast theme
4. **Speed:** Normal pace, can speed up repetitive parts
5. **Audio:** Clear narration, remove background noise

## Alternative Shorter Version (3 minutes)

For social media:
1. Skip introduction (10s)
2. Show setup command (30s)
3. Show generated API in Swagger (30s)
4. Create and retrieve one item (60s)
5. Show pagination and search (30s)
6. Highlight key features (20s)
7. Call to action (10s)