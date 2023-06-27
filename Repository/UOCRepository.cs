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
    public class UOCRepository : RepositoryBase<UOC>, IUOCRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public UOCRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateUOC(UOC uoc)
        {
            uoc.CreatedBy = _createdBy;
            uoc.CreatedOn = DateTime.Now;
            uoc.Unit = _unitname;
            var result = await Create(uoc);
            
            return result.Id;
        }

        public async Task<string> DeleteUOC(UOC uoc)
        {
            Delete(uoc);
            string result = $"UOC details of {uoc.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<UOC>> GetAllActiveUOC()
        {
            var AllActiveUoc = await FindByCondition(x => x.ActiveStatus == true).ToListAsync();
            return AllActiveUoc;
        }

        public async Task<IEnumerable<UOC>> GetAllUOC()
        {
            var GetallUoc = await FindAll().ToListAsync();
            return GetallUoc;
        }

        public async Task<UOC> GetUOCById(int id)
        {
            var UocbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return UocbyId;
        }

        public async Task<string> UpdateUOC(UOC uoc)
        {
            uoc.LastModifiedBy = _createdBy;
            uoc.LastModifiedOn = DateTime.Now;
            Update(uoc);
            string result = $"UOC details of {uoc.Id} is updated successfully!";
            return result;
        }
    }
}
