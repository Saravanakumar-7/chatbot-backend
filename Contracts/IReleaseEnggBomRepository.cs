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
    public interface IReleaseEnggBomRepository : IRepositoryBase<ReleaseEnggBom>
    {
        Task<PagedList<ReleaseEnggBom>> GetAllReleaseEnggBom(PagingParameter pagingParameter);
        Task<ReleaseEnggBom> GetReleaseEnggBomById(int id);
        Task<IEnumerable<ReleaseEnggBom>> GetAllActiveReleaseEnggBom();
        Task<int?> CreateReleaseEnggBom(ReleaseEnggBom releaseEnggBom);
        Task<string> UpdateReleaseEnggBom(ReleaseEnggBom releaseEnggBom);
        Task<string> DeleteReleaseEnggBom(ReleaseEnggBom releaseEnggBom);
        Task<ReleaseEnggBom> ReleasedEnggBomByItemAndRevisionNumber(string itemNumber, decimal revisionNumber);
        Task<ReleaseEnggBom> ReleasedEnggProductionByItemAndRevisionNumber(string itemNumber, decimal revisionNumber);
    }
}
