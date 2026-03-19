#!/bin/bash

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
WHITE='\033[1;37m'
NC='\033[0m' # No Color

# Function to display usage
usage() {
    echo "Usage: $0 -e ENTITY_NAME -n NAMESPACE [-p PLURAL_NAME] [-s] [-m]"
    echo ""
    echo "Options:"
    echo "  -e ENTITY_NAME    The name of your main entity (singular, e.g., 'Product')"
    echo "  -n NAMESPACE      The root namespace for your project (e.g., 'MyCompany.Api')"
    echo "  -p PLURAL_NAME    The plural form of your entity name (default: ENTITY_NAME + 's')"
    echo "  -s                Skip running dotnet restore"
    echo "  -m                Skip creating initial migration"
    echo "  -h                Display this help message"
    echo ""
    echo "Example:"
    echo "  $0 -e Product -p Products -n MyStore.Api"
    exit 1
}

# Parse command line arguments
SKIP_RESTORE=false
SKIP_MIGRATION=false

while getopts "e:n:p:smh" opt; do
    case ${opt} in
        e )
            ENTITY_NAME=$OPTARG
            ;;
        n )
            NAMESPACE=$OPTARG
            ;;
        p )
            PLURAL_NAME=$OPTARG
            ;;
        s )
            SKIP_RESTORE=true
            ;;
        m )
            SKIP_MIGRATION=true
            ;;
        h )
            usage
            ;;
        \? )
            echo "Invalid option: -$OPTARG" >&2
            usage
            ;;
    esac
done

# Validate required parameters
if [ -z "$ENTITY_NAME" ] || [ -z "$NAMESPACE" ]; then
    echo -e "${RED}Error: Entity name and namespace are required${NC}"
    usage
fi

# Set default plural name if not provided
if [ -z "$PLURAL_NAME" ]; then
    PLURAL_NAME="${ENTITY_NAME}s"
fi

# Validate inputs
if [[ $ENTITY_NAME =~ [[:space:]] ]]; then
    echo -e "${RED}Error: Entity name cannot contain spaces${NC}"
    exit 1
fi

if [[ $NAMESPACE =~ [[:space:]] ]]; then
    echo -e "${RED}Error: Namespace cannot contain spaces${NC}"
    exit 1
fi

# Convert first letter to lowercase for variable names
ENTITY_NAME_LOWER="$(echo ${ENTITY_NAME:0:1} | tr '[:upper:]' '[:lower:]')${ENTITY_NAME:1}"
PLURAL_NAME_LOWER="$(echo ${PLURAL_NAME:0:1} | tr '[:upper:]' '[:lower:]')${PLURAL_NAME:1}"

echo ""
echo -e "${CYAN}🚀 .NET CRUD API Starter Kit Setup${NC}"
echo -e "${CYAN}===================================${NC}"
echo ""
echo -e "${GREEN}Entity Name: $ENTITY_NAME${NC}"
echo -e "${GREEN}Plural Name: $PLURAL_NAME${NC}"
echo -e "${GREEN}Namespace: $NAMESPACE${NC}"
echo ""

# Function to replace placeholders
replace_placeholders() {
    local content="$1"
    content="${content//\{\{EntityName\}\}/$ENTITY_NAME}"
    content="${content//\{\{PluralName\}\}/$PLURAL_NAME}"
    content="${content//\{\{Namespace\}\}/$NAMESPACE}"
    content="${content//\{\{entityName\}\}/$ENTITY_NAME_LOWER}"
    content="${content//\{\{pluralName\}\}/$PLURAL_NAME_LOWER}"
    echo "$content"
}

# Create directory structure
echo -e "${YELLOW}📁 Creating project structure...${NC}"

