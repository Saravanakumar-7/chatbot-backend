using Contracts;
using Entities;
using Entities.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace Repository
{
    public class VendorCategoryRepository : RepositoryBase<VendorCategory>, IVendorCategoryRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public VendorCategoryRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateVendorCategory(VendorCategory vendorCategory)
        {
            vendorCategory.CreatedBy = _createdBy;
            vendorCategory.CreatedOn = DateTime.Now;
            vendorCategory.Unit = _unitname;
            var result = await Create(vendorCategory);
            
            return result.Id;
        }

        public async Task<string> DeleteVendorCategory(VendorCategory vendorCategory)
        {
            Delete(vendorCategory);
            string result = $"VendorCategory details of {vendorCategory.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<VendorCategory>> GetAllActiveVendorCategory()
        {
            var AllActiveVendorCategories = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveVendorCategories;
        }

        public async Task<IEnumerable<VendorCategory>> GetAllVendorCategory()
        {
            var GetallVendorCategories = await FindAll().ToListAsync();

            return GetallVendorCategories;
        }

        public async Task<VendorCategory> GetVendorCategoryById(int id)
        {
            var vendorCategorybyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return vendorCategorybyId;
        }

        public async Task<string> UpdateVendorCategory(VendorCategory vendorCategory)
        {
            vendorCategory.LastModifiedBy = _createdBy;
            vendorCategory.LastModifiedOn = DateTime.Now;
            Update(vendorCategory);
            string result = $"Vendor Category of Detail {vendorCategory.Id} is updated successfully!";
            return result;
        }
    }
}
