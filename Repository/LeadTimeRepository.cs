using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class LeadTimeRepository:RepositoryBase<LeadTime>,ILeadTimeRepository
    {
        public LeadTimeRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateLeadTime(LeadTime leadTime)
        {
            leadTime.CreatedBy = "Admin";
            leadTime.CreatedOn = DateTime.Now;
            var result = await Create(leadTime);
            leadTime.Unit = "Bangalore";
            return result.Id;
        }

        public async Task<string> DeleteLeadTime(LeadTime leadTime)
        {
            Delete(leadTime);
            string result = $"LeadTime details of {leadTime.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<LeadTime>> GetAllActiveLeadTime()
        {
            var LeadTimeList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return LeadTimeList;
        }

        public async Task<IEnumerable<LeadTime>> GetAllLeadTime()
        {
            var LeadTimeList = await FindAll().ToListAsync();

            return LeadTimeList;
        }

        public async Task<LeadTime> GetLeadTimeById(int id)
        {
            var LeadTimeList = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return LeadTimeList;
        }

        public async Task<string> UpdateLeadTime(LeadTime leadTime)
        {
            leadTime.LastModifiedBy = "Admin";
            leadTime.LastModifiedOn = DateTime.Now;
            Update(leadTime);
            string result = $"LeadTime details of {leadTime.Id} is updated successfully!";
            return result;
        }
    }
}
