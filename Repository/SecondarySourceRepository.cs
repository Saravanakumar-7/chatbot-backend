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
    public class SecondarySourceRepository : RepositoryBase<SecondarySource>, ISecondarySourceRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public SecondarySourceRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateSecondarySource(SecondarySource secondarySource)
        {
            secondarySource.CreatedBy = _createdBy;
            secondarySource.CreatedOn = DateTime.Now;
            secondarySource.Unit = _unitname;
            var result = await Create(secondarySource);
            
            return result.Id;

        }

        public async Task<string> DeleteSecondarySource(SecondarySource secondarySource)
        {
            Delete(secondarySource);
            string result = $"secondarySource details of {secondarySource.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<SecondarySource>> GetAllActiveSecondarySources()
        {

            var AllActiveSecondarySources = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveSecondarySources;
        }

        public async Task<IEnumerable<SecondarySource>> GetAllSecondarySources()
        {

            var GetallSecondarySourcesList = await FindAll().ToListAsync();

            return GetallSecondarySourcesList;
        }

        public async Task<SecondarySource> GetSecondarySourceById(int id)
        {
            var SecondarySourcesbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return SecondarySourcesbyId;
        }
        public async Task<string> UpdateSecondarySource(SecondarySource secondarySource)
        {

            secondarySource.LastModifiedBy = _createdBy;
            secondarySource.LastModifiedOn = DateTime.Now;
            Update(secondarySource);
            string result = $"secondarySource details of {secondarySource.Id} is updated successfully!";
            return result;
        }
    }
}
