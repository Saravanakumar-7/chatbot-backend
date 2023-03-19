using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface ICategoryRepository : IRepositoryBase<Category>
    {
        Task<PagedList<Category>> GetAllCategory(PagingParameter pagingParameter, SearchParames searchParames);
        Task<Category> GetCategoryById(int id);
        Task<PagedList<Category>> GetAllActiveCategory(PagingParameter pagingParameter, SearchParames searchParames);
        Task<int?> CreateCategory(Category category);
        Task<string> UpdateCategory(Category category);
        Task<string> DeleteCategory(Category category);
    }
}
