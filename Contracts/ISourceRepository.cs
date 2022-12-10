using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ISourceRepository : IRepositoryBase<Source>
    {
        Task<IEnumerable<Source>> GetAllSources();
        Task<Source> GetSourceById(int id);
        Task<IEnumerable<Source>> GetAllActiveSources();
        Task<int?> CreateSource(Source source);
        Task<string> UpdateSource(Source source);
        Task<string> DeleteSource(Source source);
    }
}
