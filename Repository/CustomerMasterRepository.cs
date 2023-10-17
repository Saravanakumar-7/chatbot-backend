using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Entities.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CustomerMasterRepository: RepositoryBase<CustomerMaster>,ICustomerMasterRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public CustomerMasterRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateCustomerMaster(CustomerMaster customerMaster)
        {
            customerMaster.CreatedBy = _createdBy;
            customerMaster.CreatedOn = DateTime.Now;
            customerMaster.Unit = _unitname;
            var result = await Create(customerMaster);
            
            return result.Id;
        }
        public async Task<CustomerMaster> GetCSNumberAutoIncrementCount()
        {
            var cSNumberAutoIncrementCount = await TipsMasterDbContext.CustomerMasters.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            return cSNumberAutoIncrementCount;
        }
        //for Avision CustomerMaster Format
        public async Task<string> GenerateCustomerNumberAvision()
        {
            using var transaction = await TipsMasterDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var rfqNumberEntity = await TipsMasterDbContext.CSNOs.SingleAsync();
                rfqNumberEntity.CurrentValue += 1;
                TipsMasterDbContext.Update(rfqNumberEntity);
                await TipsMasterDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                int currentYear = DateTime.Now.Year % 100; // Get the last two digits of the current year
                int nextYear = (DateTime.Now.Year + 1) % 100; // Get the last two digits of the next year

                return $"ASPL|CS|{currentYear:D2}{nextYear:D2}-{rfqNumberEntity.CurrentValue:D6}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task<string> DeleteCustomerMaster(CustomerMaster customerMaster)
        {
            Delete(customerMaster);
            string result = $"CustomerMaster details are deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<CustomerIdNameListDto>> GetAllCustomerMasterIdNameList()
        {
            IEnumerable<CustomerIdNameListDto> getAllActiveCustomerIdNameList = await TipsMasterDbContext.CustomerMasters
                                              
                                .Select(x => new CustomerIdNameListDto() 
                                {
                                    Id = x.Id,
                                    CustomerAliasName = x.CustomerAliasName,
                                    CustomerName = x.CustomerName,
                                    CustomerNumber = x.CustomerNumber,
                                    CustomerCategory = x.CustomerCategory,
                                    CustomerType = x.CustomerType,

                                })
                                .OrderByDescending(x => x.Id)
                              .ToListAsync();   

            return getAllActiveCustomerIdNameList;
        }

        public async Task<IEnumerable<CustomerIdNameListDto>> GetAllActiveCustomerMasterIdNameList()
        {
            IEnumerable<CustomerIdNameListDto> getAllActiveCustomerIdNameList = await TipsMasterDbContext.CustomerMasters
                                .Where(x => x.IsActive == true)
                                .Select(x => new CustomerIdNameListDto()
                                {
                                    Id = x.Id,
                                    CustomerAliasName = x.CustomerAliasName,
                                    CustomerName = x.CustomerName,
                                    CustomerNumber = x.CustomerNumber,
                                 
                                })
                                .OrderByDescending(x => x.Id)
                              .ToListAsync();

            return getAllActiveCustomerIdNameList;
        }

        //public async Task<PagedList<CustomerMaster>> GetAllActiveCustomerMasters([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        //{
        //    var customermasterDetails = FindAll()
        //         .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CustomerName.Contains(searchParams.SearchValue) ||
        //            inv.CustomerAliasName.Contains(searchParams.SearchValue))))
        //           .Include(t => t.CustomerAddresses)
        //     .Include(t => t.CustomerShippingAddresses)
        //     .Include(t => t.CustomerContacts)
        //      .Include(d => d.CustomerBanking)
        //      .Include(d => d.CustomerMasterHeadCountings);

        //    return PagedList<CustomerMaster>.ToPagedList(customermasterDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}

        public async Task<IEnumerable<CustomerMaster>> GetAllActiveCustomerMasters()
        {
            var allActiveCompanyMasters = await FindByCondition(x => x.IsActive == true)
            .Include(t => t.CustomerAddresses)
            .Include(t => t.RelatedCustomers)
            .Include(t => t.CustomerShippingAddresses)
            .Include(t => t.CustomerContacts)
            .Include(d => d.CustomerBanking)
            .Include(d => d.CustomerMasterHeadCountings)
            .ToListAsync();
            return allActiveCompanyMasters;
        }

        public async Task<CustomerMaster> GetLatestCustomerMasterDetail()
        {
            var getLatestCustomermasterList = TipsMasterDbContext.CustomerMasters.OrderByDescending(x => x.Id).FirstOrDefault();
            return getLatestCustomermasterList;
        }

        
            public async Task<PagedList<CustomerMaster>> GetAllCustomerMasters([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
            {
                var customermasterDetails = FindAll().OrderByDescending(x => x.Id)
                  .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CustomerName.Contains(searchParams.SearchValue) ||
                                         inv.CustomerAliasName.Contains(searchParams.SearchValue) || inv.CustomerNumber.Contains(searchParams.SearchValue))))
                    .Include(t => t.CustomerAddresses)
                   .Include(t => t.RelatedCustomers)
                 .Include(t => t.CustomerShippingAddresses)
                 .Include(t => t.CustomerContacts)
                  .Include(d => d.CustomerBanking)
                  .Include(d => d.CustomerMasterHeadCountings);

                return PagedList<CustomerMaster>.ToPagedList(customermasterDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
            }


            public async Task<CustomerMaster> GetCustomerMasterById(int id)
        {
            var getCustomerMasterById = await TipsMasterDbContext.CustomerMasters.Where(x => x.Id == id)
                              .Include(x => x.CustomerAddresses)
                              .Include(t => t.RelatedCustomers)
                              .Include(x => x.CustomerShippingAddresses)
                              .Include(m => m.CustomerContacts)
                              .Include(s=>s.CustomerBanking)
                              .Include(v => v.CustomerMasterHeadCountings)
                              .FirstOrDefaultAsync();

            return getCustomerMasterById;
        }

        public async Task<string> UpdateCustomerMaster(CustomerMaster customerMaster)
        {
            customerMaster.LastModifiedBy = _createdBy;
            customerMaster.LastModifiedOn = DateTime.Now;
            Update(customerMaster);
            string result = $"CustomerMaster details are updated successfully!";
            return result;
        }

        public async Task<CustomerMaster> GetCustomerMasterByCustomerNo(string customerNumber)
        {
            var customerMasterDetails = await TipsMasterDbContext.CustomerMasters.Where(x => x.CustomerNumber == customerNumber)
                              .Include(x => x.CustomerAddresses)
                              .Include(t => t.RelatedCustomers)
                              .Include(x => x.CustomerShippingAddresses)
                              .Include(m => m.CustomerContacts)
                              .Include(s => s.CustomerBanking)
                              .Include(v => v.CustomerMasterHeadCountings)
                              .FirstOrDefaultAsync();

            return customerMasterDetails;
        }
    }
}
