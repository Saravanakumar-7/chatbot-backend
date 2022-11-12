using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Migrations;
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
            var result = await Create(companyMaster);
            return result.Id;
        }

        public async Task<string> DeleteCompanyMaster(CompanyMaster companyMaster)
        {
            Delete(companyMaster);
            string result = $"CompanyMaster details of {companyMaster.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<CompanyIdNameListDto>> GetAllActiveCompanyIdNameList()
        {
            IEnumerable<CompanyIdNameListDto> companyDetails = await TipsMasterDbContext.CompanyMasters
                                .Select(x => new CompanyIdNameListDto()
                                {
                                    Id = x.Id,
                                    CompanyAliasName = x.CompanyAliasName,
                                    CompanyName = x.CompanyName
                                })
                              .ToListAsync();

            return companyDetails;
        }

        public async Task<IEnumerable<CompanyMaster>> GetAllActiveCompanyMaster()
        {
            var companyMasterDetails = await FindAll().ToListAsync();
            return companyMasterDetails;
        }

        public async Task<IEnumerable<CompanyMaster>> GetAllCompanyMaster()
        {
            var companyMasterDetails = await TipsMasterDbContext.CompanyMasters
                                .Include(t => t.CompanyAddresses)
                                .Include(x => x.CompanyContacts)
                                .Include(m => m.CompanyBankings)
                                .ToListAsync();

            return companyMasterDetails;
        }

        public async Task<CompanyMaster> GetCompanyMasterById(int id)
        {
            var companyMasterDetails = await TipsMasterDbContext.CompanyMasters.Where(x => x.Id == id)
                                .Include(t => t.CompanyAddresses)
                                .Include(x => x.CompanyContacts)
                                .Include(m => m.CompanyBankings)
                                .FirstOrDefaultAsync();

            return companyMasterDetails;
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
