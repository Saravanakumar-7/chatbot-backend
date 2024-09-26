using Entities.Helper;
using Entities;
using Tips.Purchase.Api.Entities;
using Entities.DTOs;
using Contracts;
using Tips.Purchase.Api.Entities.DTOs;
using Tips.Purchase.Api.Entities.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPOCollectionTrackerRepository : IRepositoryBase<POCollectionTracker>
    {
        Task<PagedList<POCollectionTracker>> GetAllPOCollectionTrackers(PagingParameter pagingParameter, SearchParamess searchParamess);
        Task<POCollectionTracker> GetPOCollectionTrackerById(int id);
        Task<POCollectionTrackerDetailsDto> GetPOCollectionTrackerByVendorId(string vendorId);
        Task<int?> CreatePOCollectionTracker(POCollectionTracker pocollectionTracker);
        Task<string> UpdatePOCollectionTracker(POCollectionTracker pocollectionTracker);
        Task<string> DeletePOCollectionTracker(POCollectionTracker pocollectionTracker);
        Task<List<OpenPurchaseOrderDetailsDto>> GetOpenPODetailsByVendorId(string vendorId);
        Task<IEnumerable<POCollectionTracker>> SearchPOCollectionTracker(SearchParamess searchParamess);
        Task<IEnumerable<POCollectionTracker>> SearchPOCollectionTrackerDate(SearchDatesParams searchDatesParams);
        Task<IEnumerable<POCollectionTracker>> GetAllPOCollectionTrackerWithItems(POCollectionTrackerSearchDto poCollectionTrackerSearch);
        //Task<IEnumerable<PayableSPReport>> GetPayableSPReportWithParam(string PONumber, string VendorName, string ProjectNumber);
        //Task<IEnumerable<PayableSPReport>> GetPayableSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
    }
}
