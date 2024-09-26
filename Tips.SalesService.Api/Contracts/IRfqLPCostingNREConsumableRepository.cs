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
    public interface IRfqLPCostingNREConsumableRepository
    {
        Task<PagedList<RfqLPCostingNREConsumable>> GetAllRfqLPCostingNREConsumable(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<RfqLPCostingNREConsumable> GetRfqLPCostingNREConsumableById(int id);
        Task<int?> CreateRfqLPCostingNREConsumable(RfqLPCostingNREConsumable rfqLPCostingNREConsumable);
        Task<string> UpdateRfqLPCostingNREConsumable(RfqLPCostingNREConsumable rfqLPCostingNREConsumable);
        Task<string> DeleteRfqLPCostingNREConsumable(RfqLPCostingNREConsumable rfqLPCostingNREConsumable);
    }
}
