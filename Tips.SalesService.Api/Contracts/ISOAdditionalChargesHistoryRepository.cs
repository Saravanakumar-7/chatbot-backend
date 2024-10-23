using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISOAdditionalChargesHistoryRepository : IRepositoryBase<SOAdditionalChargesHistory>
    {
        Task<SOAdditionalChargesHistory> CreateSOAdditionalChargesHistory(SOAdditionalChargesHistory soAdditionalChargesHistory);
        Task<string> UpdateSOAdditionalChargesHistory(SOAdditionalChargesHistory soAdditionalChargesHistory);
    }
}
