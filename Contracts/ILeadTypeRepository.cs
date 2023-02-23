using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ILeadTypeRepository : IRepositoryBase<LeadType>
    {
        Task<IEnumerable<LeadType>> GetAllLeadTypes();
        Task<LeadType> GetLeadTypeById(int id);
        Task<IEnumerable<LeadType>> GetAllActiveLeadTypes();
        Task<int?> CreateLeadType(LeadType leadType);
        Task<string> UpdateLeadType(LeadType leadType);
        Task<string> DeleteLeadType(LeadType leadType);
    }
}
