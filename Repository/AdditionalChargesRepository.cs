using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class AdditionalChargesRepository : RepositoryBase<AdditionalCharges>, IAdditionalChargesRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public AdditionalChargesRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateAdditionalCharges(AdditionalCharges additionalCharges)
        {
            additionalCharges.CreatedBy = _createdBy;
            additionalCharges.CreatedOn = DateTime.Now;
            additionalCharges.Unit = _unitname;
            var result = await Create(additionalCharges);

            return result.Id;
        }

        public async Task<string> DeleteAdditionalCharges(AdditionalCharges additionalCharges)
        {
            Delete(additionalCharges);
            string result = $"AdditionalCharges details of {additionalCharges.Id} is deleted successfully!";
            return result;
        }

        public async Task<AdditionalCharges> GetAdditionalChargesById(int id)
        {
            var additionalChargesbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return additionalChargesbyId;
        }

        public async Task<IEnumerable<AdditionalCharges>> GetAllActiveAdditionalCharges([FromQuery] SearchParames searchParams)
        {
            var additionalChargesActiveDetails = FindAll()
                               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.AdditionalChargesLabelName.Contains(searchParams.SearchValue) ||
                                      inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))) && inv.ActiveStatus == true);
            return additionalChargesActiveDetails;
        }

        public async Task<IEnumerable<AdditionalCharges>> GetAllAdditionalCharges([FromQuery] SearchParames searchParams)
        {
            var additionalChargesActiveDetails = FindAll().OrderByDescending(x=>x.Id)
                               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.AdditionalChargesLabelName.Contains(searchParams.SearchValue) ||
                                      inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return additionalChargesActiveDetails;
        }

        public async Task<string> UpdateAdditionalCharges(AdditionalCharges additionalCharges)
        {
            additionalCharges.LastModifiedBy = _createdBy;
            additionalCharges.LastModifiedOn = DateTime.Now;
            Update(additionalCharges);
            string result = $"AdditionalCharges details of {additionalCharges.Id} is updated successfully!";
            return result;
        }
    }
}
