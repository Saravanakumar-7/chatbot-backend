using Entities.Helper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IRfqLPCostingOtherChargesRepository
    {
        Task<PagedList<RfqLPCostingOtherCharges>> GetAllRfqLPCostingOtherCharges(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<RfqLPCostingOtherCharges> GetRfqLPCostingOtherChargesById(int id);
        Task<int?> CreateRfqLPCostingOtherCharges(RfqLPCostingOtherCharges rfqLPCostingOtherCharges);
        Task<string> UpdateRfqLPCostingOtherCharges(RfqLPCostingOtherCharges rfqLPCostingOtherCharges);
        Task<string> DeleteRfqLPCostingOtherCharges(RfqLPCostingOtherCharges rfqLPCostingOtherCharges);
    }
}
