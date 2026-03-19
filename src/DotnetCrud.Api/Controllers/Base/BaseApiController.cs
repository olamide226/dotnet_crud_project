using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DotnetCrud.Core.Models;
using DotnetCrud.Core.Interfaces;
using DotnetCrud.Core.DTOs;
using AutoMapper;
using System.Linq.Expressions;

namespace DotnetCrud.Api.Controllers.Base
{
    /// <summary>
    /// Base API controller providing standard CRUD operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public abstract class BaseApiController<TEntity, TCreateDto, TUpdateDto, TResponseDto> : ControllerBase
        where TEntity : BaseEntity
    {
        protected readonly IRepository<TEntity> _repository;
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;

        protected BaseApiController(
            IRepository<TEntity> repository,
            IMapper mapper,
            ILogger logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get all entities with pagination
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="sortBy">Sort field name</param>
        /// <param name="sortDesc">Sort in descending order</param>
        /// <param name="search">Search term</param>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<TResponseDto>), 200)]
        public virtual async Task<ActionResult<PaginatedResponse<TResponseDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDesc = false,
            [FromQuery] string? search = null)
        {
            try
            {
                _logger.LogInformation($"Getting paginated {typeof(TEntity).Name} - Page: {page}, PageSize: {pageSize}");

                // Validate pagination parameters
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 20;
                if (pageSize > 100) pageSize = 100; // Max page size

                Expression<Func<TEntity, bool>>? filter = null;
                if (!string.IsNullOrWhiteSpace(search))
                {
                    filter = GetSearchFilter(search);
                }

                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null;
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    orderBy = GetOrderByExpression(sortBy, sortDesc);
                }

                var (items, totalCount) = await _repository.GetPaginatedAsync(
                    page,
                    pageSize,
                    filter,
                    orderBy);

                var dtos = _mapper.Map<List<TResponseDto>>(items);

                var response = new PaginatedResponse<TResponseDto>
                {
                    Items = dtos,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    HasPrevious = page > 1,
                    HasNext = page < (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting paginated {typeof(TEntity).Name}");
                return StatusCode(500, new { error = "An error occurred while retrieving data" });
            }
        }

        /// <summary>
        /// Get entity by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TResponseDto), 200)]
        [ProducesResponseType(404)]
        public virtual async Task<ActionResult<TResponseDto>> GetById(Guid id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return NotFound(new { error = $"{typeof(TEntity).Name} not found" });
                }

                var dto = _mapper.Map<TResponseDto>(entity);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting {typeof(TEntity).Name} by ID: {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving the item" });
            }
        }

        /// <summary>
        /// Create new entity
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TResponseDto), 201)]
        [ProducesResponseType(400)]
        public virtual async Task<ActionResult<TResponseDto>> Create([FromBody] TCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var entity = _mapper.Map<TEntity>(createDto);
                
                // Set audit fields if user is authenticated
                if (User.Identity?.IsAuthenticated ?? false)
                {
                    entity.CreatedBy = User.Identity.Name;
                }

                var created = await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                var responseDto = _mapper.Map<TResponseDto>(created);

                _logger.LogInformation($"Created new {typeof(TEntity).Name} with ID: {created.Id}");

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating {typeof(TEntity).Name}");
                return StatusCode(500, new { error = "An error occurred while creating the item" });
            }
        }

        /// <summary>
        /// Update existing entity
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public virtual async Task<IActionResult> Update(Guid id, [FromBody] TUpdateDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return NotFound(new { error = $"{typeof(TEntity).Name} not found" });
                }

                _mapper.Map(updateDto, entity);
                
                // Set audit fields
                entity.UpdatedAt = DateTime.UtcNow;
                if (User.Identity?.IsAuthenticated ?? false)
                {
                    entity.UpdatedBy = User.Identity.Name;
                }

                await _repository.UpdateAsync(entity);
                await _repository.SaveChangesAsync();

                _logger.LogInformation($"Updated {typeof(TEntity).Name} with ID: {id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating {typeof(TEntity).Name} with ID: {id}");
                return StatusCode(500, new { error = "An error occurred while updating the item" });
            }
        }

        /// <summary>
        /// Delete entity (soft delete by default)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var exists = await _repository.ExistsAsync(id);
                if (!exists)
                {
                    return NotFound(new { error = $"{typeof(TEntity).Name} not found" });
                }

                await _repository.DeleteAsync(id);
                await _repository.SaveChangesAsync();

                _logger.LogInformation($"Deleted {typeof(TEntity).Name} with ID: {id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting {typeof(TEntity).Name} with ID: {id}");
                return StatusCode(500, new { error = "An error occurred while deleting the item" });
            }
        }

        /// <summary>
        /// Check if entity exists
        /// </summary>
        [HttpHead("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public virtual async Task<IActionResult> Exists(Guid id)
        {
            var exists = await _repository.ExistsAsync(id);
            return exists ? Ok() : NotFound();
        }

        /// <summary>
        /// Get count of entities
        /// </summary>
        [HttpGet("count")]
        [ProducesResponseType(typeof(int), 200)]
        public virtual async Task<ActionResult<int>> GetCount([FromQuery] string? search = null)
        {
            Expression<Func<TEntity, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(search))
            {
                filter = GetSearchFilter(search);
            }

            var count = await _repository.CountAsync(filter);
            return Ok(count);
        }

        /// <summary>
        /// Override this method to implement search functionality
        /// </summary>
        protected virtual Expression<Func<TEntity, bool>>? GetSearchFilter(string search)
        {
            // Default implementation - override in derived controllers
            return null;
        }

        /// <summary>
        /// Override this method to implement custom sorting
        /// </summary>
        protected virtual Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? GetOrderByExpression(
            string sortBy, 
            bool descending)
        {
            // Default implementation sorts by CreatedAt
            if (descending)
            {
                return q => q.OrderByDescending(e => e.CreatedAt);
            }
            return q => q.OrderBy(e => e.CreatedAt);
        }
    }
}