using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Contracts
{
    public interface IMaterialIssueHistoryRepository : IRepositoryBase<MaterialIssueHistory>
    {
        Task<int> CreateMaterialIssueHistory(MaterialIssueHistory materialIssueHistory);
    }
}
