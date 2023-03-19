using Entities.Helper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ILeadRepository : IRepositoryBase<Lead>
    {
        Task<PagedList<Lead>> GetAllLeads(PagingParameter pagingParameter, SearchParames searchParames);
        Task<Lead> GetLeadById(int id);
        Task<int?> GetLeadIDIncrementCount(DateTime date);
        Task<Lead> CreateLead(Lead lead);
        Task<string> UpdateLead(Lead lead);
        Task<string> DeleteLead(Lead lead);
    }
}
