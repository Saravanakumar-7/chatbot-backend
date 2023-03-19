using Entities.Helper;
using Entities;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Contracts
{
    public interface IMaterialIssueRepository : IRepositoryBase<MaterialIssue>
    {
        Task<PagedList<MaterialIssue>> GetAllMaterialIssues(PagingParameter pagingParameter, SearchParamess searchParamess);
        Task<MaterialIssue> GetMaterialIssueById(int id);
        Task<int> CreateMaterialIssue(MaterialIssue materialIssue);
        Task<string> UpdateMaterialIssue(MaterialIssue materialIssue);
        Task<string> DeleteMaterialIssue(MaterialIssue materialIssue);
        Task<MaterialIssue> GetMaterialIssueByShopOrderNo(string shopOrderNo);

        Task<IEnumerable<MaterialIssueIdNameList>> GetAllMaterialIssueIdNameList();

    }
}
