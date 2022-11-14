using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ISalutationsRepository : IRepositoryBase<Salutations>
    {
        Task<IEnumerable<Salutations>> GetAllSalutations();
        Task<Salutations> GetSalutationsById(int id);
        Task<IEnumerable<Salutations>> GetAllActiveSalutations();
        Task<int?> CreateSalutations(Salutations salutations);
        Task<string> UpdateSalutations(Salutations salutations);
        Task<string> DeleteSalutations(Salutations salutations);
    }
}
