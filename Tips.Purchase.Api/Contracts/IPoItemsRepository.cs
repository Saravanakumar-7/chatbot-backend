
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

    }
}
