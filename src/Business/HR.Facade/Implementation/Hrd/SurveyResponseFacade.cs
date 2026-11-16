using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class SurveyResponseFacade : CrudFacade<SurveyResponse>, ISurveyResponseFacade
    {
        public SurveyResponseFacade(ISurveyResponseRepository repo) : base(repo)
        {
        }

    }
}
