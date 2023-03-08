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
            customerMaster.Unit = "Bangalore";
            var result = await Create(customerMaster);
            
            return result.Id;
        }
        public async Task<CustomerMaster> GetCSNumberAutoIncrementCount()
        {
            var cSNumberAutoIncrementCount = await TipsMasterDbContext.CustomerMasters.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            return cSNumberAutoIncrementCount;
        }

        public async Task<string> DeleteCustomerMaster(CustomerMaster customerMaster)
        {
            Delete(customerMaster);
            string result = $"CustomerMaster details are deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<CustomerIdNameListDto>> GetAllActiveCustomerMasterIdNameList()
        {
            IEnumerable<CustomerIdNameListDto> getAllActiveCustomerIdNameList = await TipsMasterDbContext.CustomerMasters
                                .Select(x => new CustomerIdNameListDto() 
                                {
                                    Id = x.Id,
                                    CustomerAliasName = x.CustomerAliasName,
                                    CustomerName = x.CustomerName,
                                    CustomerNumber = x.CustomerNumber

                                })
                                .OrderByDescending(x => x.Id)
                              .ToListAsync();   

            return getAllActiveCustomerIdNameList;
        }

        public async Task<IEnumerable<CustomerMaster>> GetAllActiveCustomerMasters()
        {
            var getAllActiveCustomermasterList = await FindAll().OrderByDescending(x=>x.Id).ToListAsync();
            return getAllActiveCustomermasterList;
        }

        public async Task<CustomerMaster> GetLatestCustomerMasterDetail()
        {
            var getLatestCustomermasterList = TipsMasterDbContext.CustomerMasters.OrderByDescending(x => x.Id).FirstOrDefault();
            return getLatestCustomermasterList;
        }

        

        public async Task<PagedList<CustomerMaster>> GetAllCustomerMasters(PagingParameter pagingParameter)
        {
            var getAllCustomerMasterList = PagedList<CustomerMaster>.ToPagedList(FindAll()
                                .Include(t => t.CustomerAddresses)
                                .Include(x => x.CustomerShippingAddresses)
                                .Include(m => m.CustomerContacts)
                                .Include(s => s.CustomerBanking)
                                .Include(v => v.CustomerMasterHeadCountings)
                                .OrderByDescending(x => x.Id), pagingParameter.PageNumber, pagingParameter.PageSize);


            return getAllCustomerMasterList;
        }         

        public async Task<CustomerMaster> GetCustomerMasterById(int id)
        {
            var getCustomerMasterById = await TipsMasterDbContext.CustomerMasters.Where(x => x.Id == id)
                              .Include(x => x.CustomerAddresses)
                              .Include(x => x.CustomerShippingAddresses)
                              .Include(m => m.CustomerContacts)
                              .Include(s=>s.CustomerBanking)
                              .Include(v => v.CustomerMasterHeadCountings)
                              .FirstOrDefaultAsync();

            return getCustomerMasterById;
        }

        public async Task<string> UpdateCustomerMaster(CustomerMaster customerMaster)
        {
            customerMaster.LastModifiedBy = "Admin";
            customerMaster.LastModifiedOn = DateTime.Now;
            Update(customerMaster);
            string result = $"CustomerMaster details are updated successfully!";
            return result;
        }

        public async Task<CustomerMaster> GetCustomerMasterByCustomerNo(string customerNumber)
        {
            var customerMasterDetails = await TipsMasterDbContext.CustomerMasters.Where(x => x.CustomerNumber == customerNumber)
                              .Include(x => x.CustomerAddresses)
                              .Include(x => x.CustomerShippingAddresses)
                              .Include(m => m.CustomerContacts)
                              .Include(s => s.CustomerBanking)
                              .Include(v => v.CustomerMasterHeadCountings)
                              .FirstOrDefaultAsync();

            return customerMasterDetails;
        }
    }
}
