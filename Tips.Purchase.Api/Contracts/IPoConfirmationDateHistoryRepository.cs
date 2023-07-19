using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPoConfirmationDateHistoryRepository : IRepositoryBase<PoConfirmationDateHistory>
    {
        Task<long> CreatePoConfirmationDateHistory(PoConfirmationDateHistory poConfirmationDateHistory);
    }
}