directories=(
    "src/$NAMESPACE.Api/Controllers"
    "src/$NAMESPACE.Api/Middleware"
    "src/$NAMESPACE.Api/Filters"
    "src/$NAMESPACE.Core/Models"
    "src/$NAMESPACE.Core/DTOs"
    "src/$NAMESPACE.Core/Interfaces"
    "src/$NAMESPACE.Core/Constants"
    "src/$NAMESPACE.Infrastructure/Data"
    "src/$NAMESPACE.Infrastructure/Repositories"
    "src/$NAMESPACE.Infrastructure/Services"
    "src/$NAMESPACE.Application/Services"
    "src/$NAMESPACE.Application/Validators"
    "src/$NAMESPACE.Application/Mappings"
    "tests/$NAMESPACE.UnitTests"
    "tests/$NAMESPACE.IntegrationTests"
    "docs"
    ".github/workflows"
    
    # Base directories for starter kit files
    "src/DotnetCrud.Core/Models"
    "src/DotnetCrud.Core/Interfaces"
    "src/DotnetCrud.Core/DTOs"
    "src/DotnetCrud.Api/Controllers/Base"
    "src/DotnetCrud.Infrastructure/Repositories"
)

for dir in "${directories[@]}"; do
    mkdir -p "$dir"
done

echo -e "${GREEN}✅ Directory structure created${NC}"

# Copy base starter kit files if they don't exist
if [ -d "$(dirname "$0")/../src/DotnetCrud.Core" ]; then
    echo -e "${YELLOW}📝 Copying starter kit base files...${NC}"
    
    # Copy base files from the starter kit
    cp -n "$(dirname "$0")/../src/DotnetCrud.Core/Models/BaseEntity.cs" "src/DotnetCrud.Core/Models/" 2>/dev/null || true
    cp -n "$(dirname "$0")/../src/DotnetCrud.Core/Interfaces/IRepository.cs" "src/DotnetCrud.Core/Interfaces/" 2>/dev/null || true
    cp -n "$(dirname "$0")/../src/DotnetCrud.Core/DTOs/PaginatedResponse.cs" "src/DotnetCrud.Core/DTOs/" 2>/dev/null || true
    cp -n "$(dirname "$0")/../src/DotnetCrud.Core/DTOs/BaseDtos.cs" "src/DotnetCrud.Core/DTOs/" 2>/dev/null || true
    cp -n "$(dirname "$0")/../src/DotnetCrud.Api/Controllers/Base/BaseApiController.cs" "src/DotnetCrud.Api/Controllers/Base/" 2>/dev/null || true
    cp -n "$(dirname "$0")/../src/DotnetCrud.Infrastructure/Repositories/GenericRepository.cs" "src/DotnetCrud.Infrastructure/Repositories/" 2>/dev/null || true
    cp -n "$(dirname "$0")/../src/DotnetCrud.Api/Program.cs" "src/DotnetCrud.Api/" 2>/dev/null || true
    cp -n "$(dirname "$0")/../src/DotnetCrud.Infrastructure/Data/ApplicationDbContext.cs" "src/DotnetCrud.Infrastructure/Data/" 2>/dev/null || true
    
    echo -e "${GREEN}✅ Starter kit base files copied${NC}"
fi

# Generate Entity Model
echo -e "${YELLOW}📝 Generating entity model...${NC}"

entity_template='using System;
using System.ComponentModel.DataAnnotations;
using DotnetCrud.Core.Models;

namespace {{Namespace}}.Core.Models
{
    public class {{EntityName}} : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        // TODO: Add your entity-specific properties here
        // Example:
        // public decimal Price { get; set; }
        // public int Quantity { get; set; }
        // public string Category { get; set; }
    }
}'

entity_content=$(replace_placeholders "$entity_template")
echo "$entity_content" > "src/$NAMESPACE.Core/Models/$ENTITY_NAME.cs"

# Generate DTOs
dto_template='using System.ComponentModel.DataAnnotations;

namespace {{Namespace}}.Core.DTOs
{
    public class Create{{EntityName}}Dto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        // TODO: Add properties that should be provided when creating
    }

    public class Update{{EntityName}}Dto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        // TODO: Add properties that can be updated
    }

    public class {{EntityName}}ResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // TODO: Add properties that should be returned in responses
    }
}'

dto_content=$(replace_placeholders "$dto_template")
echo "$dto_content" > "src/$NAMESPACE.Core/DTOs/${ENTITY_NAME}Dto.cs"

