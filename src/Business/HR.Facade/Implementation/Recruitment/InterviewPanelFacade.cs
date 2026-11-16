using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class InterviewPanelFacade : CrudFacade<InterviewPanel>, IInterviewPanelFacade
    {
        public InterviewPanelFacade(IInterviewPanelRepository repo) : base(repo)
        {
        }
        protected override string[] UniqueKeys => new[] { nameof(InterviewPanel.InterviewId), nameof(InterviewPanel.EmployeeId) };
    }
}
