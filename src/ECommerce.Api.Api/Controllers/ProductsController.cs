using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ECommerce.Api.Core.Models;
using ECommerce.Api.Core.DTOs;
using ECommerce.Api.Core.Interfaces;
using DotnetCrud.Api.Controllers.Base;
using DotnetCrud.Core.DTOs;
using AutoMapper;
using System.Linq.Expressions;

namespace ECommerce.Api.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseApiController<Product, CreateProductDto, UpdateProductDto, ProductResponseDto>
    {
        public ProductsController(
            IRepository<Product> repository, 
            IMapper mapper, 
            ILogger<ProductsController> logger) 
            : base(repository, mapper, logger)
        {
        }
        
        // The base controller provides:
        // GET    /api/products          - Get all (paginated)
        // GET    /api/products/{id}     - Get by ID
        // POST   /api/products          - Create new
        // PUT    /api/products/{id}     - Update
        // DELETE /api/products/{id}     - Delete
        
        // Custom endpoint: Search products
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedResponse<ProductResponseDto>>> SearchProducts(
            [FromQuery] string? term,
            [FromQuery] string? category,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var filter = BuildSearchFilter(term, category, minPrice, maxPrice);
            var (products, totalCount) = await _repository.GetPaginatedAsync(
                page, 
                pageSize, 
                filter,
                q => q.OrderBy(p => p.Name));
                
            var dtos = _mapper.Map<List<ProductResponseDto>>(products);
            
            return Ok(new PaginatedResponse<ProductResponseDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }
        
        // Custom endpoint: Get featured products
        [HttpGet("featured")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ProductResponseDto>>> GetFeaturedProducts()
        {
            var products = await _repository.FindAsync(p => p.IsFeatured && p.StockQuantity > 0);
            var dtos = _mapper.Map<List<ProductResponseDto>>(products.Take(10));
            return Ok(dtos);
        }
        
        // Custom endpoint: Check stock availability
        [HttpGet("{id}/check-stock")]
        public async Task<ActionResult<object>> CheckStock(Guid id, [FromQuery] int quantity)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return NotFound();
                
            var available = product.StockQuantity >= quantity;
            return Ok(new 
            { 
                available, 
                currentStock = product.StockQuantity,
                requestedQuantity = quantity
            });
        }
        
        // Override search filter for product-specific search
        protected override Expression<Func<Product, bool>>? GetSearchFilter(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return null;
                
            var searchLower = search.ToLower();
            return p => p.Name.ToLower().Contains(searchLower) || 
                       p.Description.ToLower().Contains(searchLower) ||
                       p.SKU.ToLower().Contains(searchLower) ||
                       p.Category.ToLower().Contains(searchLower);
        }
        
        // Override sorting for product-specific fields
        protected override Func<IQueryable<Product>, IOrderedQueryable<Product>>? GetOrderByExpression(
            string sortBy, bool descending)
        {
            return sortBy?.ToLower() switch
            {
                "name" => descending ? q => q.OrderByDescending(p => p.Name) : q => q.OrderBy(p => p.Name),
                "price" => descending ? q => q.OrderByDescending(p => p.Price) : q => q.OrderBy(p => p.Price),
                "stock" => descending ? q => q.OrderByDescending(p => p.StockQuantity) : q => q.OrderBy(p => p.StockQuantity),
                "category" => descending ? q => q.OrderByDescending(p => p.Category) : q => q.OrderBy(p => p.Category),
                _ => base.GetOrderByExpression(sortBy, descending)
            };
        }
        
        private Expression<Func<Product, bool>>? BuildSearchFilter(
            string? term, string? category, decimal? minPrice, decimal? maxPrice)
        {
            var expressions = new List<Expression<Func<Product, bool>>>();
            
            if (!string.IsNullOrWhiteSpace(term))
            {
                var termLower = term.ToLower();
                expressions.Add(p => p.Name.ToLower().Contains(termLower) || 
                                    p.Description.ToLower().Contains(termLower));
            }
            
            if (!string.IsNullOrWhiteSpace(category))
            {
                expressions.Add(p => p.Category.ToLower() == category.ToLower());
            }
            
            if (minPrice.HasValue)
            {
                expressions.Add(p => p.Price >= minPrice.Value);
            }
            
            if (maxPrice.HasValue)
            {
                expressions.Add(p => p.Price <= maxPrice.Value);
            }
            
            if (expressions.Count == 0)
                return null;
                
            // Combine all expressions with AND
            var combined = expressions[0];
            for (int i = 1; i < expressions.Count; i++)
            {
                combined = CombineExpressions(combined, expressions[i]);
            }
            
            return combined;
        }
        
        private Expression<Func<Product, bool>> CombineExpressions(
            Expression<Func<Product, bool>> expr1,
            Expression<Func<Product, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(Product));
            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);
            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);
            return Expression.Lambda<Func<Product, bool>>(Expression.AndAlso(left, right), parameter);
        }
        
        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;
            
            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }
            
            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }
    }
}
