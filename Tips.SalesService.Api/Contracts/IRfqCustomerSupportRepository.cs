using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities; 


namespace Tips.SalesService.Api.Contracts
{
 
    public interface IRfqCustomerSupportRepository : IRepositoryBase<RfqCustomerSupport>
    {
        Task<PagedList<RfqCustomerSupport>> GetAllRfqCustomerSupport(PagingParameter pagingParameter);
        Task<RfqCustomerSupport> GetRfqCustomerSupportById(int id);
        Task<int?> CreateRfqCustomerSupport(RfqCustomerSupport rfqCustomerSupport);
        Task<string> UpdateRfqCustomerSupport(RfqCustomerSupport rfqCustomerSupport);
        Task<string> DeleteRfqCustomerSupport(RfqCustomerSupport rfqCustomerSupport);
        Task<RfqCustomerSupport> RfqCustomerSupportByRfqNumber(string RfqNumber);
    }

}
