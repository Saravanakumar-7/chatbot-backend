using Entities;
using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPoItemsRepository
    {
        Task<IEnumerable<PoItem>> GetAllPoItems();
        Task<PoItem> GetPoItemsById(int id);
        Task<IEnumerable<PoItem>> GetAllActivePoItems();
        Task<int?> CreatePoItems(PoItem poItems);
        Task<string> UpdatePoItems(PoItem poItems);
        Task<string> DeletePoItems(PoItem poItems);
    }
}
