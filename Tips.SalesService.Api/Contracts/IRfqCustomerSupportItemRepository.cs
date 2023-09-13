using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface IRfqCustomerSupportItemRepository : IRepositoryBase<RfqCustomerSupportItems>
    {
        Task<IEnumerable<RfqCustomerSupportItems>> GetAllRfqCustomerSupportItem();
        Task<IEnumerable<RfqCustomerSupportItems>> RfqCsReleasedItemList(string rfqNumber);

        Task<RfqCustomerSupportItems> GetRfqCustomerSupportItemById(int id);
        Task<IEnumerable<RfqCustomerSupportItems>> GetRfqCustomerSupportItemByRfqNumber(string rfqNumber);
        Task<IEnumerable<RfqCustomerSupportItems>> GetRfqCustomerSupportRelesedDetailsByRfqNumber(string rfqNumber);

        Task<IEnumerable<RfqCustomerSupportItems>> GetRfqCustomerSupportItemByRfqNumber(string rfqNumber, decimal revNumber);


        Task<int?> CreateRfqCustomerSupportItem(RfqCustomerSupportItems rfqCustomerSupportItems);
        Task<string> UpdateRfqCustomerSupportItem(RfqCustomerSupportItems rfqCustomerSupportItems);
        Task<string> DeleteRfqCustomerSupportItem(RfqCustomerSupportItems rfqCustomerSupportItems);

        Task<string> ActivateRfqCustomerSupportItemById(RfqCustomerSupportItems rfqCustomerSupportItems);
        Task<string> DeactivateRfqCustomerSupportItemById(RfqCustomerSupportItems rfqCustomerSupportItems);
        Task<IEnumerable<RfqCustomerSupportItems>> GetAllActiveRfqCustomerSupportItemsByRfqNumber(string rfqNumber);

        Task<bool> IsFullyReleasedRfqEngg(string rfqNumber, decimal revNumber);
        Task<bool> IsNotYetReleasedRfqEngg(string rfqNumber, decimal revNumber);

        Task<bool> IsFullyReleasedRfqCs(string rfqNumber, decimal revNumber);
        Task<bool> IsNotYetReleasedRfqCs(string rfqNumber, decimal revNumber);
        Task<IEnumerable<string>> GetRfqCsandForecastCsDetailListByItemNumber(string itemNumber);
        Task<IEnumerable<string>> GetRfqEnggandForecastCsProjectList();
        Task<IEnumerable<string>> GetRfqCsandForecastCsProjectNumberList();

        Task<IEnumerable<string>> GetRfqEnggandForecastCsDetailListByItemNumber(string itemNumber);

    }
}
