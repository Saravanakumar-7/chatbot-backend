using Contracts;
using Entities;
using Entities.Helper;
using Entities.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DepartmentRepository : RepositoryBase<Department>, IDepartmentRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public DepartmentRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateDepartment(Department department)
        {
            department.CreatedBy = _createdBy;
            department.CreatedOn = DateTime.Now;
            department.Unit = _unitname;
            var result = await Create(department);
            
            return result.Id;
        }

        public async Task<string> DeleteDepartment(Department department)
        {
            Delete(department);
            string result = $"Department details of {department.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Department>> GetAllActiveDepartment()
        {
            var departmentDetails = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return departmentDetails;
        }

        public async Task<IEnumerable<Department>> GetAllDepartment()
        {
            var departmentDetails = await FindAll().ToListAsync();
            return departmentDetails;
        }

        public async Task<Department> GetDepartmentById(int id)
        {
            var DepartmentbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return DepartmentbyId;
        }

        public async Task<string> UpdateDepartment(Department department)
        {
            department.LastModifiedBy = _createdBy;
            department.LastModifiedOn = DateTime.Now;
            Update(department);
            string result = $"Department of Detail {department.Id} is updated successfully!";
            return result;
        }
    }
}
