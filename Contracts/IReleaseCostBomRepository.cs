using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.DTOs;

namespace Contracts
{
    public interface IReleaseCostBomRepository : IRepositoryBase<CostingBom>
    {
        Task<int?> CreateReleaseCostBom(CostingBom releaseCostBom);
        Task<IEnumerable<object>> GetAllReleaseCostBomItemNumberVersionList();
        Task<CostingBom> ReleasedCostBomByItemAndRevisionNumber(string itemNumber, decimal revisionNumber);
    }
}
