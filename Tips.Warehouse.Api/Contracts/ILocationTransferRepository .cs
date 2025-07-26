using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface ILocationTransferRepository : IRepositoryBase<LocationTransfer>
    {
        Task<int?> CreateLocationTransfer(LocationTransfer locationTransfer);
        Task<string> UpdateLocationTransfer(LocationTransfer locationTransfer);
        Task<string> DeleteLocationTransfer(LocationTransfer locationTransfer);
        Task<PagedList<LocationTransfer>> GetAllLocationTransfer(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<LocationTransfer> GetLocationTransferById(int id);
        Task<List<LocationTransferFromDto>> GetProjectLocWareFromInventoryByItemNo(string itemNumber); 
        Task<IEnumerable<LocationTransfer>> SearchLocationTransfer([FromQuery] SearchParammes searchParammes);
        Task<IEnumerable<LocationTransfer>> SearchLocationTransferDate([FromQuery] SearchDateParam searchDatesParams);
        Task<IEnumerable<LocationTransfer>> GetAllLocationTransferWithItems(LocationTransferSearchDto locationTransferSearchDto);
        Task<IEnumerable<LocationTransferIdNameList>> GetAllLocationTransferIdNameList();
        Task<PagedList<LocationTransferSPReport>> LocationTransferSPReport(PagingParameter pagingParameter);
        Task<IEnumerable<LocationTransferSPReport>> LocationTransferSPReportWithParam(string FromPartNumber, string FromPartType, string FromWarehouse, string FromLocation, string FromProjectNumber, string ToPartnumber, string ToPartType, string ToWarehouse, string ToLocation, string ToProjectNumber);
        Task<IEnumerable<LocationTransferSpReportForTras>> LocationTransferSPReportWithParamForTras(string FromPartNumber, string FromPartType, string FromWarehouse, string FromLocation, string FromProjectNumber, string ToPartnumber, string ToPartType, string ToWarehouse, string ToLocation, string ToProjectNumber);
        Task<IEnumerable<LocationTransferSpReportForAvi>> LocationTransferSPReportWithParamForAvi(string FromPartNumber, string FromPartType, string FromWarehouse, string FromLocation, string FromProjectNumber, string ToPartnumber, string ToPartType, string ToWarehouse, string ToLocation, string ToProjectNumber);
        Task<IEnumerable<LocationTransferSPReport>> LocationTransferSPReportDates(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<LocationTransferSpReportForTras>> LocationTransferSPReportDatesForTras(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<LocationTransferSpReportForAvi>> LocationTransferSPReportDatesForAvi(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<MRNSPReport>> MRNSPReportWithParam(string? ProjectNumber, string? ShopOrderType, string? ShopOrderNumber, string? KPN, string? PartType);
        Task<IEnumerable<MRNSPReportForTrans>> MRNSPReportWithParamForTrans(string? ProjectNumber, string? ShopOrderType, string? ShopOrderNumber, string? PartNumber, string? PartType);
        Task<IEnumerable<MRNSPReportForTrans>> MRNSPReportDatesForTrans(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<MRNSPReport>> MRNSPReportDates(DateTime? FromDate, DateTime? ToDate);
        Task<int> GetLatestLocationTransferId();
    }
}