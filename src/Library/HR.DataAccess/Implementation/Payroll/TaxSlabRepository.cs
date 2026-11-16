using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class TaxSlabRepository : Repository<TaxSlab>, ITaxSlabRepository
    {
        public TaxSlabRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
