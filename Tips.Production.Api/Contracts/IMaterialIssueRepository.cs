using Entities.Helper;
using Entities;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

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
        Task<IEnumerable<MaterialIssue>> GetAllMaterialIssueWithItems(MaterialIssueSearchDto materialIssueSearch);
        Task<IEnumerable<MaterialIssue>> SearchMaterialIssue([FromQuery] SearchParamess searchParammes);
        Task<IEnumerable<MaterialIssue>> SearchMaterialIssueDate([FromQuery] SearchDateparames searchDatesParams);
        Task<IEnumerable<MaterialIssueSPReport>> MaterialIssueSPReport();
        Task<IEnumerable<PickList>> PickListProductionSPReport(string? ShopOrderNumber);
        Task<IEnumerable<MaterialIssueSPReport>> GetMaterialIssueSPReportWithParam(string? shopOrderNo, string? FGitemnumber, string? projectNo,
                                                                                                   string? salesOrderNo);
        Task<IEnumerable<MaterialIssueSPReport>> GetMaterialIssueSPReportWithDate(DateTime? FromDate, DateTime? ToDate);

    }
}
