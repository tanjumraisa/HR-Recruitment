using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class InterviewPanelRepository : Repository<InterviewPanel>, IInterviewPanelRepository
    {
        public InterviewPanelRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
