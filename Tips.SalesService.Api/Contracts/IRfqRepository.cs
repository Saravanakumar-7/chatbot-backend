using Entities;
using Entities.Helper;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface IRfqRepository : IRepositoryBase<Rfq>
    {
        Task<PagedList<Rfq>> GetAllRfq(PagingParameter pagingParameter);
        Task<Rfq> GetRfqById(int id);
        Task<int?> CreateRfq(Rfq rfq);
        Task<string> UpdateRfq(Rfq rfq);
        Task<string> DeleteRfq(Rfq rfq);
        Task<IEnumerable<RfqNumberListDto>> GetAllActiveRfqNumberList();
        Task<IEnumerable<RfqNumberListDto>> GetAllActiveRfqNumberListByCustomerName(string CustomerName);
        Task<Rfq> RfqSourcingByRfqNumbers(string id);
        Task<Rfq> RfqDetailsByRfqNumbers(string rfqNumber);        
        Task<Rfq> RfqLpcostingByRfqNumbers(string id);
        Task<Rfq> RfqLpCostingReleaseByRfqNumbers(string id);

    }
}
