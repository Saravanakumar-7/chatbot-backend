using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CustomerTypeRepository : RepositoryBase<CustomerType>, ICustomerTypeRepository
    {
        
        public CustomerTypeRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
            

        }

    

        public async Task<int?> CreateCustomerType(CustomerType customerType)
        {
            customerType.CreatedBy = "Admin";
            customerType.CreatedOn = DateTime.Now;
            customerType.Unit = "Bangalore";
            var result= await Create(customerType);
           
            return result.Id;
        }

       

        public async Task<string> DeleteCustomerType(CustomerType customerType)
        {
            Delete(customerType);
            string result = $"Customer Type details of {customerType.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<CustomerType>> GetAllActiveCustomerTypes([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var currencyDetails = FindAll()
                      .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CustomerTypeName.Contains(searchParams.SearchValue) ||
                      inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<CustomerType>.ToPagedList(currencyDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<CustomerType>> GetAllCustomerTypes([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var currencyDetails = FindAll().OrderByDescending(x => x.Id)
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CustomerTypeName.Contains(searchParams.SearchValue) ||
             inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<CustomerType>.ToPagedList(currencyDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }


        public async Task<CustomerType> GetCustomerTypeById(int id)
        {
            var CustomerTypebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return CustomerTypebyId;
        }

        public async Task<string> UpdateCustomerType(CustomerType customerType)
        {
            customerType.LastModifiedBy = "Admin";
            customerType.LastModifiedOn = DateTime.Now;
            Update(customerType);
            string result = $"Customer Type details of {customerType.Id} is updated successfully!";
            return result;
        }
    }
}
