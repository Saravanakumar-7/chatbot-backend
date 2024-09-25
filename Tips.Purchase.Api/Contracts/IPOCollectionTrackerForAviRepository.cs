using Entities.Helper;
using Entities;
using Tips.Purchase.Api.Entities.DTOs;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.Dto;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPOCollectionTrackerForAviRepository : IRepositoryBase<POCollectionTrackerForAvi>
    {
        Task<PagedList<POCollectionTrackerForAvi>> GetAllPOCollectionTrackersForAvi(PagingParameter pagingParameter, SearchParamess searchParamess);
        Task<POCollectionTrackerForAvi> GetPOCollectionTrackerForAviById(int id);
        Task<POCollectionTrackerForAviDetailsDto> GetPOCollectionTrackerForAviByVendorId(string vendorId);
        Task<int?> CreatePOCollectionTrackerForAvi(POCollectionTrackerForAvi pocollectionTracker);
        Task<string> UpdatePOCollectionTrackerForAvi(POCollectionTrackerForAvi pocollectionTracker);
        Task<string> DeletePOCollectionTrackerForAvi(POCollectionTrackerForAvi pocollectionTracker);
        Task<List<OpenPurchaseOrderForAviDetailsDto>> GetOpenPOForAviDetailsByVendorId(string vendorId);
        Task<IEnumerable<POCollectionTrackerForAvi>> SearchPOCollectionTrackerForAvi(SearchParamess searchParamess);
        Task<IEnumerable<POCollectionTrackerForAvi>> SearchPOCollectionTrackerForAviDate(SearchDatesParams searchDatesParams);
        Task<IEnumerable<POCollectionTrackerForAvi>> GetAllPOCollectionTrackerForAviWithItems(POCollectionTrackerForAviSearchDto poCollectionTrackerSearch);
        Task<IEnumerable<PayableSPReport>> GetPayableSPReportWithParam(string PONumber, string VendorName, string ProjectNumber);
        Task<IEnumerable<PayableSPReport>> GetPayableSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<List<POCollectionTrackerForAvi>> GetAllPOCollectionTrackersForAviByPonumber(string Ponumber);
    }
}
