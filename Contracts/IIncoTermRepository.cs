using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IIncoTermRepository : IRepositoryBase<IncoTerm>
    {
        Task<PagedList<IncoTerm>> GetAllIncoTerm(PagingParameter pagingParameter, SearchParames searchParams);
        Task<IncoTerm> GetIncoTermById(int id);
        Task<PagedList<IncoTerm>> GetAllActiveIncoTerm(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateIncoTerm(IncoTerm incoTerm);
        Task<string> UpdateIncoTerm(IncoTerm incoTerm);
        Task<string> DeleteIncoTerm(IncoTerm incoTerm);
    }
}
