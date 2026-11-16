using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class TaxSlabFacade : CrudFacade<TaxSlab>, ITaxSlabFacade
    {
        public TaxSlabFacade(ITaxSlabRepository repo) : base(repo)
        {
        }

    }
}
