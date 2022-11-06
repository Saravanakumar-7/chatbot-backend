using Contracts;
using Entities;
using Entities.Migrations;
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
            var Department = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return Department;
        }

        public async Task<IEnumerable<Department>> GetAllDepartment()
        {
            var Departments= await FindAll().ToListAsync();

            return Departments;
        }

        public async Task<Department> GetDepartmentById(int id)
        {
            var department = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return department;
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
