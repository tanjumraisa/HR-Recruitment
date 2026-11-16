using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class RecruitmentSourceRepository : Repository<RecruitmentSource>, IRecruitmentSourceRepository
    {
        public RecruitmentSourceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
