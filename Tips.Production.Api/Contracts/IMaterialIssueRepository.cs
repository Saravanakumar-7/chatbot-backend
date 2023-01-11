using Entities.Helper;
using Entities;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Contracts
{
    public interface IMaterialIssueRepository : IRepositoryBase<MaterialIssue>
    {
        Task<PagedList<MaterialIssue>> GetAllMaterialIssue(PagingParameter pagingParameter);
        Task<MaterialIssue> GetMaterialIssueById(int id);
        Task<int> CreateMaterialIssue(MaterialIssue materialIssue);
        Task<string> UpdateMaterialIssue(MaterialIssue materialIssue);
        Task<string> DeleteMaterialIssue(MaterialIssue materialIssue);
    }
}
