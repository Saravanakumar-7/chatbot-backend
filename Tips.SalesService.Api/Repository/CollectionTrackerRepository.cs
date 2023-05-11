using Entities;
using Microsoft.EntityFrameworkCore;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Repository
{
    public class CollectionTrackerRepository : RepositoryBase<CollectionTracker>, ICollectionTrackerRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public CollectionTrackerRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<int?> CreateCollectionTracker(CollectionTracker collectionTracker)
        {
            collectionTracker.CreatedBy = "Admin";
            collectionTracker.CreatedOn = DateTime.Now;
            collectionTracker.Unit = "Bangalore";
            var result = await Create(collectionTracker);
            return result.Id;
        }

        public async Task<string> DeleteCollectionTracker(CollectionTracker collectionTracker)
        {
            Delete(collectionTracker);
            string result = $"CollectionTracker details of {collectionTracker.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<CollectionTracker>> GetAllCollectionTrackers()
        {
            var collectionTrackerDetails = await FindAll().ToListAsync();
            return collectionTrackerDetails;
        }

        public async Task<CollectionTracker> GetCollectionTrackerById(int id)
        {
            var collectionTrackerDetailsById = await FindByCondition(x => x.Id == id)
                .Include(c=>c.SOBreakDown)
                .FirstOrDefaultAsync();
            return collectionTrackerDetailsById;
        }

        public async Task<CollectionTrackerDetailsDto> GetSOCollectionTrackerByCustomerId(string customerId)
        {
            var salesOrderTotalValue = _tipsSalesServiceDbContext.SalesOrders.Where(x => x.CustomerId == customerId).Sum(s => s.Total);

            var collectionDetails = _tipsSalesServiceDbContext.CollectionTrackers.Where(x => x.CustomerId == customerId).Select(x=>x.AlreadyRecieved);

                var alreadyRecieved = Convert.ToInt32(salesOrderTotalValue) - Convert.ToInt32(collectionDetails);

                var collectiveTrackerDetails = await _tipsSalesServiceDbContext.CollectionTrackers
                                .Select(s => new CollectionTrackerDetailsDto()
                                {
                                    TotalSumOfSOAmount = salesOrderTotalValue,
                                    AlreadyRecieved = Convert.ToDecimal(alreadyRecieved)

                                }).Distinct().FirstOrDefaultAsync();
            
            return collectiveTrackerDetails;

        }

        public async Task<string> UpdateCollectionTracker(CollectionTracker collectionTracker)
        {
            collectionTracker.LastModifiedBy = "Admin";
            collectionTracker.LastModifiedOn = DateTime.Now;
            Update(collectionTracker);
            string result = $"CollectionTracker details of {collectionTracker.Id} is updated successfully!";
            return result;
        }
    }
}
