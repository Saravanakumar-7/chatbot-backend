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
    public class SourceDetailsRepository : RepositoryBase<SourceDetails>, ISourceDetailsRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public SourceDetailsRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int?> CreateSourceDetails(SourceDetails sourceDetails)
        {
            sourceDetails.CreatedBy = _createdBy;
            sourceDetails.CreatedOn = DateTime.Now;
            sourceDetails.Unit = _unitname;
            var result = await Create(sourceDetails); return result.Id;
        }
        public async Task<string> DeleteSourceDetails(SourceDetails sourceDetails)
        {
            Delete(sourceDetails);
            string result = $"sourceDetails details of {sourceDetails.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<SourceDetails>> GetAllActiveSourceDetails()
        {
            var AllActivesourceDetails = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivesourceDetails;
        }
        public async Task<IEnumerable<SourceDetails>> GetAllSourceDetails()
        {
            var GetallSourceDetails = await FindAll().ToListAsync(); return GetallSourceDetails;
        }
        public async Task<SourceDetails> GetSourceDetailsById(int id)
        {
            var sourceDetailsbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); return sourceDetailsbyId;
        }
        public async Task<string> UpdateSourceDetails(SourceDetails sourceDetails)
        {
            sourceDetails.LastModifiedBy = _createdBy;
            sourceDetails.LastModifiedOn = DateTime.Now;
            Update(sourceDetails);
            string result = $"sourceDetails details of {sourceDetails.Id} is updated successfully!";
            return result;
        }
    }
}