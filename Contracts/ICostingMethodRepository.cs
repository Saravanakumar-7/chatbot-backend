using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICostingMethodRepository : IRepositoryBase<CostingMethod>
    {
        Task<PagedList<CostingMethod>> GetAllCostingMethods(PagingParameter pagingParameter, SearchParames searchParams);
        Task<CostingMethod> GetCostingMethodById(int id);
        Task<PagedList<CostingMethod>> GetAllActiveCostingMethods(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateCostingMethod(CostingMethod costingMethod);
        Task<string> UpdateCostingMethod(CostingMethod costingMethod);
        Task<string> DeleteCostingMethod(CostingMethod costingMethod);
    }
}
