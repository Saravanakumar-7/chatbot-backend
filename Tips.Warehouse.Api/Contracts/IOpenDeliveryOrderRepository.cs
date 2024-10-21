using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{   
    public interface IOpenDeliveryOrderRepository : IRepositoryBase<OpenDeliveryOrder>
    {
        Task<PagedList<OpenDeliveryOrder>> GetAllOpenDeliveryOrders(PagingParameter pagingParameter, SearchParams searchParams);
        Task<string> GenerateODONumber();
        Task<OpenDeliveryOrder> GetOpenDeliveryOrderById(int id);
        Task<int?> CreateOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder);
        Task<string> UpdateOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder);
        Task<int?> GetODONumberAutoIncrementCount(DateTime date);
        Task<ODODetailsDto> GetODODetailsByItemNo(string itemNumber);
        Task<List<WarehouseDetailsDto>> GetWarehouseODOByItemNo(string itemNumber);
        Task<List<LocationDetailsDto>> GetLocationODOByItemNo(string itemNumber, string warehouse);
        Task<string> DeleteOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder);

        Task<IEnumerable<OpenDeliveryOrderIdNameList>> GetAllOpenDeliveryOrderIdNameList();
        Task<string> GenerateODONumberAvision();
        Task<IEnumerable<OpenDeliveryOrder>> GetAllOpenDeliveryOrderWithItems(OpenDeliveryOrderSearchDto OpenDeliveryOrderSearch);
        Task<IEnumerable<OpenDeliveryOrder>> SearchOpenDeliveryOrder([FromQuery] SearchParames searchParames);
        Task<IEnumerable<OpenDeliveryOrder>> SearchOpenDeliveryOrderDate([FromQuery] SearchsDateParms searchsDateParms);
 
        Task<PagedList<OpenDeliveryOrderSPReport>> OpenDeliveryOrderSPReport(PagingParameter pagingParameter);
        Task<IEnumerable<OpenDeliveryOrderSPReport>> OpenDeliveryOrderSPReportWithParam(string OpenDONumber, string CustomerName, string CustomerAliasName, string LeadId, string IssuedTo, string KPN, string MPN, string Warehouse, string Location, string ODOType);
        Task<IEnumerable<OpenDeliveryOrderSPReportForTrans>> OpenDeliveryOrderSPReportWithParamForTrans(string OpenDONumber, string CustomerName,
                                                                                                                 string IssuedTo, string ItemNumber, string MPN, string Warehouse,
                                                                                                                 string Location, string ODOType, string ProjectNumber);
        Task<IEnumerable<OpenDeliveryOrderSPReport>> OpenDeliveryOrderSPReportDates(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<OpenDeliveryOrderSPReportForTrans>> OpenDeliveryOrderSPReportDateForTrans(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<ODOMonthlyConsumptionSPReport>> GetODOMonthlyConsumptionSPReportWithParam(string CustomerId);
        Task<IEnumerable<ODOMonthlyConsumptionSPReport>> GetODOMonthlyConsumptionSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<List<ODOQuantityDto>> GetListOfODOQtyByItemNo(string itemNumber);

    }

}
