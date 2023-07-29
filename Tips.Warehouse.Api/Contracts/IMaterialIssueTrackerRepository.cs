using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IMaterialIssueTrackerRepository : IRepositoryBase<MaterialIssueTracker>
    {
        Task<int> AddDataToMaterialIssueTracker(MaterialIssueTracker materialIssueTracker);
    }
}
