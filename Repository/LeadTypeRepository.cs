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
    public class LeadTypeRepository : RepositoryBase<LeadType>, ILeadTypeRepository
    {
        public LeadTypeRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateLeadType(LeadType leadType)
        {
            leadType.CreatedBy = "Admin";
            leadType.CreatedOn = DateTime.Now;
            leadType.Unit = "Bangalore";
            var result = await Create(leadType);
            
            return result.Id;

        }

        public async Task<string> DeleteLeadType(LeadType leadType)
        {
            Delete(leadType);
            string result = $"leadType details of {leadType.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<LeadType>> GetAllActiveLeadTypes([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {

            var leadTypeDetails = FindAll()
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.LeadTypeName.Contains(searchParams.SearchValue) ||
            inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<LeadType>.ToPagedList(leadTypeDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<LeadType>> GetAllLeadTypes([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {

            var leadTypeDetails = FindAll().OrderByDescending(x => x.Id)
               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.LeadTypeName.Contains(searchParams.SearchValue) ||
                  inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<LeadType>.ToPagedList(leadTypeDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<LeadType> GetLeadTypeById(int id)
        {
            var LeadTypebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return LeadTypebyId;
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
