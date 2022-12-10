using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ISecondarySourceRepository : IRepositoryBase<SecondarySource>
    {
        Task<IEnumerable<SecondarySource>> GetAllSecondarySources();
        Task<SecondarySource> GetSecondarySourceById(int id);
        Task<IEnumerable<SecondarySource>> GetAllActiveSecondarySources();
        Task<int?> CreateSecondarySource(SecondarySource secondarySource);
        Task<string> UpdateSecondarySource(SecondarySource secondarySource);
        Task<string> DeleteSecondarySource(SecondarySource secondarySource);
    }
}
