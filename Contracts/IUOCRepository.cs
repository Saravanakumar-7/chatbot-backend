using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IUOCRepository : IRepositoryBase<UOC>
    {
        Task<IEnumerable<UOC>> GetAllUOC();
        Task<UOC> GetUOCById(int id);
        Task<IEnumerable<UOC>> GetAllActiveUOC();
        Task<int?> CreateUOC(UOC uoc);
        Task<string> UpdateUOC(UOC uoc);
        Task<string> DeleteUOC(UOC uoc);
    }
}
