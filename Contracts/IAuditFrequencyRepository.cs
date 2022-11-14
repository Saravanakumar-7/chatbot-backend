using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IAuditFrequencyRepository : IRepositoryBase<AuditFrequency>
    {
        Task<IEnumerable<AuditFrequency>> GetAllAuditFrequencies();
        Task<AuditFrequency> GetAuditFrequenyById(int id);
        Task<IEnumerable<AuditFrequency>> GetAllActiveAuditFrequencies();
        Task<int?> CreateAuditFrequency(AuditFrequency customerType);
        Task<string> UpdateAuditFrequency(AuditFrequency customerType);
        Task<string> DeleteAuditFrequency(AuditFrequency customerType);
    }
}
