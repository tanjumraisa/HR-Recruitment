using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class TeamMemberFacade : CrudFacade<TeamMember>, ITeamMemberFacade
    {
        public TeamMemberFacade(ITeamMemberRepository repo) : base(repo)
        {
        }
        protected override string[] UniqueKeys => new[] { nameof(TeamMember.TeamId), nameof(TeamMember.EmployeeId) };
    }
}
