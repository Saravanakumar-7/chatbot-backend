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
        Task<PagedList<CostCenter>> GetAllCostCenters(PagingParameter pagingParameter, SearchParames searchParams);
        Task<CostCenter> GetCostCenterById(int id);
        Task<PagedList<CostCenter>> GetAllActiveCostCenters(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateCostCenter(CostCenter costCenter);
        Task<string> UpdateCostCenter(CostCenter costCenter);
        Task<string> DeleteCostCenter(CostCenter costCenter);
    }
}
