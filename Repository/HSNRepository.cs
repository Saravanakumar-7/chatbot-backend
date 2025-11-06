using Contracts;
using Entities;
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
    public class HSNRepository : RepositoryBase<Hsn>, IHSNRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public HSNRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int?> CreateHSN(Hsn hsn)
        {
            hsn.CreatedBy = _createdBy;
            hsn.CreatedOn = DateTime.Now;
            hsn.Unit = _unitname;
            var result = await Create(hsn);

            return result.Id;
        }

        public async Task<string> DeleteHSN(Hsn hsn)
        {
            Delete(hsn);
            string result = $"Hsn details of {hsn.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Hsn>> GetAllActiveHSN([FromQuery] SearchParames searchParams)
        {
            var HsnDetails = FindAll()
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.hsn.Contains(searchParams.SearchValue))) && inv.IsActive == true);
            return HsnDetails;
        }
        public async Task<IEnumerable<Hsn>> GetAllHSN([FromQuery] SearchParames searchParams)
        {
            var HsnDetails = FindAll()
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.hsn.Contains(searchParams.SearchValue))));
            return HsnDetails;
        }

        public async Task<Hsn> GetHSNById(int id)
        {
            var HsnbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return HsnbyId;
        }

        public async Task<string> UpdateHSN(Hsn hsn)
        {
            hsn.LastModifiedBy = _createdBy;
            hsn.LastModifiedOn = DateTime.Now;
            Update(hsn);
            string result = $"Hsn details of {hsn.Id} is updated successfully!";
            return result;
        }
    }

}
