using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ILeadStatusRepository : IRepositoryBase<LeadStatus>
    {
        Task<PagedList<LeadStatus>> GetAllLeadStatus(PagingParameter pagingParameter, SearchParames searchParames);
        Task<LeadStatus> GetLeadStatusById(int id);
        Task<PagedList<LeadStatus>> GetAllActiveLeadStatus(PagingParameter pagingParameter, SearchParames searchParames);
        Task<int?> CreateLeadStatus(LeadStatus leadStatus);
        Task<string> UpdateLeadStatus(LeadStatus leadStatus);
        Task<string> DeleteLeadStatus(LeadStatus leadStatus);
    }
}
