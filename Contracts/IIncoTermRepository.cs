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
        Task<IEnumerable<IncoTerm>> GetAllIncoTerm();
        Task<IncoTerm> GetIncoTermById(int id);
        Task<IEnumerable<IncoTerm>> GetAllActiveIncoTerm();
        Task<int?> CreateIncoTerm(IncoTerm incoTerm);
        Task<string> UpdateIncoTerm(IncoTerm incoTerm);
        Task<string> DeleteIncoTerm(IncoTerm incoTerm);
    }
}
