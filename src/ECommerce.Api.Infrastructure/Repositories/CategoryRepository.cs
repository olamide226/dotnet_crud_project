using ECommerce.Api.Core.Models;
using ECommerce.Api.Core.Interfaces;
using ECommerce.Api.Infrastructure.Data;
using DotnetCrud.Infrastructure.Repositories;

namespace ECommerce.Api.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
        
        // TODO: Add any custom repository methods specific to Category here
    }
}
