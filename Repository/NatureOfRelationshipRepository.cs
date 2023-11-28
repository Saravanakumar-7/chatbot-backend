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
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class NatureOfRelationshipRepository : RepositoryBase<NatureOfRelationship>, INatureOfRelationshipRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public NatureOfRelationshipRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateNatureOfRelationship(NatureOfRelationship natureOfRelationship)
        {
            natureOfRelationship.CreatedBy = _createdBy;
            natureOfRelationship.CreatedOn = DateTime.Now;
            natureOfRelationship.Unit = _unitname;
            var result = await Create(natureOfRelationship);
            
            return result.Id;
        }

        public async Task<string> DeleteNatureOfRelationship(NatureOfRelationship natureOfRelationship)
        {
            Delete(natureOfRelationship);
            string result = $"NatureOfRelationship details of {natureOfRelationship.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<NatureOfRelationship>> GetAllActiveNatureOfRelationships([FromQuery] SearchParames searchParams)
        {
            var natureOfRelationshipDetails = FindAll()
                                   .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.NatureOfRelationshipName.Contains(searchParams.SearchValue) ||
                                  inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return natureOfRelationshipDetails;
        }

        public async Task<IEnumerable<NatureOfRelationship>> GetAllNatureOfRelationships([FromQuery] SearchParames searchParams)
        {
            var natureOfRelationshipDetails = FindAll().OrderByDescending(x => x.Id)
                                   .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.NatureOfRelationshipName.Contains(searchParams.SearchValue) ||
                                  inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return natureOfRelationshipDetails;
        }

        public async Task<NatureOfRelationship> GetNatureOfRelationshipById(int id)
        {

            var NatureOfRelationshipbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return NatureOfRelationshipbyId;
        }

        public async Task<string> UpdateNatureOfRelationship(NatureOfRelationship natureOfRelationship)
        {
            natureOfRelationship.LastModifiedBy = _createdBy;
            natureOfRelationship.LastModifiedOn = DateTime.Now;
            Update(natureOfRelationship);
            string result = $"Customer Type details of {natureOfRelationship.Id} is updated successfully!";
            return result;
        }
    }
}
