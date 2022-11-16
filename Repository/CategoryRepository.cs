using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateCategory(Category category)
        {
            category.CreatedBy = "Admin";
            category.CreatedOn = DateTime.Now;
            var result = await Create(category);
            return result.Id;
        }

        public async Task<string> DeleteCategory(Category category)
        {
            Delete(category);
            string result = $"Category details of {category.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Category>> GetAllActiveCategory()
        {
            var categoryList = await FindByCondition(x => x.ActiveStatus == true).ToListAsync();
            return categoryList;
        }

        public async Task<IEnumerable<Category>> GetAllCategory()
        {
            var categoryList = await FindAll().ToListAsync();
            return categoryList;
        }

        public async Task<Category> GetCategoryById(int id)
        {
            var categoryList = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return categoryList;
        }

        public async Task<string> UpdateCategory(Category category)
        {
            category.LastModifiedBy = "Admin";
            category.LastModifiedOn = DateTime.Now;
            Update(category);
            string result = $"Category details of {category.Id} is updated successfully!";
            return result;
        }
    }
}
