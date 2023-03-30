using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface IRoleAccessRepository : IRepositoryBase<RoleAccess>
    {
        Task<PagedList<RoleAccess>> GetAllRoleAccess(PagingParameter pagingParameter, SearchParames searchParams);
        Task<RoleAccess> GetRoleAccessById(int id);
        Task<PagedList<RoleAccess>> GetAllActiveRoleAccess(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateRoleAccess(RoleAccess roleAccess);
        Task<string> UpdateRoleAccess(RoleAccess roleAccess);
        Task<string> DeleteRoleAccess(RoleAccess roleAccess);
        Task<IEnumerable<RoleAccess>> GetRoleAccessByRoleId(int roleId);
    }
}
