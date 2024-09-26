using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class LeadWebsiteRepository : RepositoryBase<LeadWebsite>, ILeadWebsiteRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public LeadWebsiteRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateLeadWebsite(LeadWebsite leadWebsite)
        {
            leadWebsite.CreatedBy = _createdBy;
            leadWebsite.CreatedOn = DateTime.Now;
            leadWebsite.Unit = _unitname;
            var result = await Create(leadWebsite);
            return result.Id;
        }

        public async Task<string> DeleteLeadWebsite(LeadWebsite leadWebsite)
        {
            Delete(leadWebsite);
            string result = $"leadWebsite details of {leadWebsite.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<LeadWebsite>> GetAllActiveLeadWebsite(PagingParameter pagingParameter, SearchParames searchParams)
        {
            var getAllleadWebsite = FindAll()
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.FullName.Contains(searchParams.SearchValue) ||
            inv.Email.Contains(searchParams.SearchValue) || inv.PropertyType.Contains(searchParams.SearchValue))));
            return PagedList<LeadWebsite>.ToPagedList(getAllleadWebsite, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<LeadWebsite>> GetAllLeadWebsite(PagingParameter pagingParameter, SearchParames searchParams)
        {
            var getAllleadWebsite = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.FullName.Contains(searchParams.SearchValue) ||
                 inv.Email.Contains(searchParams.SearchValue) || inv.PropertyType.Contains(searchParams.SearchValue))));

            return PagedList<LeadWebsite>.ToPagedList(getAllleadWebsite, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<LeadWebsite> GetLeadWebsiteById(int id)
        {
            var leadWebsiteById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return leadWebsiteById;
        }

        public async Task<string> UpdateLeadWebsite(LeadWebsite leadWebsite)
        {

            leadWebsite.LastModifiedBy = _createdBy;
            leadWebsite.LastModifiedOn = DateTime.Now;
            Update(leadWebsite);
            string result = $"leadWebsite details of {leadWebsite.Id} is updated successfully!";
            return result;
        }
    }
}
