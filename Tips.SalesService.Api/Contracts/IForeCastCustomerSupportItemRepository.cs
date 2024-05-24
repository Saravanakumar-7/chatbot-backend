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

        Task<IEnumerable<ForeCastCustomerSupportItem>> GetForecastCustomerSupportRelesedDetailsByForecastNumber(string forecastNo);

        Task<IEnumerable<ForeCastCustomerSupportItem>> GetForecastCustomerSupportItemByForecastNumber(string forecastNumber, decimal revNumber);

        Task<IEnumerable<ForeCastCustomerSupportItem>> GetAllActiveForecastCsItemsByForecastNo(string forecastNo);
        Task<List<int>> ForcastCsReleasedItemList(string forcastNumber);
        Task<bool> IsFullyReleasedForeCastCs(string rfqNumber, decimal revNumber);
        Task<bool> IsNotYetReleasedForeCastCs(string rfqNumber, decimal revNumber);
    }
}
