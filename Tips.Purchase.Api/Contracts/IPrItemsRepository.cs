using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPrItemsRepository
    {
        Task<IEnumerable<PrItem>> GetAllPrItems();
        Task<PrItem> GetPrItemById(int id);
        Task<IEnumerable<PrItem>> GetAllActivePrItems();
        Task<int?> CreatePrItem(PrItem prItem);
        Task<string> UpdatePrItem(PrItem prItem);
        Task<string> DeletePrItem(PrItem prItem);
    }
}
