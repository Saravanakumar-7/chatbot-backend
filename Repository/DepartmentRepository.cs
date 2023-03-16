using Contracts;
using Entities;
using Entities.Helper;
using Entities.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DepartmentRepository : RepositoryBase<Department>, IDepartmentRepository
    {
        public DepartmentRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateDepartment(Department department)
        {
            department.CreatedBy = "Admin";
            department.CreatedOn = DateTime.Now;
            department.Unit = "Bangalore";
            var result = await Create(department);
            
            return result.Id;
        }

        public async Task<string> DeleteDepartment(Department department)
        {
            Delete(department);
            string result = $"Department details of {department.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<Department>> GetAllActiveDepartment([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var departmentDetails = FindAll()
                       .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.DepartmentName.Contains(searchParams.SearchValue) ||
                       inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<Department>.ToPagedList(departmentDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<Department>> GetAllDepartment([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var departmentDetails = FindAll().OrderByDescending(x => x.Id)
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.DepartmentName.Contains(searchParams.SearchValue) ||
             inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<Department>.ToPagedList(departmentDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<Department> GetDepartmentById(int id)
        {
            var DepartmentbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return DepartmentbyId;
        }

        public async Task<string> UpdateDepartment(Department department)
        {
            department.LastModifiedBy = "Admin";
            department.LastModifiedOn = DateTime.Now;
            Update(department);
            string result = $"Department of Detail {department.Id} is updated successfully!";
            return result;
        }
    }
}
