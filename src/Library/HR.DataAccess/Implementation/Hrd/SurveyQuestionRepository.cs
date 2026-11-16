using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class SurveyQuestionRepository : Repository<SurveyQuestion>, ISurveyQuestionRepository
    {
        public SurveyQuestionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
