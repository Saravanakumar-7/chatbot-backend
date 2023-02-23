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
    public interface IEnggBomGroupRepository : IRepositoryBase<EnggBomGroup>
    {
        Task<PagedList<EnggBomGroup>> GetAllEnggBomGroup(PagingParameter pagingParameter);
        Task<EnggBomGroup> GetEnggBomGroupById(int id);
        Task<IEnumerable<EnggBomGroup>> GetAllActiveEnggBomGroup();
        Task<int?> CreateEnggBomGroup(EnggBomGroup enggbomGroup);
        Task<string> UpdateEnggBomGroup(EnggBomGroup enggbomGroup);
        Task<string> DeleteEnggBomGroup(EnggBomGroup enggbomGroup);
        Task<IEnumerable<ListOfBomGroupDto>> GetAllBomGroupList();
    }
}
