using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ECommerce.Api.Core.Models;
using ECommerce.Api.Core.DTOs;
using ECommerce.Api.Core.Interfaces;
using DotnetCrud.Api.Controllers.Base;
using AutoMapper;

namespace ECommerce.Api.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : BaseApiController<Category, CreateCategoryDto, UpdateCategoryDto, CategoryResponseDto>
    {
        public CategoriesController(
            IRepository<Category> repository, 
            IMapper mapper, 
            ILogger<CategoriesController> logger) 
            : base(repository, mapper, logger)
        {
        }
        
        // The base controller provides:
        // GET    /api/categories          - Get all (paginated)
        // GET    /api/categories/{id}     - Get by ID
        // POST   /api/categories          - Create new
        // PUT    /api/categories/{id}     - Update
        // DELETE /api/categories/{id}     - Delete
        
        // TODO: Add any custom endpoints specific to Category here
    }
}
