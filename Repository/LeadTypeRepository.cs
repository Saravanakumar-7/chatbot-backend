using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class LeadTypeRepository : RepositoryBase<LeadType>, ILeadTypeRepository
    {
        public LeadTypeRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateLeadType(LeadType leadType)
        {
            leadType.CreatedBy = "Admin";
            leadType.CreatedOn = DateTime.Now;
            var result = await Create(leadType);
            leadType.Unit = "Bangalore";
            return result.Id;

        }

        public async Task<string> DeleteLeadType(LeadType leadType)
        {
            Delete(leadType);
            string result = $"leadType details of {leadType.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<LeadType>> GetAllActiveLeadTypes()
        {

            var leadTypeList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return leadTypeList;
        }

        public async Task<IEnumerable<LeadType>> GetAllLeadTypes()
        {

            var leadStatusList = await FindAll().ToListAsync();

            return leadStatusList;
        }

        public async Task<LeadType> GetLeadTypeById(int id)
        {
            var leadType = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return leadType;
        }
        public async Task<string> UpdateLeadType(LeadType leadType)
        {

            leadType.LastModifiedBy = "Admin";
            leadType.LastModifiedOn = DateTime.Now;
            Update(leadType);
            string result = $"leadStatus details of {leadType.Id} is updated successfully!";
            return result;
        }
    }
}
