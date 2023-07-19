using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CustomerTypeRepository : RepositoryBase<CustomerType>, ICustomerTypeRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public CustomerTypeRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateCustomerType(CustomerType customerType)
        {
            customerType.CreatedBy = _createdBy;
            customerType.CreatedOn = DateTime.Now;
            customerType.Unit = _unitname;
            var result= await Create(customerType);
           
            return result.Id;
        }

       

        public async Task<string> DeleteCustomerType(CustomerType customerType)
        {
            Delete(customerType);
            string result = $"Customer Type details of {customerType.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<CustomerType>> GetAllActiveCustomerTypes([FromQuery] SearchParames searchParams)
        {
            var customerTypeDetails = FindAll()
          .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CustomerTypeName.Contains(searchParams.SearchValue) ||
         inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return customerTypeDetails;
        }

        public async Task<IEnumerable<CustomerType>> GetAllCustomerTypes([FromQuery] SearchParames searchParams)
        {
            var customerTypeDetails = FindAll()
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CustomerTypeName.Contains(searchParams.SearchValue) ||
          inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return customerTypeDetails;
        }


        public async Task<CustomerType> GetCustomerTypeById(int id)
        {
            var CustomerTypebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return CustomerTypebyId;
        }

        public async Task<string> UpdateCustomerType(CustomerType customerType)
        {
            customerType.LastModifiedBy = _createdBy;
            customerType.LastModifiedOn = DateTime.Now;
            Update(customerType);
            string result = $"Customer Type details of {customerType.Id} is updated successfully!";
            return result;
        }
    }
}
