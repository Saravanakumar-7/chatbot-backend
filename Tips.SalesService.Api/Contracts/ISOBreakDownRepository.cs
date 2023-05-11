using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISOBreakDownRepository : IRepositoryBase<SOBreakDown>
    {
        Task<IEnumerable<SOBreakDown>> GetAllSOBreakDown();
        Task<SOBreakDown> GetSOBreakDownById(int id);
        Task<int?> CreateSOBreakDown(SOBreakDown collectionTrackerItem);
        Task<string> UpdateSOBreakDown(SOBreakDown collectionTrackerItem);
        Task<string> DeleteSOBreakDown(SOBreakDown collectionTrackerItem);
    }
}
