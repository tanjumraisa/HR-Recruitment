using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class TrainingProgramRepository : Repository<TrainingProgram>, ITrainingProgramRepository
    {
        public TrainingProgramRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
