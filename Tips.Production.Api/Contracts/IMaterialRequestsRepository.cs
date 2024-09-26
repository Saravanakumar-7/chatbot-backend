using Entities.Helper;
using Entities;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Tips.Production.Api.Contracts
{
    public interface IMaterialRequestsRepository : IRepositoryBase<MaterialRequests>
    {
        Task<PagedList<MaterialRequests>> GetAllMaterialRequest(PagingParameter pagingParameter, SearchParamess searchParammes);
        Task<PagedList<MaterialRequests>> GetAllMRStatusOpen(PagingParameter pagingParameter, SearchParamess searchParammes);
        Task<PagedList<MaterialRequests>> GetAllMROpenwithPartialStatus(PagingParameter pagingParameter, SearchParamess searchParammes);
        Task<IEnumerable<MaterialRequests>> GetAllMRStatusClose();
        Task<MaterialRequests> GetMaterialRequestById(int id);
        Task<MaterialRequests> GetMaterialReqByMRNumber(string MRnumber);
        Task<string> GenerateMRNumber();
        Task<int?> GetMRNumberAutoIncrementCount(DateTime date);
        Task<int?> CreateMaterialRequest(MaterialRequests request);
        Task<string> UpdateMaterialRequest(MaterialRequests request);
        Task<string> DeleteMaterialRequest(MaterialRequests request);
        Task<IEnumerable<MaterialRequestIdNoDto>> GetAllOpenMRIdNoList();
        Task<IEnumerable<MaterialRequests>> GetAllMaterialRequestsWithItems(MaterialRequestSearchDto materialRequestSearch);
        Task<IEnumerable<MaterialRequests>> SearchMaterialRequests([FromQuery] SearchParamess searchParammes);
        Task<IEnumerable<MaterialRequests>> SearchMaterialRequestsDate([FromQuery] SearchDateparames searchDatesParams);
        Task<string> GenerateMRNumberForAvision();
        Task<MaterialRequests> GetMaterialReqByShopOrderNumber(string ShopOrderNo);
        Task<IEnumerable<MaterialRequestSPReport>> MaterialRequestSPReport();
        Task<PagedList<MaterialRequestSpReportForTrans>> GetMaterialRequestSPReportForTrans(PagingParameter pagingParameter);
        Task<IEnumerable<MaterialRequestSPReport>> GetMaterialRequestSPReportWithParam(string? mRNumber, string? projectNo, string? KPN,
                                                                                                   string? shoporderNo);
        Task<IEnumerable<MaterialRequestSpReportForTrans>> GetMaterialRequestSPReportWithParamForTrans(string? mRNumber, string? projectNo, string? Itemnumber,
                                                                                                  string? shoporderNo);
        Task<IEnumerable<MaterialRequestSPReport>> GetMaterialRequestSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<MaterialRequestSpReportForTrans>> GetMaterialRequestSPReportWithDateForTrans(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<MaterialIssueAgainstMRSPReport>> GetMaterialIssueAgainstMaterialRequestSPReportWithParam(string? mRNumber, string? projectType, 
                                                                                                string? projectNo, string? KPN,string? shoporderNo);
        Task<IEnumerable<MaterialIssueAgainstMRSPReport>> GetMaterialIssueAgainstMaterialRequestSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        public void SaveAsync();
    }
}
