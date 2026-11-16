using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class EmployeeDocumentRepository : Repository<EmployeeDocument>, IEmployeeDocumentRepository
    {
        public EmployeeDocumentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
