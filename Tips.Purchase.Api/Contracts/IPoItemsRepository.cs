using Entities;
using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPoItemsRepository
    {
        Task<IEnumerable<PoItem>> GetAllPoItems();
        Task<PoItem> GetPoItemById(int id);
        Task<IEnumerable<PoItem>> GetAllActivePoItems();
        Task<int?> CreatePoItem(PoItem poItem);
        Task<string> UpdatePoItem(PoItem poItem);
        Task<string> DeletePoItem(PoItem poItem);
    }
}
