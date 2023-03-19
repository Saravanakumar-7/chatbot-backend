using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<PagedList<LeadStatus>> GetAllActiveLeadStatus([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {

            var LeadStatusDetails = FindAll()
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.LeadStatusName.Contains(searchParams.SearchValue) ||
             inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<LeadStatus>.ToPagedList(LeadStatusDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<LeadStatus>> GetAllLeadStatus([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {

            var LeadStatusDetails = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.LeadStatusName.Contains(searchParams.SearchValue) ||
                 inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<LeadStatus>.ToPagedList(LeadStatusDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
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
