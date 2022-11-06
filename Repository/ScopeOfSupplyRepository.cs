using Contracts;
using Entities;
using Entities.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ScopeOfSupplyRepository : RepositoryBase<ScopeOfSupply>, IScopeOfSupplyRepository

    {
        public ScopeOfSupplyRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateScopeOfSupply(ScopeOfSupply scopeOfSupply)
        {
            scopeOfSupply.CreatedBy = "Admin";
            scopeOfSupply.CreatedOn = DateTime.Now;
            var result = await Create(scopeOfSupply);
            return result.Id;
        }

        public async Task<string> DeleteScopeOfSupply(ScopeOfSupply scopeOfSupply)
        {
            Delete(scopeOfSupply);
            string result = $"Scope Of Supply details of {scopeOfSupply.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<ScopeOfSupply>> GetAllActiveScopeOfSupply()
        {
            var scopeOfSupplies = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return scopeOfSupplies;
        }

        public async Task<IEnumerable<ScopeOfSupply>> GetAllScopeOfSupply()
        {
            var scopeOfSupplies = await FindAll().ToListAsync();

            return scopeOfSupplies;
        }

        public async Task<ScopeOfSupply> GetScopeOfSupplyById(int id)
        {
            var scopeOfSupply = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return scopeOfSupply;
        }

        public async Task<string> UpdateScopeOfSupply(ScopeOfSupply scopeOfSupply)
        {
            scopeOfSupply.LastModifiedBy = "Admin";
            scopeOfSupply.LastModifiedOn = DateTime.Now;
            Update(scopeOfSupply);
            string result = $"Scope Of Supply of Detail {scopeOfSupply.Id} is updated successfully!";
            return result;
        }
    }
}
