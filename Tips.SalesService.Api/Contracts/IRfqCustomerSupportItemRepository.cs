using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IRfqCustomerSupportItemRepository : IRepositoryBase<RfqCustomerSupportItems>
    {
        Task<IEnumerable<RfqCustomerSupportItems>> GetAllRfqCustomerSupportItem();
        Task<RfqCustomerSupportItems> GetRfqCustomerSupportItemById(int id);
        Task<IEnumerable<RfqCustomerSupportItems>> GetRfqCustomerSupportItemByRfqNumber(string rfqNumber);
        Task<IEnumerable<RfqCustomerSupportItems>> GetRfqCustomerSupportRelesedDetailsByRfqNumber(string rfqNumber);
        



        Task<int?> CreateRfqCustomerSupportItem(RfqCustomerSupportItems rfqCustomerSupportItems);
        Task<string> UpdateRfqCustomerSupportItem(RfqCustomerSupportItems rfqCustomerSupportItems);
        Task<string> DeleteRfqCustomerSupportItem(RfqCustomerSupportItems rfqCustomerSupportItems);

        Task<string> ActivateRfqCustomerSupportItemById(RfqCustomerSupportItems rfqCustomerSupportItems);
        Task<string> DeactivateRfqCustomerSupportItemById(RfqCustomerSupportItems rfqCustomerSupportItems);
        Task<IEnumerable<RfqCustomerSupportItems>> GetAllActiveRfqCustomerSupportItemsByRfqNumber(string rfqNumber);




    }
}
