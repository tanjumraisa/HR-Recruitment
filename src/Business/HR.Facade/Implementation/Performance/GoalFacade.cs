using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class GoalFacade : CrudFacade<Goal>, IGoalFacade
    {
        public GoalFacade(IGoalRepository repo) : base(repo)
        {
        }

    }
}
