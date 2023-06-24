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
    public class SourceRepository : RepositoryBase<Source>, ISourceRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public SourceRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateSource(Source source)
        {
            source.CreatedBy = _createdBy;
            source.CreatedOn = DateTime.Now;
            source.Unit = _unitname;
            var result = await Create(source);
         
            return result.Id;

        }

        public async Task<string> DeleteSource(Source source)
        {
            Delete(source);
            string result = $"source details of {source.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Source>> GetAllActiveSources()
        {

            var AllActiveSecondarySources = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveSecondarySources;
        }

        public async Task<IEnumerable<Source>> GetAllSources()
        {

            var GetallSources = await FindAll().ToListAsync();

            return GetallSources;
        }

        public async Task<Source> GetSourceById(int id)
        {
            var SourcesbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return SourcesbyId;
        }
        public async Task<string> UpdateSource(Source source)
        {

            source.LastModifiedBy = _createdBy;
            source.LastModifiedOn = DateTime.Now;
            Update(source);
            string result = $"Source details of {source.Id} is updated successfully!";
            return result;
        }
    }
}
