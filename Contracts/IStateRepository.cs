using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public interface IStateRepository : IRepositoryBase<State>
    {
        Task<IEnumerable<State>> GetAllStates();
        Task<State> GetStateById(int id);
        Task<IEnumerable<State>> GetAllActiveStates();
        Task<int?> CreateState(State state);
        Task<string> UpdateState(State state);
        Task<string> DeleteState(State state);
    }
}