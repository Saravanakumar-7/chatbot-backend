using Entities;
using Entities.Helper;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface ICollectionTrackerRepository : IRepositoryBase<CollectionTracker>
    {
        Task<IEnumerable<CollectionTracker>> GetAllCollectionTrackers();
        Task<CollectionTracker> GetCollectionTrackerById(int id);
        Task<CollectionTrackerDetailsDto> GetSOCollectionTrackerByCustomerId(string customerId);
        Task<int?> CreateCollectionTracker(CollectionTracker collectionTracker);
        Task<string> UpdateCollectionTracker(CollectionTracker collectionTracker);
        Task<string> DeleteCollectionTracker(CollectionTracker collectionTracker);
    }
}
