
using Entities.Helper;
using Entities;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPoItemsRepository : IRepositoryBase<PoItem>
    {
        Task<IEnumerable<PoItem>> GetPODetailsByPONumberandItemNo(string PONumber, string ItemNumber);
        Task<string> UpdatePOOrderItem(PoItem poItem);
        Task<int?> GetPODetailsByPONumber(string PONumber);
        Task<List<OpenPurchaseOrderDto>> GetOpenPOTGDetailsByItem(string itemNumber);
        Task<List<OpenPurchaseOrderDto>> GetOpenPODetailsByItem(string itemNumber);
        
        //Task<IEnumerable<PoItem>> GetPODetailsByItemNo(string ItemNumber);
        Task<PoItem> ClosePoItemSatusByPoItemId(int poItemId);
        Task<int?> GetPoItemOpenStatusCount(int poId);
        Task<List<OpenPoQuantityDto>> GetListOfOpenPOQtyByItemNoList(List<string> itemNumberList);
        Task<PoItem> GetPoItemDetailsById(int poItemId);
        Task<int?> GetPoItemsPartiallyClosedStatusCount(string poNumber, int poItemId);
        Task<IEnumerable<PoItem>> GetPoItemDetailsByPONumberandItemNo(string ItemNumber, string PONumber, int poItemId);
        Task<OpenPurchaseOrderDto?> GetOpenPOTGDetailsByItemForCoverage(string itemNumber);
        Task<OpenPurchaseOrderByProjectNoDto?> GetOpenPOTGDetailsByItemAndProjecNoForCoverage(string itemNumber, string projectNo);
        Task<List<OpenPoQuantityDto>> GetListOfOpenPOQtyByItemNoListByProjectNo(string projectNo, List<string> itemNumberList);
    }
}
