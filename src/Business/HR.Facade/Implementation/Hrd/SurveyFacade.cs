using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class SurveyFacade : CrudFacade<Survey>, ISurveyFacade
    {
        public SurveyFacade(ISurveyRepository repo) : base(repo)
        {
        }

    }
}
