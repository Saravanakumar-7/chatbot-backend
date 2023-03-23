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
    public interface IRfqLPCostingProcessRepository
    {
        Task<PagedList<RfqLPCostingProcess>> GetAllRfqLPCostingProcess(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<RfqLPCostingProcess> GetRfqLPCostingProcessById(int id);
        Task<int?> CreateRfqLPCostingProcess(RfqLPCostingProcess rfqLPCostingProcess);
        Task<string> UpdateRfqLPCostingProcess(RfqLPCostingProcess rfqLPCostingProcess);
        Task<string> DeleteRfqLPCostingProcess(RfqLPCostingProcess rfqLPCostingProcess);
    }
}
