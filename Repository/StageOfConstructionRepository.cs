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
    public class StageOfConstructionRepository : RepositoryBase<StageOfConstruction>, IStageOfConstructionRepository
    {
        public StageOfConstructionRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        { }
        public async Task<int?> CreateStageOfConstruction(StageOfConstruction stageOfConstruction)
        {
            stageOfConstruction.CreatedBy = "Admin";
            stageOfConstruction.CreatedOn = DateTime.Now;
            stageOfConstruction.Unit = "Bangalore";
            var result = await Create(stageOfConstruction);
            return result.Id;
        }
        public async Task<string> DeleteStageOfConstruction(StageOfConstruction stageOfConstruction)
        {
            Delete(stageOfConstruction);
            string result = $"StageOfConstruction details of {stageOfConstruction.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<StageOfConstruction>> GetAllActiveStageOfConstruction()
        {
            var AllActiveStageOfConstruction = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveStageOfConstruction;
        }
        public async Task<IEnumerable<StageOfConstruction>> GetAllStageOfConstructions()
        {
            var GetallStageOfConstruction = await FindAll().ToListAsync(); return GetallStageOfConstruction;
        }
        public async Task<StageOfConstruction> GetStageOfConstructionById(int id)
        {
            var StageOfConstructionbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return StageOfConstructionbyId;
        }
        public async Task<string> UpdateStageOfConstruction(StageOfConstruction stageOfConstruction)
        {
            stageOfConstruction.LastModifiedBy = "Admin";
            stageOfConstruction.LastModifiedOn = DateTime.Now;
            Update(stageOfConstruction);
            string result = $"StageOfConstruction details of {stageOfConstruction.Id} is updated successfully!";
            return result;
        }
    }
}