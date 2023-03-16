using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
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
            leadTime.Unit = "Bangalore";
            var result = await Create(leadTime);
           
            return result.Id;
        }

        public async Task<string> DeleteLeadTime(LeadTime leadTime)
        {
            Delete(leadTime);
            string result = $"LeadTime details of {leadTime.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<LeadTime>> GetAllActiveLeadTime([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var leadTimeDetails = FindAll()
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.Days.Contains(searchParams.SearchValue) ||
            inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<LeadTime>.ToPagedList(leadTimeDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<LeadTime>> GetAllLeadTime([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var leadTimeDetails = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.Days.Contains(searchParams.SearchValue) ||
                 inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<LeadTime>.ToPagedList(leadTimeDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<LeadTime> GetLeadTimeById(int id)
        {
            var LeadTimebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return LeadTimebyId;
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
