using Entities;
using Entities.Helper;
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
 
    public interface IRfqCustomerSupportRepository : IRepositoryBase<RfqCustomerSupport>
    {
        Task<PagedList<RfqCustomerSupport>> GetAllRfqCustomerSupport(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<RfqCustomerSupport> GetRfqCustomerSupportById(int id);
        Task<RfqCustomerSupport> GetRfqCustomerSupportDetailsById(int id);        
        Task<int?> CreateRfqCustomerSupport(RfqCustomerSupport rfqCustomerSupport);
        Task<string> UpdateRfqCustomerSupport(RfqCustomerSupport rfqCustomerSupport);
        Task<string> DeleteRfqCustomerSupport(RfqCustomerSupport rfqCustomerSupport);
        Task<RfqCustomerSupport> GetRfqCustomerSupportByRfqNumber(string RfqNumber);
        Task<RfqCustomerSupport> UpdateRfqcsRevNo(RfqCustomerSupport rfqCustomerSupport);
        Task<RfqCustomerSupport> GetRfqCsByRfqNoAndRevNo(string rfqNumber, decimal revisionNumber);
        Task<RfqCustomerSupport> GetRfqCsLatestRevNoByRfqnumber(string rfqNumber);
        Task<string> UpdateRfqCSRev(string rfqnumber, int rfqrev);
        Task<RfqCustomerSupport> GetRfqCustomerSupportDetailsbyrfqnumber(string rfqno);

    }

}
