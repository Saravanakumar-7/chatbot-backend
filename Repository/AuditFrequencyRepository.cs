using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class AuditFrequencyRepository : RepositoryBase<AuditFrequency>, IAuditFrequencyRepository
    {
        public AuditFrequencyRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateAuditFrequency(AuditFrequency auditFrequency)
        {
            auditFrequency.CreatedBy = "Admin";
            auditFrequency.CreatedOn = DateTime.Now;
            auditFrequency.Unit = "Bangalore";
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

            var AllActiveAuditFrequencies = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveAuditFrequencies;
        }

        public async Task<IEnumerable<AuditFrequency>> GetAllAuditFrequencies()
        {

            var GetallAuditFrequencies = await FindAll().ToListAsync();

            return GetallAuditFrequencies;
        }

        public async Task<AuditFrequency> GetAuditFrequenyById(int id)
        {
            var AuditFrequencyyid = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return AuditFrequencyyid;
        }       
        public async Task<string> UpdateAuditFrequency(AuditFrequency auditFrequency)
        {

            auditFrequency.LastModifiedBy = "Admin";
            auditFrequency.LastModifiedOn = DateTime.Now;
            Update(auditFrequency);
            string result = $"AuditFrequency details of {auditFrequency.Id} is updated successfully!";
            return result;
        }
    }
}