# Generate Controller
controller_template='using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using {{Namespace}}.Core.Models;
using {{Namespace}}.Core.DTOs;
using {{Namespace}}.Core.Interfaces;
using DotnetCrud.Api.Controllers.Base;
using AutoMapper;

namespace {{Namespace}}.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class {{PluralName}}Controller : BaseApiController<{{EntityName}}, Create{{EntityName}}Dto, Update{{EntityName}}Dto, {{EntityName}}ResponseDto>
    {
        public {{PluralName}}Controller(
            IRepository<{{EntityName}}> repository, 
            IMapper mapper, 
            ILogger<{{PluralName}}Controller> logger) 
            : base(repository, mapper, logger)
        {
        }
        
        // The base controller provides:
        // GET    /api/{{pluralName}}          - Get all (paginated)
        // GET    /api/{{pluralName}}/{id}     - Get by ID
        // POST   /api/{{pluralName}}          - Create new
        // PUT    /api/{{pluralName}}/{id}     - Update
        // DELETE /api/{{pluralName}}/{id}     - Delete
        
        // TODO: Add any custom endpoints specific to {{EntityName}} here
    }
}'

controller_content=$(replace_placeholders "$controller_template")
echo "$controller_content" > "src/$NAMESPACE.Api/Controllers/${PLURAL_NAME}Controller.cs"

# Generate Repository
repository_template='using {{Namespace}}.Core.Models;
using {{Namespace}}.Core.Interfaces;
using {{Namespace}}.Infrastructure.Data;
using DotnetCrud.Infrastructure.Repositories;

namespace {{Namespace}}.Infrastructure.Repositories
{
    public class {{EntityName}}Repository : GenericRepository<{{EntityName}}>, I{{EntityName}}Repository
    {
        public {{EntityName}}Repository(ApplicationDbContext context) : base(context)
        {
        }
        
        // TODO: Add any custom repository methods specific to {{EntityName}} here
    }
}'

repository_content=$(replace_placeholders "$repository_template")
echo "$repository_content" > "src/$NAMESPACE.Infrastructure/Repositories/${ENTITY_NAME}Repository.cs"

# Generate Repository Interface
repository_interface_template='using {{Namespace}}.Core.Models;
using DotnetCrud.Core.Interfaces;

namespace {{Namespace}}.Core.Interfaces
{
    public interface I{{EntityName}}Repository : IRepository<{{EntityName}}>
    {
        // TODO: Add any custom repository method signatures here
    }
}'

repository_interface_content=$(replace_placeholders "$repository_interface_template")
echo "$repository_interface_content" > "src/$NAMESPACE.Core/Interfaces/I${ENTITY_NAME}Repository.cs"

# Update DbContext
echo -e "${YELLOW}📝 Creating DbContext...${NC}"

dbcontext_template='using Microsoft.EntityFrameworkCore;
using {{Namespace}}.Core.Models;

namespace {{Namespace}}.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<{{EntityName}}> {{PluralName}} { get; set; }
        // TODO: Add other DbSets here

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // {{EntityName}} configuration
            modelBuilder.Entity<{{EntityName}}>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                
                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
                
                // TODO: Add additional entity configuration here
            });

            // TODO: Add other entity configurations here
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}'

dbcontext_content=$(replace_placeholders "$dbcontext_template")
echo "$dbcontext_content" > "src/$NAMESPACE.Infrastructure/Data/ApplicationDbContext.cs"

# Create AutoMapper Profile
mapper_template='using AutoMapper;
using {{Namespace}}.Core.Models;
using {{Namespace}}.Core.DTOs;

namespace {{Namespace}}.Application.Mappings
{
    public class {{EntityName}}Profile : Profile
    {
        public {{EntityName}}Profile()
        {
            CreateMap<{{EntityName}}, {{EntityName}}ResponseDto>();
            CreateMap<Create{{EntityName}}Dto, {{EntityName}}>();
            CreateMap<Update{{EntityName}}Dto, {{EntityName}}>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
        }
    }
}'

mapper_content=$(replace_placeholders "$mapper_template")
echo "$mapper_content" > "src/$NAMESPACE.Application/Mappings/${ENTITY_NAME}Profile.cs"

