using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class TrainingEnrollmentRepository : Repository<TrainingEnrollment>, ITrainingEnrollmentRepository
    {
        public TrainingEnrollmentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
