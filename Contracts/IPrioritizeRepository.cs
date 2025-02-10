using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface IPrioritizeRepository : IRepositoryBase<Prioritize>
    {
        Task<PagedList<Prioritize>> GetAllPrioritize(PagingParameter pagingParameter, SearchParames searchParams);
        Task<Prioritize> GetPrioritizeById(int id);
        Task<int?> CreatePrioritize(Prioritize prioritize);
        Task<string> UpdatePrioritize(Prioritize prioritize);
        Task<string> DeletePrioritize(Prioritize prioritize);
    }
}
