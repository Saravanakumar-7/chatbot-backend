using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class PrioritizeRepository : RepositoryBase<Prioritize>, IPrioritizeRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;

        public PrioritizeRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreatePrioritize(Prioritize prioritize)
        {
            prioritize.CreatedBy = _createdBy;
            prioritize.CreatedOn = DateTime.Now;
            prioritize.Unit = _unitname;
            var result = await Create(prioritize);

            return result.Id;
        }

        public async Task<string> DeletePrioritize(Prioritize prioritize)
        {
            Delete(prioritize);
            string result = $"Prioritize details of {prioritize.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<Prioritize>> GetAllPrioritize(PagingParameter pagingParameter, SearchParames searchParams)
        {

            var prioritizeDetails =FindAll().OrderByDescending(x => x.Id)
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || 
               inv.PrioritizeName.Contains(searchParams.SearchValue) ||
               inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<Prioritize>.ToPagedList(prioritizeDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<Prioritize> GetPrioritizeById(int id)
        {
            var prioritizebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return prioritizebyId;
        }
         
        public async Task<string> UpdatePrioritize(Prioritize prioritize)
        {
            prioritize.LastModifiedBy = _createdBy;
            prioritize.LastModifiedOn = DateTime.Now;
            Update(prioritize);
            string result = $"Prioritize details of {prioritize.Id} is updated successfully!";
            return result;
        }
    }
}
