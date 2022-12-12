using Entities;
using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPoItemsRepository
    {
        Task<IEnumerable<PoItems>> GetAllPoItems();
        Task<PoItems> GetPoItemsById(int id);
        Task<IEnumerable<PoItems>> GetAllActivePoItems();
        Task<int?> CreatePoItems(PoItems poItems);
        Task<string> UpdatePoItems(PoItems poItems);
        Task<string> DeletePoItems(PoItems poItems);
    }
}
