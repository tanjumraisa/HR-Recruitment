using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class TeamFacade : CrudFacade<Team>, ITeamFacade
    {
        public TeamFacade(ITeamRepository repo) : base(repo)
        {
        }
        protected override string? UniqueProperty => nameof(Team.Name);
    }
}
