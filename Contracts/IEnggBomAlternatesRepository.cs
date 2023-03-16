using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEnggBomAlternatesRepository
    {
        Task<PagedList<EnggAlternates>> GetAllEnggAlternate(PagingParameter pagingParameter, SearchParames searchParams);
        Task<EnggAlternates> GetEnggAlternateById(int id);
        Task<PagedList<EnggAlternates>> GetAllActiveEnggAlternate(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateEnggAlternate(EnggAlternates enggAlternates);
        Task<string> UpdateEnggAlternate(EnggAlternates enggAlternates);
        Task<string> DeleteEnggAlternate(EnggAlternates enggAlternates);
    }
}