# Create appsettings.json
echo -e "${YELLOW}📝 Creating configuration files...${NC}"

appsettings_content='{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database='$NAMESPACE';Username=postgres;Password=yourpassword"
  },
  "JWT": {
    "Secret": "your-very-long-secret-key-at-least-32-characters",
    "Issuer": "'$NAMESPACE'",
    "Audience": "'$NAMESPACE'",
    "ExpirationInMinutes": 60
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000", "http://localhost:4200"]
  }
}'

echo "$appsettings_content" > "src/$NAMESPACE.Api/appsettings.json"

# Create README
readme_template='# {{Namespace}}

A RESTful API built with .NET 8 using the CRUD Starter Kit.

## Quick Start

1. Update database connection string in `appsettings.json`
2. Run migrations: `dotnet ef database update`
3. Run the API: `dotnet run`
4. Access Swagger UI: https://localhost:5001/swagger

## API Endpoints

- `GET /api/{{pluralName}}` - Get all {{pluralName}} (paginated)
- `GET /api/{{pluralName}}/{id}` - Get {{entityName}} by ID
- `POST /api/{{pluralName}}` - Create new {{entityName}}
- `PUT /api/{{pluralName}}/{id}` - Update {{entityName}}
- `DELETE /api/{{pluralName}}/{id}` - Delete {{entityName}}

## Features

- JWT Authentication
- Pagination
- Soft Delete
- Audit Fields (CreatedAt, UpdatedAt)
- Swagger Documentation
- Global Error Handling
- Request/Response Logging

## Development

To add new properties to {{EntityName}}:

1. Update the model in `src/{{Namespace}}.Core/Models/{{EntityName}}.cs`
2. Update DTOs in `src/{{Namespace}}.Core/DTOs/{{EntityName}}Dto.cs`
3. Update AutoMapper profile if needed
4. Create a new migration: `dotnet ef migrations add AddNewProperties`
5. Update the database: `dotnet ef database update`
'

readme_content=$(replace_placeholders "$readme_template")
echo "$readme_content" > "README.md"

# Create .gitignore
echo -e "${YELLOW}📝 Creating .gitignore...${NC}"

cat > .gitignore << 'EOF'
## Ignore Visual Studio temporary files, build results, and
## files generated by popular Visual Studio add-ons.

# User-specific files
*.rsuser
*.suo
*.user
*.userosscache
*.sln.docstates

# User-specific files (MonoDevelop/Xamarin Studio)
*.userprefs

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio 2015/2017 cache/options directory
.vs/
# Uncomment if you have tasks that create the project's static files in wwwroot
#wwwroot/

# Visual Studio 2017 auto generated files
Generated\ Files/

# MSTest test Results
[Tt]est[Rr]esult*/
[Bb]uild[Ll]og.*

# .NET Core
project.lock.json
project.fragment.lock.json
artifacts/

# Files built by Visual Studio
*_i.c
*_p.c
*_h.h
*.ilk
*.meta
*.obj
*.iobj
*.pch
*.pdb
*.ipdb
*.pgc
*.pgd
*.rsp
*.sbr
*.tlb
*.tli
*.tlh
*.tmp
*.tmp_proj
*_wpftmp.csproj
*.log
*.vspscc
*.vssscc
.builds
*.pidb
*.svclog
*.scc

# Visual Studio code coverage results
*.coverage
*.coveragexml

# NCrunch
_NCrunch_*
.*crunch*.local.xml
nCrunchTemp_*

# NuGet Packages
*.nupkg
# The packages folder can be ignored because of Package Restore
**/[Pp]ackages/*
# except build/, which is used as an MSBuild target.
!**/[Pp]ackages/build/
# Uncomment if necessary however generally it will be regenerated when needed
#!**/[Pp]ackages/repositories.config
# NuGet v3's project.json files produces more ignorable files
*.nuget.props
*.nuget.targets

# Microsoft Azure Build Output
csx/
*.build.csdef

# Microsoft Azure Emulator
ecf/
rcf/

# Windows Store app package directories and files
AppPackages/
BundleArtifacts/
Package.StoreAssociation.xml
_pkginfo.txt
*.appx
*.appxbundle
*.appxupload

