using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Entities.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CompanyMasterRepository : RepositoryBase<CompanyMaster>, ICompanyMasterRepository
    {
        public CompanyMasterRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateCompanyMaster(CompanyMaster companyMaster)
        {
            companyMaster.CreatedBy = "Admin";
            companyMaster.CreatedOn = DateTime.Now;
            companyMaster.Unit = "Bangalore";
            var result = await Create(companyMaster);
            
            return result.Id;
        }

        public async Task<string> DeleteCompanyMaster(CompanyMaster companyMaster)
        {
            Delete(companyMaster);
            string result = $"CompanyMaster details of {companyMaster.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<CompanyIdNameListDto>> GetAllActiveCompanyMasterIdNameList()
        {
            IEnumerable<CompanyIdNameListDto> getAllActiveCompanyMasterIdNameList = await TipsMasterDbContext.CompanyMasters
                                .Where(x=>x.IsActive == true)                
                                .Select(x => new CompanyIdNameListDto()
                                {
                                    Id = x.Id,
                                    CompanyId = x.CompanyId,
                                    CompanyAliasName = x.CompanyAliasName,
                                    CompanyName = x.CompanyName,
                                    CompanyCategory = x.CompanyCategory,
                                    CompanyType = x.CompanyType
                                })
                              .ToListAsync();

            return getAllActiveCompanyMasterIdNameList;
        }

        public async Task<IEnumerable<CompanyIdNameListDto>> GetAllCompanyMasterIdNameList()
        {
            IEnumerable<CompanyIdNameListDto> getAllActiveCompanyMasterIdNameList = await TipsMasterDbContext.CompanyMasters

                                .Select(x => new CompanyIdNameListDto()
                                {
                                    Id = x.Id,
                                    CompanyId = x.CompanyId,
                                    CompanyAliasName = x.CompanyAliasName,
                                    CompanyName = x.CompanyName,
                                    CompanyCategory = x.CompanyCategory,
                                    CompanyType = x.CompanyType
                                })
                              .ToListAsync();

            return getAllActiveCompanyMasterIdNameList;
        }

        public async Task<IEnumerable<CompanyMaster>> GetAllActiveCompanyMasters()
        {
            var allActiveCompanyMasters = await FindByCondition(x => x.IsActive == true)
            .Include(t => t.CompanyAddresses)
            .Include(x => x.CompanyContacts)
            .Include(m => m.CompanyBankings)
            .Include(v => v.CompanyMasterHeadCountings).ToListAsync();
            return allActiveCompanyMasters;
        }
        public async Task<PagedList<CompanyMaster>> GetAllCompanyMasters([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var companymasterDetails = FindAll().OrderByDescending(x => x.Id)
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CompanyName.Contains(searchParams.SearchValue) ||
                inv.CompanyId.Contains(searchParams.SearchValue) || inv.CompanyType.Contains(searchParams.SearchValue) || inv.PurchaseGroup.Contains(searchParams.SearchValue)
                                 || inv.CompanyAliasName.Contains(searchParams.SearchValue))))
                                                                                                                                                                            
             .Include(t => t.CompanyAddresses)
             .Include(t => t.CompanyBankings)
             .Include(t => t.CompanyContacts)
              .Include(d => d.CompanyMasterHeadCountings);

            return PagedList<CompanyMaster>.ToPagedList(companymasterDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }


        public async Task<CompanyMaster> GetCompanyMasterById(int id)
        {
            var getCompanyMasterById = await TipsMasterDbContext.CompanyMasters.Where(x => x.Id == id)
                                .Include(t => t.CompanyAddresses)
                                .Include(x => x.CompanyContacts)
                                .Include(m => m.CompanyBankings)
                                .Include(v => v.CompanyMasterHeadCountings)
                                .FirstOrDefaultAsync();

            return getCompanyMasterById;
        }

        public async Task<string> UpdateCompanyMaster(CompanyMaster companyMaster)
        {
            companyMaster.LastModifiedBy = "Admin";
            companyMaster.LastModifiedOn = DateTime.Now;
            Update(companyMaster);
            string result = $"companyMaster of Detail {companyMaster.Id} is updated successfully!";
            return result;
        }
    }
}
