using System.Linq.Expressions;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Repository
{
    public class SOBreakDownRepository : RepositoryBase<SOBreakDown>, ISOBreakDownRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public SOBreakDownRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<int?> CreateSOBreakDown(SOBreakDown collectionTrackerItem)
        {
            var result = await Create(collectionTrackerItem);
            return result.Id;
        }

        public Task<string> DeleteSOBreakDown(SOBreakDown collectionTrackerItem)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SOBreakDown>> GetAllSOBreakDown()
        {
            throw new NotImplementedException();
        }

        public Task<SOBreakDown> GetSOBreakDownById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UpdateSOBreakDown(SOBreakDown collectionTrackerItem)
        {

            Update(collectionTrackerItem);
            string result = $"SOBreakDown details of {collectionTrackerItem.Id} is updated successfully!";
            return result;
        }
    }
}
