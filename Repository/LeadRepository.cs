using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class LeadRepository : RepositoryBase<Lead>, ILeadRepository
    {
        public LeadRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateLead(Lead lead)
        {
            lead.CreatedBy = "Admin";
            lead.CreatedOn = DateTime.Now;
            lead.LastModifiedBy = "Admin";
            lead.LastModifiedOn = DateTime.Now;
            lead.Unit = "Bangalore";
            var result = await Create(lead);
            
            return result.Id;

        }

        public async Task<string> DeleteLead(Lead lead)
        {
            Delete(lead);
            string result = $"lead details of {lead.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<Lead>> GetAllLeads(PagingParameter pagingParameter)
        {
            var GetallleadDetails = PagedList<Lead>.ToPagedList(FindAll()
                                .Include(x => x.LeadAddress)
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return GetallleadDetails;
        }      

        public async Task<Lead> GetLeadById(int id)
        {
            var LeadDetailsbyId = await TipsMasterDbContext.Leads.Where(x => x.Id == id)                               
                               .Include(v => v.LeadAddress)
                               .FirstOrDefaultAsync();
            return LeadDetailsbyId;
        }

        public async Task<string> UpdateLead(Lead lead)
        {
            lead.LastModifiedBy = "Admin";
            lead.LastModifiedOn = DateTime.Now;
            Update(lead);
            string result = $"Lead of Detail {lead.Id} is updated successfully!";
            return result;
        }
    }
}
