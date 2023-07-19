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
        Task<IEnumerable<ScopeOfSupply>> GetAllScopeOfSupply(SearchParames searchParams);
        Task<ScopeOfSupply> GetScopeOfSupplyById(int id);
        Task<IEnumerable<ScopeOfSupply>> GetAllActiveScopeOfSupply(SearchParames searchParams);
        Task<int?> CreateScopeOfSupply(ScopeOfSupply scopeOfSupply);
        Task<string> UpdateScopeOfSupply(ScopeOfSupply scopeOfSupply);
        Task<string> DeleteScopeOfSupply(ScopeOfSupply scopeOfSupply);
}
}
