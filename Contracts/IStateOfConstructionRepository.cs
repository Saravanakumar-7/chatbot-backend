using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public interface IStateOfConstructionRepository : IRepositoryBase<StateOfConstruction>
    {
        Task<IEnumerable<StateOfConstruction>> GetAllStateOfConstructions();
        Task<StateOfConstruction> GetStateOfConstructionById(int id);
        Task<IEnumerable<StateOfConstruction>> GetAllActiveStateOfConstruction();
        Task<int?> CreateStateOfConstruction(StateOfConstruction stateOfConstruction);
        Task<string> UpdateStateOfConstruction(StateOfConstruction stateOfConstruction);
        Task<string> DeleteStateOfConstruction(StateOfConstruction stateOfConstruction);
    }
}