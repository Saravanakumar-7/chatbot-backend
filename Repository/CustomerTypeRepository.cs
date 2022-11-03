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

        public async Task<string> ActivateCustomerType(int id)
        {

            var customerType = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();  
            if (customerType != null)
            {
                customerType.IsActive = true;
                TipsMasterDbContext.Add(customerType);
                await TipsMasterDbContext.SaveChangesAsync();
                
                return  "Data Succufully Activated!";
                
            }
            else
            {
                return  "Customer Type with the given id not found";
                
            }
            
        }

        public async Task<int?> CreateCustomerType(CustomerType customerType)
        {
            var result= await Create(customerType);
            return result.Id;
        }

        public async Task<string> DeactivateCustomerType(int id)
        {
            
            var customerType = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            if (customerType != null)
            {
                customerType.IsActive = false;
                TipsMasterDbContext.Add(customerType);
                await TipsMasterDbContext.SaveChangesAsync();

                return "Data Succufully DeActivated!";
            }
            else
            {
                return "Customer Type with the given id not found";
            }
            
        }

        public async Task<IEnumerable<CustomerType>> GetAllActiveCustomerTypes()
        {

            var customerTypeList = await FindByCondition(x => x.IsActive == true).ToListAsync();

            return customerTypeList;
        }

        public Task<IEnumerable<CustomerType>> GetAllCustomerTypes()
        {
            throw new NotImplementedException();
        }

        public Task<CustomerType> GetCustomerTypeById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
