using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForeCastCustomerSupportItemRepository
    {
        Task<IEnumerable<ForeCastCustomerSupportItem>> GetAllForeCastCustomerSupportItem();
        Task<ForeCastCustomerSupportItem> GetForeCastCustomerSupportItemById(int id);
        Task<int?> CreateForeCastCustomerSupportItem(ForeCastCustomerSupportItem foreCastCustomerSupportItem);
        Task<string> UpdateForeCastCustomerSupportItem(ForeCastCustomerSupportItem foreCastCustomerSupportItem);
        Task<string> DeleteForeCastCustomerSupportItem(ForeCastCustomerSupportItem foreCastCustomerSupportItem);

        Task<string> ActivateForeCastCustomerSupportItemById(ForeCastCustomerSupportItem foreCastCustomerSupportItem);
    }
}
