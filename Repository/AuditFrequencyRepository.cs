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
    public class AuditFrequencyRepository : RepositoryBase<AuditFrequency>, IAuditFrequencyRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public AuditFrequencyRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateAuditFrequency(AuditFrequency auditFrequency)
        {
            auditFrequency.CreatedBy = _createdBy;
            auditFrequency.CreatedOn = DateTime.Now;
            auditFrequency.Unit = _unitname;
            var result = await Create(auditFrequency);
            
            return result.Id;
            
        }

        public async Task<string> DeleteAuditFrequency(AuditFrequency auditFrequency)
        {
            Delete(auditFrequency);
            string result = $"AuditFrequency details of {auditFrequency.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<AuditFrequency>> GetAllActiveAuditFrequencies()
        {

            var auditFrequencyDetails = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return auditFrequencyDetails;
        }

        public async Task<IEnumerable<AuditFrequency>> GetAllAuditFrequencies()
        {

            var auditFrequencyDetails = await FindAll().ToListAsync();
            return auditFrequencyDetails;
        }

        public async Task<AuditFrequency> GetAuditFrequenyById(int id)
        {
            var AuditFrequencyyid = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return AuditFrequencyyid;
        }       
        public async Task<string> UpdateAuditFrequency(AuditFrequency auditFrequency)
        {

            auditFrequency.LastModifiedBy = _createdBy;
            auditFrequency.LastModifiedOn = DateTime.Now;
            Update(auditFrequency);
            string result = $"AuditFrequency details of {auditFrequency.Id} is updated successfully!";
            return result;
        }
    }
}
