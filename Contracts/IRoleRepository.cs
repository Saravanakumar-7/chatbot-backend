using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Helper;
using Entities;

namespace Contracts
{
    public interface IRoleRepository : IRepositoryBase<Role>
    {
        Task<PagedList<Role>> GetAllRoles(PagingParameter pagingParameter, SearchParames searchParams);
        Task<Role> GetRoleById(int id);
        Task<PagedList<Role>> GetAllActiveRoles(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateRole(Role role);
        Task<string> UpdateRole(Role role);
        Task<string> DeleteRole(Role role);
    }
}
