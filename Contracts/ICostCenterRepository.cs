using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICostCenterRepository : IRepositoryBase<CostCenter>

    {
        Task<IEnumerable<CostCenter>> GetAllCostCenters();
        Task<CostCenter> GetCostCenterById(int id);
        Task<IEnumerable<CostCenter>> GetAllActiveCostCenters();
        Task<int?> CreateCostCenter(CostCenter costCenter);
        Task<string> UpdateCostCenter(CostCenter costCenter);
        Task<string> DeleteCostCenter(CostCenter costCenter);
    }
}
