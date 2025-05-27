using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface ICollectionTrackerRepository : IRepositoryBase<CollectionTracker>
    {
        Task<PagedList<CollectionTracker>> GetAllCollectionTrackers(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<CollectionTracker> GetCollectionTrackerById(int id);
        Task<CollectionTrackerDetailsDto> GetSOCollectionTrackerByCustomerId(string customerId);
        Task<List<OpenSalesOrderDetailsForKeusDto>> GetOpenSODetailsByCustomerIdForKeus(string salesOrderNumber);
        Task<int?> CreateCollectionTracker(CollectionTracker collectionTracker);
        Task<string> UpdateCollectionTracker(CollectionTracker collectionTracker,List<SOBreakDown> sOBreakDowns);
        Task<string> DeleteCollectionTracker(CollectionTracker collectionTracker);
        Task<List<OpenSalesOrderDetailsDto>> GetOpenSODetailsByCustomerId(string customerId);
        Task<IEnumerable<CollectionTracker>> SearchCollectionTracker([FromQuery] SearchParammes searchParammes);
        Task<IEnumerable<CollectionTracker>> SearchCollectionTrackerDate([FromQuery] SearchDateParam searchDateParam);
        Task<IEnumerable<CollectionTracker>> GetAllCollectionTrackerWithItems(CollectionTrackerSearchDto collectionTrackerSearch);
        Task<IEnumerable<CollectionTrackerSPReport>> GetCollectionTrackerSPReportWithParam(string CustomerId);
        Task<IEnumerable<CollectionTrackerSPReport>> GetCollectionTrackerSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<CollectionTrackerByCustomerIdSPReport>> GetCollectionTrackerByCustomerIdSPReportWithParam(string CustomerId);
        Task<IEnumerable<CollectionTrackerByCustomerIdSPReport>> GetCollectionTrackerByCustomerIdSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<CollectionTrackerBySalesOrderNoSPReport>> GetCollectionTrackerBySalesOrderNoSPReportWithParam(string CustomerId);
        Task<IEnumerable<CollectionTrackerBySalesOrderNoSPReport>> GetCollectionTrackerBySalesOrderNoSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<CollectionTrackerWithCustomerWiseSPReport>> GetCollectionTrackerWithCustomerWiseSPReportWithParam(string CustomerId);
        Task<IEnumerable<CollectionTrackerWithCustomerWiseSPReport>> GetCollectionTrackerWithCustomerWiseSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<CollectionTrackerWithSalesOrderNoWiseSPReport>> GetCollectionTrackerWithSalesOrderNoWiseSPReportWithParam(string salesOrderNumber);
        Task<IEnumerable<CollectionTrackerWithSalesOrderNoWiseSPReport>> GetCollectionTrackerWithSalesOrderNoWiseSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<AdvanceReceivedEntryLevelSPResport>> GetAdvanceReceivedEntryLevelSPReportWithParam(string CustomerId, string TypeOfSolution);
        Task<IEnumerable<AdvanceReceivedEntryLevelSPResport>> GetAdvanceReceivedEntryLevelSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<AdvanceReceivedEntryLevelSPResport>> GetFirstAdvanceReceivedEntryLevelSPReportWithParam(string CustomerId, string TypeOfSolution);
        Task<IEnumerable<AdvanceReceivedEntryLevelSPResport>> GetFirstAdvanceReceivedEntryLevelSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<AdvanceReceivedEntryLevelSPResport>> GetLatestAdvanceReceivedEntryLevelSPReportWithParam(string CustomerId, string TypeOfSolution);
        Task<IEnumerable<AdvanceReceivedEntryLevelSPResport>> GetLatestAdvanceReceivedEntryLevelSPReportWithDate(DateTime? FromDate, DateTime? ToDate);

    }
}

