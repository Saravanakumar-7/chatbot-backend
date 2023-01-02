using Entities.Helper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;
using Entities.DTOs;
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
        Task<Rfq> RfqSourcingByRfqNumbersss(string id);

    }
}
