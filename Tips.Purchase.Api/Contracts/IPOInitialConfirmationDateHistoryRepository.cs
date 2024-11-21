using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPOInitialConfirmationDateHistoryRepository : IRepositoryBase<POInitialConfirmationDateHistory>
    {
        Task<long> CreatePOInitialConfirmationDate(POInitialConfirmationDateHistory poInitialConfirmationDateHistory);
    }
}
