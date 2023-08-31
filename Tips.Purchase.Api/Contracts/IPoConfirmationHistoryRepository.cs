using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPoConfirmationHistoryRepository : IRepositoryBase<PoConfirmationHistory>
    {
        Task<long> CreatePoConfirmationHistory(PoConfirmationHistory poConfirmationHistory);
    }
}
