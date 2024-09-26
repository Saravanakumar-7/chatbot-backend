using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace Repository
{
    public class SFTRepository : RepositoryBase<SFT>, ISFTRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public SFTRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int?> CreateSFT(SFT sFt)
        {
            sFt.CreatedBy = _createdBy;
            sFt.CreatedOn = DateTime.Now;
            sFt.Unit = _unitname;
            var result = await Create(sFt); return result.Id;
        }
        public async Task<string> DeleteSFT(SFT sFt)
        {
            Delete(sFt);
            string result = $"SFt details of {sFt.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<SFT>> GetAllActiveSFT()
        {
            var AllActiveSFT = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveSFT;
        }
        public async Task<IEnumerable<SFT>> GetAllSFT()
        {
            var GetallSFT = await FindAll().ToListAsync(); return GetallSFT;
        }
        public async Task<SFT> GetSFTById(int id)
        {
            var SFTbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); return SFTbyId;
        }
        public async Task<string> UpdateSFT(SFT sFt)
        {
            sFt.LastModifiedBy = _createdBy;
            sFt.LastModifiedOn = DateTime.Now;
            Update(sFt);
            string result = $"SFT details of {sFt.Id} is updated successfully!";
            return result;
        }
    }
}