using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal class OtherChargesRepository : RepositoryBase<OtherCharges>, IOtherChargesRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public OtherChargesRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateOtherCharges(OtherCharges otherCharges)
        {
            otherCharges.CreatedBy = _createdBy;
            otherCharges.CreatedOn = DateTime.Now;
            otherCharges.Unit = _unitname;
            var result = await Create(otherCharges);

            return result.Id;
        }

        public async Task<string> DeleteOtherCharges(OtherCharges otherCharges)
        {
            Delete(otherCharges);
            string result = $"OtherCharges details of {otherCharges.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<OtherCharges>> GetAllActiveOtherCharges()
        {
            var otherChargesActiveDetails = await FindByCondition(x => x.ActiveStatus == true).ToListAsync();
            return otherChargesActiveDetails;
        }

        public async Task<IEnumerable<OtherCharges>> GetAllOtherCharges()
        {
            var otherChargesDetails = await FindAll().OrderByDescending(x => x.Id).ToListAsync();
            return otherChargesDetails;
        }

        public async Task<OtherCharges> GetOtherChargesById(int id)
        {
            var otherChargesbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return otherChargesbyId;
        }

        public async Task<string> UpdateOtherCharges(OtherCharges otherCharges)
        {
            otherCharges.LastModifiedBy = _createdBy;
            otherCharges.LastModifiedOn = DateTime.Now;
            Update(otherCharges);
            string result = $"OtherCharges details of {otherCharges.Id} is updated successfully!";
            return result;
        }
    }
}
