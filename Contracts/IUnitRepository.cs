using Entities.Helper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUnitRepository : IRepositoryBase<Unit>
    {
        Task<PagedList<Unit>> GetAllUnits(PagingParameter pagingParameter, SearchParames searchParams);


        Task<Unit> GetUnitById(int id);
        Task<PagedList<Unit>> GetAllActiveUnits(PagingParameter pagingParameter, SearchParames searchParams);

        Task<int?> CreateUnit(Unit unit);
        Task<string> UpdateUnit(Unit unit);
        Task<string> DeleteUnit(Unit unit);
    }
}
