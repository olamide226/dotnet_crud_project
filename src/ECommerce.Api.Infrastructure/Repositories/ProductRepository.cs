using ECommerce.Api.Core.Models;
using ECommerce.Api.Core.Interfaces;
using ECommerce.Api.Infrastructure.Data;
using DotnetCrud.Infrastructure.Repositories;

namespace ECommerce.Api.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }
        
        // TODO: Add any custom repository methods specific to Product here
    }
}
