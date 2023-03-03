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
    public class StateOfConstructionRepository : RepositoryBase<StateOfConstruction>, IStateOfConstructionRepository
    {
        public StateOfConstructionRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        { }
        public async Task<int?> CreateStateOfConstruction(StateOfConstruction stateOfConstruction)
        {
            stateOfConstruction.CreatedBy = "Admin";
            stateOfConstruction.CreatedOn = DateTime.Now;
            stateOfConstruction.Unit = "Bangalore";
            var result = await Create(stateOfConstruction); return result.Id;
        }
        public async Task<string> DeleteStateOfConstruction(StateOfConstruction stateOfConstruction)
        {
            Delete(stateOfConstruction);
            string result = $"stateOfConstruction details of {stateOfConstruction.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<StateOfConstruction>> GetAllActiveStateOfConstruction()
        {
            var AllActiveStateOfConstruction = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveStateOfConstruction;
        }
        public async Task<IEnumerable<StateOfConstruction>> GetAllStateOfConstructions()
        {
            var GetallStateOfConstruction = await FindAll().ToListAsync(); return GetallStateOfConstruction;
        }
        public async Task<StateOfConstruction> GetStateOfConstructionById(int id)
        {
            var stateOfConstructionbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); return stateOfConstructionbyId;
        }
        public async Task<string> UpdateStateOfConstruction(StateOfConstruction stateOfConstruction)
        {
            stateOfConstruction.LastModifiedBy = "Admin";
            stateOfConstruction.LastModifiedOn = DateTime.Now;
            Update(stateOfConstruction);
            string result = $"stateOfConstruction details of {stateOfConstruction.Id} is updated successfully!";
            return result;
        }
    }
}