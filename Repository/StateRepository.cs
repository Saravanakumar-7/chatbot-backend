using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository
{
    public class StateRepository : RepositoryBase<State>, IStateRepository
    {
        public StateRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        { }
        public async Task<int?> CreateState(State state)
        {
            state.CreatedBy = "Admin";
            state.CreatedOn = DateTime.Now;
            state.Unit = "Bangalore";
            var result = await Create(state); return result.Id;
        }
        public async Task<string> DeleteState(State state)
        {
            Delete(state);
            string result = $"state details of {state.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<State>> GetAllActiveStates()
        {
            var AllActivestates = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivestates;
        }
        public async Task<IEnumerable<State>> GetAllStates()
        {
            var GetallStates = await FindAll().ToListAsync(); return GetallStates;
        }
        public async Task<State> GetStateById(int id)
        {
            var statebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); return statebyId;
        }
        public async Task<string> UpdateState(State state)
        {
            state.LastModifiedBy = "Admin";
            state.LastModifiedOn = DateTime.Now;
            Update(state);
            string result = $"state details of {state.Id} is updated successfully!";
            return result;
        }
    }
}