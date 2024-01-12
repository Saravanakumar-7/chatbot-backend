using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPrItemsRepository : IRepositoryBase<PrItem>
    {
        Task<IEnumerable<PrItem>> GetAllPrItems();
        Task<PrItem> GetPrItemById(int id);
        Task<IEnumerable<PrItem>> GetAllActivePrItems();
        Task<int?> CreatePrItem(PrItem prItem);
        Task<string> UpdatePrItem(PrItem prItem);
        Task<string> DeletePrItem(PrItem prItem);
        Task<PrItem> ClosePrItemSatusByPrItemId(int prItemId);
        Task<int?> GetPrItemOpenStatusCount(int prId);
        Task<int?> GetPrItemClosedStatusCount(string prNo);
        Task<PrItem> GetPrItemByPRNo(string prNo, string pritem);
    }
}
