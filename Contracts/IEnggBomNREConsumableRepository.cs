using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEnggBomNREConsumableRepository
    {
        Task<IEnumerable<NREConsumable>> GetAllEnggNREConsumable();
        Task<NREConsumable> GetEnggNREConsumableById(int id);
        Task<IEnumerable<NREConsumable>> GetAllActiveEnggNREConsumable();
        Task<int?> CreateEnggNREConsumable(NREConsumable bomNREConsumable);
        Task<string> UpdateEnggNREConsumable(NREConsumable bomNREConsumable);
        Task<string> DeleteEnggNREConsumable(NREConsumable bomNREConsumable);
    }
}
