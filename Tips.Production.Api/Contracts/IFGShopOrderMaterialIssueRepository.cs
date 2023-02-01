using Entities.Helper;
using Entities;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Contracts
{
    public interface IFGShopOrderMaterialIssueRepository : IRepositoryBase<FGShopOrderMaterialIssue>
    {
        Task<PagedList<FGShopOrderMaterialIssue>> GetAllFGShopOrderMaterialIssues(PagingParameter pagingParameter);
        Task<FGShopOrderMaterialIssue> GetFGShopOrderMaterialIssueById(int id);
        Task<long> CreateFGShopOrderMaterialIssue(FGShopOrderMaterialIssue fGShopOrderMaterialIssue);
        Task<string> UpdateFGShopOrderMaterialIssue(FGShopOrderMaterialIssue fGShopOrderMaterialIssue);
        Task<string> DeleteFGShopOrderMaterialIssue(FGShopOrderMaterialIssue fGShopOrderMaterialIssue);

    }
}