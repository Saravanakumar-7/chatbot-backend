using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEnggBomChildItemRepository
    {
        Task<PagedList<EnggChildItem>> GetAllChildItem(PagingParameter pagingParameter, SearchParames searchParams);
        Task<EnggChildItem> GetChildItemById(int id);
        Task<PagedList<EnggChildItem>> GetAllActiveChildItem(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateChildItem(EnggChildItem enggChildItem);
        Task<string> UpdateChildItem(EnggChildItem enggChildItem);
        Task<string> DeleteChildItem(EnggChildItem enggChildItem);
    }
}
