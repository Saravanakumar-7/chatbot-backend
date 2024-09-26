using Contracts;
using Entities;
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
    public class PreferredFreightForwarderRepository : RepositoryBase<PreferredFreightForwarder>, IPreferredFreightForwarderRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public PreferredFreightForwarderRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreatePreferredFreightForwarder(PreferredFreightForwarder preferredFreightForwarder)
        {
            preferredFreightForwarder.CreatedBy = _createdBy;
            preferredFreightForwarder.CreatedOn = DateTime.Now;
            preferredFreightForwarder.Unit = _unitname;
            var result = await Create(preferredFreightForwarder);
            
            return result.Id;
        }

        public async Task<string> DeletePreferredFreightForwarder(PreferredFreightForwarder preferredFreightForwarder)
        {
            Delete(preferredFreightForwarder);
            string result = $"preferredFreightForwarder details of {preferredFreightForwarder.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<PreferredFreightForwarder>> GetAllActivePreferredFreightForwarders([FromQuery] SearchParames searchParams)
        {
            var preferredFreightForwarderDetails = FindAll()
                                          .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.PreferredFreightforwarder.Contains(searchParams.SearchValue) ||
                                    inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return preferredFreightForwarderDetails;
        }

        public async Task<IEnumerable<PreferredFreightForwarder>> GetAllPreferredFreightForwarders([FromQuery] SearchParames searchParams)
        {
            var preferredFreightForwarderDetails = FindAll().OrderByDescending(x => x.Id)
                                           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.PreferredFreightforwarder.Contains(searchParams.SearchValue) ||
                                     inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return preferredFreightForwarderDetails;
        }

        public async Task<PreferredFreightForwarder> GetPreferredFreightForwarderById(int id)
        {
            var PreferredFreightForwarderbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return PreferredFreightForwarderbyId;
        }

        public async Task<string> UpdatePreferredFreightForwarder(PreferredFreightForwarder preferredFreightForwarder)
        {
            preferredFreightForwarder.LastModifiedBy = _createdBy;
            preferredFreightForwarder.LastModifiedOn = DateTime.Now;
            Update(preferredFreightForwarder);
            string result = $"PreferredFreightForwarder details of {preferredFreightForwarder.Id} is updated successfully!";
            return result;
        }
    }
}
