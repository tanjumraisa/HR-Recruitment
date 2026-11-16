using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class OvertimeRecordRepository : Repository<OvertimeRecord>, IOvertimeRecordRepository
    {
        public OvertimeRecordRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
