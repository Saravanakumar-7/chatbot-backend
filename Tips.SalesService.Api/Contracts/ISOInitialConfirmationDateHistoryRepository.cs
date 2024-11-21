using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISOInitialConfirmationDateHistoryRepository : IRepositoryBase<SOInitialConfirmationDateHistory>
    {
        Task<long> CreateSOInitialConfirmationDate(SOInitialConfirmationDateHistory soInitialConfirmationDateHistory);
    }
}
