using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISoConfirmationDateHistoryRepository : IRepositoryBase<SoConfirmationDateHistory>
    {
        Task<long> CreateSoConfirmationHistory(SoConfirmationDateHistory soConfirmationDateHistory);
    }
}
