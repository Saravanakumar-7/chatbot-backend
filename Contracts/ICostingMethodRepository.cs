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
        Task<IEnumerable<CostingMethod>> GetAllCostingMethods(SearchParames searchParams);
        Task<CostingMethod> GetCostingMethodById(int id);
        Task<IEnumerable<CostingMethod>> GetAllActiveCostingMethods(SearchParames searchParams);
        Task<int?> CreateCostingMethod(CostingMethod costingMethod);
        Task<string> UpdateCostingMethod(CostingMethod costingMethod);
        Task<string> DeleteCostingMethod(CostingMethod costingMethod);
    }
}
