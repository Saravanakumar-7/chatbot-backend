using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPrItemsRepository
    {
        Task<IEnumerable<PrItems>> GetAllPrItems();
        Task<PrItems> GetPrItemsById(int id);
        Task<IEnumerable<PrItems>> GetAllActivePrItems();
        Task<int?> CreatePrItems(PrItems prItems);
        Task<string> UpdatePrItems(PrItems prItems);
        Task<string> DeletePrItems(PrItems prItems);
    }
}
