using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class LeadTypeRepository : RepositoryBase<LeadType>, ILeadTypeRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public LeadTypeRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateLeadType(LeadType leadType)
        {
            leadType.CreatedBy = _createdBy;
            leadType.CreatedOn = DateTime.Now;
            leadType.Unit = _unitname;
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

            leadType.LastModifiedBy = _createdBy;
            leadType.LastModifiedOn = DateTime.Now;
            Update(leadType);
            string result = $"leadStatus details of {leadType.Id} is updated successfully!";
            return result;
        }
    }
}
