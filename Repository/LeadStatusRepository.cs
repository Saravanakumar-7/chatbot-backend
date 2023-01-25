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
    public class LeadStatusRepository : RepositoryBase<LeadStatus>, ILeadStatusRepository
    {
        public LeadStatusRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateLeadStatus(LeadStatus leadStatus)
        {
            leadStatus.CreatedBy = "Admin";
            leadStatus.CreatedOn = DateTime.Now;
            leadStatus.Unit = "Bangalore";
            var result = await Create(leadStatus);
            
            return result.Id;

        }

        public async Task<string> DeleteLeadStatus(LeadStatus leadStatus)
        {
            Delete(leadStatus);
            string result = $"leadStatus details of {leadStatus.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<LeadStatus>> GetAllActiveLeadStatus()
        {

            var AllActiveLeadStatus = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveLeadStatus;
        }

        public async Task<IEnumerable<LeadStatus>> GetAllLeadStatus()
        {

            var GetallLeadStatus = await FindAll().OrderByDescending(x => x.Id).ToListAsync();

            return GetallLeadStatus;
        }

        public async Task<LeadStatus> GetLeadStatusById(int id)
        {
            var LeadStatusbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return LeadStatusbyId;
        }
        public async Task<string> UpdateLeadStatus(LeadStatus leadStatus)
        {

            leadStatus.LastModifiedBy = "Admin";
            leadStatus.LastModifiedOn = DateTime.Now;
            Update(leadStatus);
            string result = $"leadStatus details of {leadStatus.Id} is updated successfully!";
            return result;
        }
    }
}
