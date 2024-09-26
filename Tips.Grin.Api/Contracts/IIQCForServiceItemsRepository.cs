using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Contracts
{
    public interface IIQCForServiceItemsRepository : IRepositoryBase<IQCForServiceItems>
    {
        Task<IQCForServiceItems> GetIqcForServiceItemsDetailsbyGrinForServiceItemsNo(string grinsForServiceItemsNumber);
        Task<int?> CreateIQCForServiceItems(IQCForServiceItems iQCForServiceItems);
        Task<PagedList<IQCForServiceItems>> GetAllIQCForServiceItemsDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams);

        Task<IEnumerable<IQCForServiceItemsSPReport>> GetIQCForServiceItemsSPReportWithParam(string? grinsForServiceItemsNumber, string? itemNo);
        Task<IEnumerable<IQCForServiceItemsSPReport>> GetIQCForServiceItemsSPReportWithParamForTrans(string? grinsForServiceItemsNumber, string? itemNo, string? projectNumber);
        Task<IEnumerable<IQCForServiceItemsSPReport>> GetIQCForServiceItemsSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IQCForServiceItems> GetIQCForServiceItemsDetailsbyGrinForServiceItemsNo(string grinsForServiceItemsNumber);
        Task<string> UpdateIQCForServiceItems(IQCForServiceItems iQCForServiceItems);
        Task<IQCForServiceItems> GetIQCForServiceItemsDetailsbyId(int id);
        Task<List<IQCForServiceItems>> GetIqcForServiceItemsDetailsByGrinForServiceItemsNoAndParts(Dictionary<string, List<string>> Grins);
    }
}
