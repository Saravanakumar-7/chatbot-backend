using Entities.Helper;
using Entities;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Contracts
{
    public interface ISAShopOrderMaterialIssueRepository : IRepositoryBase<SAShopOrderMaterialIssue>
    {
        Task<PagedList<SAShopOrderMaterialIssue>> GetAllSAShopOrderMaterialIssue(PagingParameter pagingParameter);
        Task<SAShopOrderMaterialIssue> GetSAShopOrderMaterialIssueById(int id);
        Task<long> CreateSAShopOrderMaterialIssue(SAShopOrderMaterialIssue sAShopOrderMaterialIssue);
        Task<string> UpdateSAShopOrderMaterialIssue(SAShopOrderMaterialIssue sAShopOrderMaterialIssue);
        Task<string> DeleteSAShopOrderMaterialIssue(SAShopOrderMaterialIssue sAShopOrderMaterialIssue);

    }
}
