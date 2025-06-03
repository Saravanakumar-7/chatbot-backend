using Entities;
using Entities.DTOs;
using Entities.Enums;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Contracts
{
    public interface IShopOrderRepository 
    {
        Task<List<PickListDTO?>> GetPickListReport(string? ShopOrderNumber);
        Task<PagedList<ShopOrder>> GetAllShopOrders(PagingParameter pagingParameter, SearchParamess searchParamess);
        Task<ShopOrder> GetShopOrderById(int id);
        Task<int?> CreateShopOrder(ShopOrder shopOrder);
        Task<string> UpdateShopOrder(ShopOrder shopOrder);
        Task<string> UpdateShopOrderForApproval(ShopOrder shopOrder);
        Task<ShopOrder> GetShopOrderDetailsByShopOrderNo(string shopOrderNo);
        Task<ShopOrder> GetShopOrderBySalesOrderNo(string salesOrderNo);
        Task<List<string>> GetShopOrderNoListBySalesOrderNo(string salesOrderNo, string itemNumber);
        Task<List<ShopOrderComsumpDetailsDto>> GetShopOrderComsumptionDetialsBySaleOrderNos(List<string> lotNoListString);
        Task<string> GetShopOrderComsumptionDetialsBySaItemNo(string saItemNo, string fgItemNumber);
        Task<ShopOrder> GetShopOrderByShopOrderNo(string shopOrderNo);
        Task<ShopOrder> GetShopOrderApprovalStatusByShopOrderNo(string shopOrderNo);
        Task<IEnumerable<ListOfShopOrderDto>> GetShopOrderByItemType(string itemType);
        Task<IEnumerable<ListOfShopOrderDto>> GetShopOrderByFGNo(string fGNumber);
        Task<IEnumerable<ListOfShopOrderDto>> GetAllPendingApprovalShopOrderNumberList();
        Task<IEnumerable<ListOfShopOrderDto>> GetShopOrderByFGNoAndSANo(string fGNumber, string sANumber);
        Task<IEnumerable<ShopOrder>> GetAllOpenShopOrders();
        Task<IEnumerable<ListOfShopOrderDto>> GetAllFGShopOrderNoList();
        Task<IEnumerable<ListOfShopOrderDto>> GetAllSAShopOrderNoList();
        Task<IEnumerable<ListOfShopOrderDto>> GetAllActiveShopOrderNoList();
        Task<IEnumerable<ListOfShopOrderDto>> GetAllShopOrderIdNameList();
        Task<IEnumerable<ShopOrder>> GetAllShopOrderWithItems(ShopOrderSearchDto shopOrderSearch);
        Task<IEnumerable<ShopOrder>> SearchShopOrder([FromQuery] SearchParamess searchParammes);
        Task<IEnumerable<ShopOrder>> SearchShopOrderDate([FromQuery] SearchDateparames searchDatesParams);
        Task<IEnumerable<ListOfShopOrderDto>> GetAllActiveShopOrderNoListByProjectNo(string projectNo, PartType partType);
        Task<IEnumerable<ListOfShopOrderDto>> GetAllShopOrderNoListByProjectNoForMRN(string projectNo, PartType partType);
        Task<string> GenerateSONumberForAvision();
        Task<string> GenerateSONumber();
        Task<string> GenerateSONumberForKeus();
        Task<IEnumerable<ShopOrderNumberSPReport>> ShopOrderNumberSPReport();
        Task<PagedList<ShopOrderSPReportForTrans>> GetShopOrderNumberSPReportForTrans(PagingParameter pagingParameter);
        Task<IEnumerable<ShopOrderNumberSPReport>> GetShopOrderSPReportWithParam(string? shopOrderNo, string? projectType,
                                                                                                  string? projectNo, string? salesOrderNo, string? KPN, string? MPN);
        Task<IEnumerable<ShopOrderSPReportForTrans>> GetShopOrderSPReportWithParamForTrans(string? WorkOrderNumber, string? projectType,
                                                                                                 string? projectNo, string? salesOrderNo, string? KPN, string? MPN, string? Status);
        Task<IEnumerable<ShopOrderNumberSPReportForAvi>> GetShopOrderSPReportWithParamForAvi(string? shopOrderNo, string? projectType,
                                                                                                 string? projectNo, string? salesOrderNo, string? KPN, string? MPN);
        Task<IEnumerable<ShopOrderNumberSPReport>> GetShopOrderSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<ShopOrderNumberSPReportForAvi>> GetShopOrderSPReportWithDateForAvi(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<ShopOrderSPReportForTrans>> GetShopOrderSPReportWithDateForTrans(DateTime? FromDate, DateTime? ToDate, string Status);
        Task<IEnumerable<ShopOrderWipQtyDto>> GetShopOrderWipQtyByProjectNo(List<string> itemNumberList,string projectNo);
        Task<ShopOrderWipQtyDto> GetSAShopOrderWipQtyByProjectNo(string itemNumber, string projectNo);
        public void SaveAsync();
    }
}
