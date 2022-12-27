using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEnggBomAlternatesRepository
    {
        Task<IEnumerable<EnggAlternates>> GetAllEnggAlternate();
        Task<EnggAlternates> GetEnggAlternateById(int id);
        Task<IEnumerable<EnggAlternates>> GetAllActiveEnggAlternate();
        Task<int?> CreateEnggAlternate(EnggAlternates enggAlternates);
        Task<string> UpdateEnggAlternate(EnggAlternates enggAlternates);
        Task<string> DeleteEnggAlternate(EnggAlternates enggAlternates);
    }
}
