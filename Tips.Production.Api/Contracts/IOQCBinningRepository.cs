using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Contracts
{
    public interface IOQCBinningRepository
    {
        Task<int> CreateOQCBinning(OQCBinning oQCBinning);
        Task<OQCBinning> GetOQCBinningById(int id);
        Task<PagedList<OQCBinning>> GetAllOQCBinning(PagingParameter pagingParameter, SearchParamess searchParamess);
        Task<List<OQCStock>?> GetOqcBinningShopOrderQty(string Itemnumber);
        Task<IEnumerable<OQCBinningSPReport>> GetOQCBinningSPReportWithParam(string? ItemNumber, string? ShopOrderNumber);
        Task<IEnumerable<OQCBinningSPReport>> GetOQCBinningSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        void SaveAsync();
    }
}
