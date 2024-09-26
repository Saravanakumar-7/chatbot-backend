using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class LeadRepository : RepositoryBase<Lead>, ILeadRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public LeadRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<Lead> CreateLead(Lead lead)
        { 
            var date = DateTime.Now;
            lead.CreatedBy = _createdBy;
            lead.CreatedOn = date.Date;
           // lead.LastModifiedBy = _createdBy;
           // lead.LastModifiedOn = DateTime.Now;
            lead.Unit = _unitname;
            var result = await Create(lead);
            
            return result;

        }

        public async Task<string> DeleteLead(Lead lead)
        {
            Delete(lead);
            string result = $"lead details of {lead.Id} is deleted successfully!";
            return result;
        }

        public async Task<int?> GetLeadIDIncrementCount(DateTime date)
        {
            var getBTONumberAutoIncrementCount = TipsMasterDbContext.Leads.Where(x => x.CreatedOn == date.Date).Count();

            return getBTONumberAutoIncrementCount;
        }

        public async Task<PagedList<Lead>> GetAllLeads([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var leadDetails = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ContactName.Contains(searchParams.SearchValue) ||
                 inv.CompanyName.Contains(searchParams.SearchValue))))
                .Include(t => t.LeadAddress);

            return PagedList<Lead>.ToPagedList(leadDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
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
            lead.LastModifiedBy = _createdBy;
            lead.LastModifiedOn = DateTime.Now;
            Update(lead);
            string result = $"Lead of Detail {lead.Id} is updated successfully!";
            return result;
        }
    }
}
