using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class SurveyQuestionFacade : CrudFacade<SurveyQuestion>, ISurveyQuestionFacade
    {
        public SurveyQuestionFacade(ISurveyQuestionRepository repo) : base(repo)
        {
        }

    }
}
