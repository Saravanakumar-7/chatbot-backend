using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IMaterialIssueTrackerRepository : IRepositoryBase<ShopOrderMaterialIssueTracker>
    {
        Task<int> AddDataToMaterialIssueTracker(ShopOrderMaterialIssueTracker materialIssueTracker);
    }
}
