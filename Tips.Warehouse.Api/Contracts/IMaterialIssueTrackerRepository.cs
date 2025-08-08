using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IMaterialIssueTrackerRepository : IRepositoryBase<ShopOrderMaterialIssueTracker>
    {
        Task<int> AddDataToMaterialIssueTracker(ShopOrderMaterialIssueTracker materialIssueTracker);
        Task<string> UpdateMaterialIssueTracker(ShopOrderMaterialIssueTracker shopOrderMaterialIssue);
        Task<List<ShopOrderMaterialIssueTracker>> GetDetailsByShopOrderNOItemNoLotNo(string PartNumber, string ShopOrderNumber, string LotNumber,string? MRNumber);

        Task<List<ShopOrderMaterialIssueTrackerDto>> SOMaterialIssueTrackerDetailsByShopOrderNo(string ShopOrderNo);
        Task<long?> CreateMaterialIssueTracker(ShopOrderMaterialIssueTracker shopOrderMaterialIssue);

        Task<List<MRNIssueTrackerDto>> GetWipQtyFromMaterialIssueTracker(string shopOrderNo, string partNumber, decimal returnedQty);
        Task<List<SomitConsumpWithBOMVersionDto>> GetSomitConsumpDetailsByShopOrderNumbers(string shopOrderNumber, string fgItemNumber);

    }
}
