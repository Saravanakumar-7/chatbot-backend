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
    public interface IRfqLPCostingRepository : IRepositoryBase<RfqLPCosting>
    {
        Task<PagedList<RfqLPCosting>> GetAllRfqLPCosting(PagingParameter pagingParameter);
        Task<RfqLPCosting> GetRfqLPCostingById(int id);
        Task<int?> CreateRfqLPCosting(RfqLPCosting rfqLPCosting);
        Task<string> UpdateRfqLPCosting(RfqLPCosting rfqLPCosting);
        Task<string> DeleteRfqLPCosting(RfqLPCosting rfqLPCosting);
        Task<RfqLPCosting> GetRfqLPCostingByRfqNumber(string RfqNumber);
    }
}
