using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ILeadStatusRepository : IRepositoryBase<LeadStatus>
    {
        Task<IEnumerable<LeadStatus>> GetAllLeadStatus();
        Task<LeadStatus> GetLeadStatusById(int id);
        Task<IEnumerable<LeadStatus>> GetAllActiveLeadStatus();
        Task<int?> CreateLeadStatus(LeadStatus leadStatus);
        Task<string> UpdateLeadStatus(LeadStatus leadStatus);
        Task<string> DeleteLeadStatus(LeadStatus leadStatus);
    }
}
