using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IScopeOfSupplyRepository : IRepositoryBase<ScopeOfSupply>
    {
        Task<IEnumerable<ScopeOfSupply>> GetAllScopeOfSupply();
        Task<ScopeOfSupply> GetScopeOfSupplyById(int id);
        Task<IEnumerable<ScopeOfSupply>> GetAllActiveScopeOfSupply();
        Task<int?> CreateScopeOfSupply(ScopeOfSupply scopeOfSupply);
        Task<string> UpdateScopeOfSupply(ScopeOfSupply scopeOfSupply);
        Task<string> DeleteScopeOfSupply(ScopeOfSupply scopeOfSupply);
}
}
