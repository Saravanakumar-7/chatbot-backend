using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public interface IStageOfConstructionRepository : IRepositoryBase<StageOfConstruction>
    {
        Task<IEnumerable<StageOfConstruction>> GetAllStageOfConstructions();
        Task<StageOfConstruction> GetStageOfConstructionById(int id);
        Task<IEnumerable<StageOfConstruction>> GetAllActiveStageOfConstruction();
        Task<int?> CreateStageOfConstruction(StageOfConstruction stageOfConstruction);
        Task<string> UpdateStageOfConstruction(StageOfConstruction stageOfConstruction);
        Task<string> DeleteStageOfConstruction(StageOfConstruction stageOfConstruction);
    }
}