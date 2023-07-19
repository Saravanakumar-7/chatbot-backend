using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IPreferredFreightForwarderRepository : IRepositoryBase<PreferredFreightForwarder>
    {
        Task<IEnumerable<PreferredFreightForwarder>> GetAllPreferredFreightForwarders(SearchParames searchParams);
        Task<PreferredFreightForwarder> GetPreferredFreightForwarderById(int id);
        Task<IEnumerable<PreferredFreightForwarder>> GetAllActivePreferredFreightForwarders(SearchParames searchParams);
        Task<int?> CreatePreferredFreightForwarder(PreferredFreightForwarder preferredFreightForwarder);
        Task<string> UpdatePreferredFreightForwarder(PreferredFreightForwarder preferredFreightForwarder);
        Task<string> DeletePreferredFreightForwarder(PreferredFreightForwarder preferredFreightForwarder);
    }
}
