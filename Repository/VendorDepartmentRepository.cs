using Contracts;
using Entities;
using Entities.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Repository
{
    public class VendorDepartmentRepository : RepositoryBase<VendorDepartment>, IVendorDepartmentRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public VendorDepartmentRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateVendorDepartment(VendorDepartment vendorDepartment)
        {
            vendorDepartment.CreatedBy = _createdBy;
            vendorDepartment.CreatedOn = DateTime.Now;
            vendorDepartment.Unit = _unitname;
            var result = await Create(vendorDepartment);
            
            return result.Id;
        }

        public async Task<string> DeleteVendorDepartment(VendorDepartment vendorDepartment)
        {
            Delete(vendorDepartment);
            string result = $"Vendor Department details of {vendorDepartment.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<VendorDepartment>> GetAllActiveVendorDepartment([FromQuery] SearchParames searchParams)
        {
            var vendorDepartmentDetails = FindAll()
                                       .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.VendorDepartmentName.Contains(searchParams.SearchValue) ||
                                 inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return vendorDepartmentDetails;
        }

        public async Task<IEnumerable<VendorDepartment>> GetAllVendorDepartment([FromQuery] SearchParames searchParams)
        {
            var vendorDepartmentDetails = FindAll()
                                        .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.VendorDepartmentName.Contains(searchParams.SearchValue) ||
                                  inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return vendorDepartmentDetails;
        }

        public async Task<VendorDepartment> GetVendorDepartmentById(int id)
        {
            var VendorDepartmentbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return VendorDepartmentbyId;
        }

        public async Task<string> UpdateVendorDepartment(VendorDepartment vendorDepartment)
        {
            vendorDepartment.LastModifiedBy = _createdBy;
            vendorDepartment.LastModifiedOn = DateTime.Now;
            Update(vendorDepartment);
            string result = $"Vendor Department of Detail {vendorDepartment.Id} is updated successfully!";
            return result;
        }
    }
}
