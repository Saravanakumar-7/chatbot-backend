using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface IUserAccessRepository : IRepositoryBase<UserAccess>
    {
        Task<PagedList<UserAccess>> GetAllUserAccess(PagingParameter pagingParameter, SearchParames searchParams);
        Task<UserAccess> GetUserAccessById(int id);
        Task<PagedList<UserAccess>> GetAllActiveUserAccess(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateUserAccess(UserAccess userAccess);
        Task<string> UpdateUserAccess(UserAccess userAccess);
        Task<string> DeleteUserAccess(UserAccess userAccess);
    }
}
