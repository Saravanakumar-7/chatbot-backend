using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public CategoryRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateCategory(Category category)
        {
            category.CreatedBy = _createdBy;
            category.CreatedOn = DateTime.Now;
            category.Unit = _unitname;
            var result = await Create(category);
            return result.Id;
           
        }

        public async Task<string> DeleteCategory(Category category)
        {
            Delete(category);
            string result = $"Category details of {category.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Category>> GetAllActiveCategory([FromQuery] SearchParames searchParams)
        {
            var categoryDetails = FindAll()
                                       .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CategoryName.Contains(searchParams.SearchValue) ||
                                 inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return categoryDetails;
        }

        public async Task<IEnumerable<Category>> GetAllCategory([FromQuery] SearchParames searchParams)
        {
            var categoryDetails = FindAll()
                                        .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CategoryName.Contains(searchParams.SearchValue) ||
                                  inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return categoryDetails;
        }

        public async Task<Category> GetCategoryById(int id)
        {
            var CategorybyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return CategorybyId;
        }

        public async Task<string> UpdateCategory(Category category)
        {
            category.LastModifiedBy = _createdBy;
            category.LastModifiedOn = DateTime.Now;
            Update(category);
            string result = $"Category details of {category.Id} is updated successfully!";
            return result;
        }
    }
}
