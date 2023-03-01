using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.DTOs;
using Entities.Helper;

namespace Contracts
{
    public interface IReleaseCostBomRepository : IRepositoryBase<CostingBom>
    {
        Task<PagedList<CostingBom>> GetAllCostingBom(PagingParameter pagingParameter);
        Task<CostingBom> GetCostingBomById(int id);
        Task<int?> CreateReleaseCostBom(CostingBom releaseCostBom);
        Task<IEnumerable<object>> GetAllReleaseCostBomItemNumberVersionList();
        Task<CostingBom> ReleasedCostBomByItemAndRevisionNumber(string itemNumber, decimal revisionNumber);
        Task<IEnumerable<CostingBom>> GetAllCostingBomVersionListByItemNumber(string itemNumber);
    }
}
