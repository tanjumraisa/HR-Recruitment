using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class OutsourcedSalaryRepository : Repository<OutsourcedSalary>, IOutsourcedSalaryRepository
    {
        public OutsourcedSalaryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
