using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPrItemsRepository
    {
        Task<IEnumerable<PrItem>> GetAllPrItems();
        Task<PrItem> GetPrItemsById(int id);
        Task<IEnumerable<PrItem>> GetAllActivePrItems();
        Task<int?> CreatePrItems(PrItem prItems);
        Task<string> UpdatePrItems(PrItem prItems);
        Task<string> DeletePrItems(PrItem prItems);
    }
}
