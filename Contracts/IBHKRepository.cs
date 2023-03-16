using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public interface IBHKRepository : IRepositoryBase<BHK>
    {
        Task<PagedList<BHK>> GetAllBHK(PagingParameter pagingParameter, SearchParames searchParams);
        Task<BHK> GetBHKById(int id);
        Task<PagedList<BHK>> GetAllActiveBHK(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateBHK(BHK bHK);
        Task<string> UpdateBHK(BHK bHK);
        Task<string> DeleteBHK(BHK bHK);
    }
}