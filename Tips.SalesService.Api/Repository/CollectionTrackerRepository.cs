using Entities;
using Microsoft.EntityFrameworkCore;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

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
