using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPoItemHistoryRepository : IRepositoryBase<PoItemHistory>
    {
        Task<PoItemHistory> CreatePoItemHistory(PoItemHistory poItemHistory);
        Task<List<PoItemHistory>> GetPoItemHistoryDetailsByPoItemId(string poNumber, string itemNumber);

    }
}
