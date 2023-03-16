using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
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
            category.Unit = "Bangalore";
            var result = await Create(category);
            return result.Id;
           
        }

        public async Task<string> DeleteCategory(Category category)
        {
            Delete(category);
            string result = $"Category details of {category.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<Category>> GetAllActiveCategory([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var categoryDetails = FindAll()
                                    .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CategoryName.Contains(searchParams.SearchValue) ||
                                    inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<Category>.ToPagedList(categoryDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<Category>> GetAllCategory([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParames)
        {
            var allcategoryDetails = FindAll().OrderByDescending(x => x.Id)
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParames.SearchValue) || inv.CategoryName.Contains(searchParames.SearchValue) ||
            inv.Description.Contains(searchParames.SearchValue))));

            return PagedList<Category>.ToPagedList(allcategoryDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<Category> GetCategoryById(int id)
        {
            var CategorybyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return CategorybyId;
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
