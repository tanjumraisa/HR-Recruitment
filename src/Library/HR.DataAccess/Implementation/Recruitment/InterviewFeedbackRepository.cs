using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class InterviewFeedbackRepository : Repository<InterviewFeedback>, IInterviewFeedbackRepository
    {
        public InterviewFeedbackRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
