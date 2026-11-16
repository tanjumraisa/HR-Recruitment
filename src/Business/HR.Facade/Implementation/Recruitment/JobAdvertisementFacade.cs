using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class JobAdvertisementFacade : CrudFacade<JobAdvertisement>, IJobAdvertisementFacade
    {
        public JobAdvertisementFacade(IJobAdvertisementRepository repo) : base(repo)
        {
        }
        protected override string[] UniqueKeys => new[] { nameof(JobAdvertisement.JobOpeningId), nameof(JobAdvertisement.RecruitmentSourceId) };
    }
}
