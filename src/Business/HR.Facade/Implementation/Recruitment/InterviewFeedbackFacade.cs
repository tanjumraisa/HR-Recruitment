using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class InterviewFeedbackFacade : CrudFacade<InterviewFeedback>, IInterviewFeedbackFacade
    {
        public InterviewFeedbackFacade(IInterviewFeedbackRepository repo) : base(repo)
        {
        }
        protected override string[] UniqueKeys => new[] { nameof(InterviewFeedback.InterviewId), nameof(InterviewFeedback.EmployeeId) };
    }
}
