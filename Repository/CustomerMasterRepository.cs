using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Entities.Migrations;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CustomerMasterRepository: RepositoryBase<CustomerMaster>,ICustomerMasterRepository
    {
        public CustomerMasterRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateCustomerMaster(CustomerMaster customerMaster)
        {
            customerMaster.CreatedBy = "Admin";
            customerMaster.CreatedOn = DateTime.Now;
            var result = await Create(customerMaster);
            customerMaster.Unit = "Bangalore";
            return result.Id;
        }

        public async Task<string> DeleteCustomerMaster(CustomerMaster customerMaster)
        {
            Delete(customerMaster);
            string result = $"customerMaster details are deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<CustomerIdNameListDto>> GetAllActiveCustomerIdNameList()
        {
            IEnumerable<CustomerIdNameListDto> customerDetails = await TipsMasterDbContext.CustomerMasters
                                .Select(x => new CustomerIdNameListDto() 
                                {
                                    Id = x.Id,
                                    CustomerAliasName = x.CustomerAliasName,
                                    CustomerName = x.CustomerName 
                                })
                              .ToListAsync();   

            return customerDetails;
        }

        public async Task<IEnumerable<CustomerMaster>> GetAllActiveCustomerMaster()
        {
            var customerDetails = await FindAll().ToListAsync();
            return customerDetails;
        }

        public async Task<PagedList<CustomerMaster>> GetAllCustomerMaster(PagingParameter pagingParameter)
        {
            var customerDetails = PagedList<CustomerMaster>.ToPagedList(FindAll()
                                .Include(t => t.CustomerAddresses)
                                .Include(x => x.CustomerShippingAddresses)
                                .Include(m => m.CustomerContacts)
                                .Include(s => s.CustomerBanking)
                                .Include(v => v.CustomerMasterHeadCountings)
                                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);


            return customerDetails;
        }         

        public async Task<CustomerMaster> GetCustomerMasterById(int id)
        {
            var customerDetails = await TipsMasterDbContext.CustomerMasters.Where(x => x.Id == id)
                              .Include(x => x.CustomerAddresses)
                              .Include(x => x.CustomerShippingAddresses)
                              .Include(m => m.CustomerContacts)
                              .Include(s=>s.CustomerBanking)
                              .Include(v => v.CustomerMasterHeadCountings)
                              .FirstOrDefaultAsync();

            return customerDetails;
        }

        public async Task<string> UpdateCustomerMaster(CustomerMaster customerMaster)
        {
            customerMaster.LastModifiedBy = "Admin";
            customerMaster.LastModifiedOn = DateTime.Now;
            Update(customerMaster);
            string result = $"customer details are updated successfully!";
            return result;
        }
    }
}
