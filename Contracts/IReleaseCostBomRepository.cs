using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.DTOs;

namespace Contracts
{
    public interface IReleaseCostBomRepository : IRepositoryBase<ReleaseCostBom>
    {
        Task<int?> CreateReleaseCostBom(ReleaseCostBom releaseCostBom);
        Task<IEnumerable<object>> GetAllReleaseCostBomItemNumberVersionList();
        Task<ReleaseCostBom> ReleasedCostBomByItemAndRevisionNumber(string itemNumber, decimal revisionNumber);
    }
}