# Visual Studio cache files
# files ending in .cache can be ignored
*.[Cc]ache
# but keep track of directories ending in .cache
!?*.[Cc]ache/

# Others
ClientBin/
~$*
*~
*.dbmdl
*.dbproj.schemaview
*.jfm
*.pfx
*.publishsettings
orleans.codegen.cs

# RIA/Silverlight projects
Generated_Code/

# Backup & report files from converting an old project file
# to a newer Visual Studio version. Backup files are not needed,
# because we have git ;-)
_UpgradeReport_Files/
Backup*/
UpgradeLog*.XML
UpgradeLog*.htm
ServiceFabricBackup/
*.rptproj.bak

# SQL Server files
*.mdf
*.ldf
*.ndf

# Business Intelligence projects
*.rdl.data
*.bim.layout
*.bim_*.settings
*.rptproj.rsuser
*- Backup*.rdl

# Microsoft Fakes
FakesAssemblies/

# GhostDoc plugin setting file
*.GhostDoc.xml

# Node.js Tools for Visual Studio
.ntvs_analysis.dat
node_modules/

# Visual Studio 6 build log
*.plg

# Visual Studio 6 workspace options file
*.opt

# Visual Studio 6 auto-generated workspace file (contains which files were open etc.)
*.vbw

# Visual Studio LightSwitch build output
**/*.HTMLClient/GeneratedArtifacts
**/*.DesktopClient/GeneratedArtifacts
**/*.DesktopClient/ModelManifest.xml
**/*.Server/GeneratedArtifacts
**/*.Server/ModelManifest.xml
_Pvt_Extensions

# Paket dependency manager
.paket/paket.exe
paket-files/

# FAKE - F# Make
.fake/

# JetBrains Rider
.idea/
*.sln.iml

# CodeRush personal settings
.cr/personal

# Python Tools for Visual Studio (PTVS)
__pycache__/
*.pyc

# Cake - Uncomment if you are using it
# tools/**
# !tools/packages.config

# Tabs Studio
*.tss

# Telerik's JustMock configuration file
*.jmconfig

# BizTalk build output
*.btp.cs
*.btm.cs
*.odx.cs
*.xsd.cs

# OpenCover UI analysis results
OpenCover/

# Azure Stream Analytics local run output
ASALocalRun/

# MSBuild Binary and Structured Log
*.binlog

# NVidia Nsight GPU debugger configuration file
*.nvuser

# MFractors (Xamarin productivity tool) working folder
.mfractor/

# Local History for Visual Studio
.localhistory/

# BeatPulse healthcheck temp database
healthchecksdb

# Backup folder for Package Reference Convert tool in Visual Studio 2017
MigrationBackup/

# Ionide (cross platform F# VS Code tools) working folder
.ionide/

# Rider
*.DotSettings.user

# macOS
.DS_Store

# Environment files
.env
.env.local
.env.development.local
.env.test.local
.env.production.local

# Application specific
appsettings.Development.json
appsettings.Production.json
EOF

# Summary
echo ""
echo -e "${GREEN}✅ Setup completed successfully!${NC}"
echo ""
echo -e "${CYAN}📋 Next Steps:${NC}"
echo -e "${WHITE}1. Navigate to the project directory${NC}"
echo -e "${WHITE}2. Update the connection string in appsettings.json${NC}"

if [ "$SKIP_RESTORE" = false ]; then
    echo -e "${YELLOW}3. Running dotnet restore...${NC}"
    if command -v dotnet &> /dev/null; then
        dotnet restore
    else
        echo -e "${RED}   dotnet CLI not found. Please install .NET SDK and run 'dotnet restore'${NC}"
    fi
fi

if [ "$SKIP_MIGRATION" = false ]; then
    echo -e "${WHITE}4. Create initial migration with:${NC}"
    echo -e "   ${CYAN}dotnet ef migrations add Initial${NC}"
fi

echo -e "${WHITE}5. Run the API with: dotnet run${NC}"
echo ""
echo -e "${MAGENTA}🎉 Happy coding!${NC}"