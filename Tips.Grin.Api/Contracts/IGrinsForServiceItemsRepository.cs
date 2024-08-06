using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Contracts
{
    public interface IGrinsForServiceItemsRepository : IRepositoryBase<GrinsForServiceItems>
    {
        Task<PagedList<GrinsForServiceItems>> GrinsForServiceItemsForServiceItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams);
        Task<GrinsForServiceItems> GetGrinsForServiceItemsById(int id);
        Task<IEnumerable<GrinForServiceItemsSPReport>> GetGrinsForServiceItemsSPReportWithParam(string? GrinsForServiceItemsNumber, string? VendorName, string? PONumber,
                                                                                                   string? KPN, string? MPN, string? Warehouse, string? Location);
        Task<IEnumerable<GrinForServiceItemsSPReport>> GetGrinsForServiceItemsSPReportWithParamForTrans(string? GrinsForServiceItemsNumber, string? VendorName, string? PONumber,
                                                                                                  string? KPN, string? MPN, string? Warehouse, string? Location, string? ProjectNumber);
        Task<IEnumerable<GrinForServiceItemsSPReport>> GetGrinsForServiceItemsSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<string> GenerateGrinsForServiceItemsNumberForAvision();
        Task<string> GenerateGrinsForServiceItemsNumber();
        Task<int?> CreateGrinsForServiceItems(GrinsForServiceItems grinsForServiceItems);
        Task<GrinsForServiceItems> GetGrinForServiceItemsByGrinForServiceItemsNo(string grinsForServiceItemsNumber);
        Task<string> UpdateGrinsForServiceItems(GrinsForServiceItems grinsForServiceItems);
        Task<int?> GetGrinsForServiceItemsIqcForServiceItemsStatusCount(string grinNo);
        Task<IEnumerable<GrinForServiceItemsNoForIqcForServiceItems>> GetAllGrinForServiceItemsNumberForIqcForServiceItems();
    }
}
