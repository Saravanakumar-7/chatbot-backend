using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ILeadTimeRepository : IRepositoryBase<LeadTime>
    {
        Task<IEnumerable<LeadTime>> GetAllLeadTime(SearchParames searchParams);
        Task<LeadTime> GetLeadTimeById(int id);
        Task<IEnumerable<LeadTime>> GetAllActiveLeadTime(SearchParames searchParams);
        Task<int?> CreateLeadTime(LeadTime leadTime);
        Task<string> UpdateLeadTime(LeadTime leadTime);
        Task<string> DeleteLeadTime(LeadTime leadTime);
    }
}
