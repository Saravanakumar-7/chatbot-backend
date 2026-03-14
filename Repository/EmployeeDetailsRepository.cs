using Contracts;
using Entities;
using System;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EmployeeDetailsRepository : RepositoryBase<EmployeeDetails>, IEmployeeDetailsRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _createdBy;
        private readonly string _unit;

        public EmployeeDetailsRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Admin";
            _unit = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Chennai";
        }

        public async Task<IEnumerable<EmployeeDetails>> GetAllEmployeeDetails([FromQuery] SearchParames searchParams)
        {
            int.TryParse(searchParams.SearchValue, out int intValue);
            decimal.TryParse(searchParams.SearchValue, out decimal decimalValue);

            var empDetails = FindAll()
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.Emp_id == intValue || inv.Emp_name.Contains(searchParams.SearchValue) || inv.Emp_salary == decimalValue ||
          inv.Emp_description.Contains(searchParams.SearchValue))));
            return empDetails;
        }

        public async Task<EmployeeDetails> GetEmployeeDetailsById(int id)
        {
            var empDetails = FindByCondition(emp => emp.Id.Equals(id)).FirstOrDefault();
            return empDetails;
        }
        public async Task<int> CreateEmployeeDetails(EmployeeDetails employeeDetails)
        {
            employeeDetails.CreatedBy = _createdBy;
            employeeDetails.CreatedOn = DateTime.Now;
            employeeDetails.Unit = _unit;
            var result = await Create(employeeDetails);
            return result.Id;
        }

        public async Task<string> UpdateEmployeeDetails(EmployeeDetails employeeDetails)
        {
            employeeDetails.LastmodifiedBy = _createdBy;
            employeeDetails.LastModifiedOn = DateTime.Now;
            Update(employeeDetails);
            string result = $"Employee details of {employeeDetails.Id} has been updated successfully!";
            return result;
        }

        public async Task<string> DeleteEmployeeDetails(EmployeeDetails employeeDetails)
        {
            Delete(employeeDetails);
            string result = $"Employee details of {employeeDetails.Id} has been deleted successfully!";
            return result;
        }



    }
}
