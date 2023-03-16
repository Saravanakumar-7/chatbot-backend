using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IAuditFrequencyRepository : IRepositoryBase<AuditFrequency>
    {
        Task<PagedList<AuditFrequency>> GetAllAuditFrequencies(PagingParameter pagingParameter, SearchParames searchParams);


        Task<AuditFrequency> GetAuditFrequenyById(int id);
        Task<PagedList<AuditFrequency>> GetAllActiveAuditFrequencies(PagingParameter pagingParameter, SearchParames searchParams);

        Task<int?> CreateAuditFrequency(AuditFrequency auditFrequency);
        Task<string> UpdateAuditFrequency(AuditFrequency auditFrequency);
        Task<string> DeleteAuditFrequency(AuditFrequency auditFrequency);
    }
}
