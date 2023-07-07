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
        Task<IEnumerable<Category>> GetAllCategory();
        Task<Category> GetCategoryById(int id);
        Task<IEnumerable<Category>> GetAllActiveCategory();
        Task<int?> CreateCategory(Category category);
        Task<string> UpdateCategory(Category category);
        Task<string> DeleteCategory(Category category);
    }
}
