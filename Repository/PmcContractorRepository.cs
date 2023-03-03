using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
namespace Repository
{
    public class PmcContractorRepository : RepositoryBase<PmcContractor>, IPmcContractorRepository
    {
        public PmcContractorRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        { }
        public async Task<int?> CreatePmcContractor(PmcContractor pmcContractor)
        {
            pmcContractor.CreatedBy = "Admin";
            pmcContractor.CreatedOn = DateTime.Now;
            pmcContractor.Unit = "Bangalore";
            var result = await Create(pmcContractor); return result.Id;
        }
        public async Task<string> DeletePmcContractor(PmcContractor pmcContractor)
        {
            Delete(pmcContractor);
            string result = $"city details of {pmcContractor.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<PmcContractor>> GetAllActivePmcContractor()
        {
            var AllActivepmcContractor = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivepmcContractor;
        }
        public async Task<IEnumerable<PmcContractor>> GetAllPmcContractor()
        {
            var GetallpmcContractor = await FindAll().ToListAsync(); return GetallpmcContractor;
        }
        public async Task<PmcContractor> GetPmcContractorById(int id)
        {
            var pmcContractorById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); return pmcContractorById;
        }
        public async Task<string> UpdatePmcContractor(PmcContractor pmcContractor)
        {
            pmcContractor.LastModifiedBy = "Admin";
            pmcContractor.LastModifiedOn = DateTime.Now;
            Update(pmcContractor);
            string result = $"pmcContractor details of {pmcContractor.Id} is updated successfully!";
            return result;
        }
    }
} 