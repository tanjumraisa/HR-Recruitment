using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class TrainingProgramFacade : CrudFacade<TrainingProgram>, ITrainingProgramFacade
    {
        public TrainingProgramFacade(ITrainingProgramRepository repo) : base(repo)
        {
        }

    }
}
