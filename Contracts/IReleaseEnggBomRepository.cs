using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Helper;
using Entities;
using Entities.DTOs;

namespace Contracts
{
    public interface IReleaseEnggBomRepository : IRepositoryBase<EngineeringBom>
    {
        Task<PagedList<EngineeringBom>> GetAllReleaseEnggBom(PagingParameter pagingParameter);
        Task<EngineeringBom> GetReleaseEnggBomById(int id);
        Task<IEnumerable<EngineeringBom>> GetAllActiveReleaseEnggBom();
        Task<int?> CreateReleaseEnggBom(EngineeringBom releaseEnggBom);
        Task<string> UpdateReleaseEnggBom(EngineeringBom releaseEnggBom);
        Task<string> DeleteReleaseEnggBom(EngineeringBom releaseEnggBom);
        Task<EngineeringBom> ReleasedEnggBomByItemAndRevisionNumber(string itemNumber, decimal revisionNumber);
        Task<EngineeringBom> ReleasedEnggProductionByItemAndRevisionNumber(string itemNumber, decimal revisionNumber);
    }
}
