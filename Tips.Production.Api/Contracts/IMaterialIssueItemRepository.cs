using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Contracts
{
    public interface IMaterialIssueItemRepository : IRepositoryBase<MaterialIssueItem>
    {
        Task<IEnumerable<MaterialIssueItem>> GetMaterialIssueItemById(int id);
        Task<string> UpdateMaterialIssueItem(MaterialIssueItem materialIssueItem);
    }
}
