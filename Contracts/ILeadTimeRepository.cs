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
        Task<PagedList<LeadTime>> GetAllLeadTime(PagingParameter pagingParameter, SearchParames searchParames);
        Task<LeadTime> GetLeadTimeById(int id);
        Task<PagedList<LeadTime>> GetAllActiveLeadTime(PagingParameter pagingParameter, SearchParames searchParames);
        Task<int?> CreateLeadTime(LeadTime leadTime);
        Task<string> UpdateLeadTime(LeadTime leadTime);
        Task<string> DeleteLeadTime(LeadTime leadTime);
    }
}
