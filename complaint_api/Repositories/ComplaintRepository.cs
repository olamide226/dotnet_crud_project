using complaint_api.Models;
using complaint_api.Data;
using DotnetCrud.Core.Interfaces;
using DotnetCrud.Infrastructure.Repositories;

namespace complaint_api.Repositories
{
    public interface IComplaintRepository : IRepository<Complaint>
    {
        // Add any custom repository methods here
    }

    public class ComplaintRepository : GenericRepository<Complaint>, IComplaintRepository
    {
        public ComplaintRepository(AppDbContext context) : base(context)
        {
        }
        
        // Implement any custom repository methods here
    }
}