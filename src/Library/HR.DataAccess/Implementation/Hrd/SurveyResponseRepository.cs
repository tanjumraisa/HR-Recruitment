using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class SurveyResponseRepository : Repository<SurveyResponse>, ISurveyResponseRepository
    {
        public SurveyResponseRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
