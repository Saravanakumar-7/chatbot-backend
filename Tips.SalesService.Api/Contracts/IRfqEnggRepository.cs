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
    public interface IRfqEnggRepository : IRepositoryBase<RfqEngg>
    {
        Task<PagedList<RfqEngg>> GetAllRfqEngg(PagingParameter pagingParameter);
        Task<RfqEngg> GetRfqEnggById(int id);
        Task<int?> CreateRfqEngg(RfqEngg rfqEngg);
        Task<string> UpdateRfqEngg(RfqEngg rfqEngg);
        Task<string> DeleteRfqEngg(RfqEngg rfqEngg);
        Task<RfqEngg> RfqEnggByRfqNumber(string RfqNumber);
    }
}



