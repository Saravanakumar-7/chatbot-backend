using Contracts;
using Entities;
using Entities.Helper;
using Entities.Migrations;
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
    public class IncoTermRepository : RepositoryBase<IncoTerm>, IIncoTermRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public IncoTermRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateIncoTerm(IncoTerm incoTerm)
        {
            incoTerm.CreatedBy = _createdBy;
            incoTerm.CreatedOn = DateTime.Now;
            incoTerm.Unit = _unitname;
            var result = await Create(incoTerm);
           
            return result.Id;
        }

        public async Task<string> DeleteIncoTerm(IncoTerm incoTerm)
        {
            Delete(incoTerm);
            string result = $"Inco Terms details of {incoTerm.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<IncoTerm>> GetAllActiveIncoTerm([FromQuery] SearchParames searchParams)
        {
            var incoTermDetails = FindAll()
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.IncoTermName.Contains(searchParams.SearchValue) ||
          inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return incoTermDetails;
        }

        public async Task<IEnumerable<IncoTerm>> GetAllIncoTerm([FromQuery] SearchParames searchParams)
        {
            var incoTermDetails = FindAll().OrderByDescending(x => x.Id)
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.IncoTermName.Contains(searchParams.SearchValue) ||
          inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return incoTermDetails;
        }


        public async Task<IncoTerm> GetIncoTermById(int id)
        {
            var IncoTermbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return IncoTermbyId;
        }

        public async Task<string> UpdateIncoTerm(IncoTerm incoTerm)
        {
            incoTerm.LastModifiedBy = _createdBy;
            incoTerm.LastModifiedOn = DateTime.Now;
            Update(incoTerm);
            string result = $"Inco Term of Detail {incoTerm.Id} is updated successfully!";
            return result;
        }
    }
}
