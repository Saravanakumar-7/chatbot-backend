using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ILeadTypeRepository : IRepositoryBase<LeadType>
    {
        Task<PagedList<LeadType>> GetAllLeadTypes(PagingParameter pagingParameter, SearchParames searchParames);
        Task<LeadType> GetLeadTypeById(int id);
        Task<PagedList<LeadType>> GetAllActiveLeadTypes(PagingParameter pagingParameter, SearchParames searchParames);
        Task<int?> CreateLeadType(LeadType leadType);
        Task<string> UpdateLeadType(LeadType leadType);
        Task<string> DeleteLeadType(LeadType leadType);
    }
}
