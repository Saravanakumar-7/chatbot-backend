using Contracts;
using Entities;
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

        //public async Task<string> ActivateCustomerType(int id)
        //{

        //    var customerType = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();  
        //    if (customerType != null)
        //    {
        //        Update(customerType);
        //        customerType.IsActive = true;
        //        TipsMasterDbContext.Add(customerType);
        //        await TipsMasterDbContext.SaveChangesAsync();
                
        //        return  "Data Succufully Activated!";
                
        //    }
        //    else
        //    {
        //        return  "Customer Type with the given id not found";
                
        //    }
            
        //}

        public async Task<int?> CreateCustomerType(CustomerType customerType)
        {
            customerType.CreatedBy = "Admin";
            customerType.CreatedOn = DateTime.Now;
            var result= await Create(customerType);
            return result.Id;
        }

        //public async Task<string> DeactivateCustomerType(int id)
        //{
            
        //    var customerType = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
        //    if (customerType != null)
        //    {
        //        customerType.IsActive = false;
        //        TipsMasterDbContext.Add(customerType);
        //        await TipsMasterDbContext.SaveChangesAsync();

        //        return "Data Succufully DeActivated!";
        //    }
        //    else
        //    {
        //        return "Customer Type with the given id not found";
        //    }
            
        //}

        public async Task<string> DeleteCustomerType(CustomerType customerType)
        {
            Delete(customerType);
            string result = $"Customer Type details of {customerType.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<CustomerType>> GetAllActiveCustomerTypes()
        {
            var customerTypeList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return customerTypeList;
        }

        public async Task<IEnumerable<CustomerType>> GetAllCustomerTypes()
        {

            var customerTypeList = await FindAll().ToListAsync();

            return customerTypeList;
        }

        public async Task<CustomerType> GetCustomerTypeById(int id)
        {
            var customerType = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return customerType;
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
