using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace Repository
{
    public class StageOfConstructionRepository : RepositoryBase<StageOfConstruction>, IStageOfConstructionRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public StageOfConstructionRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int?> CreateStageOfConstruction(StageOfConstruction stageOfConstruction)
        {
            stageOfConstruction.CreatedBy = _createdBy;
            stageOfConstruction.CreatedOn = DateTime.Now;
            stageOfConstruction.Unit = _unitname;
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
            stageOfConstruction.LastModifiedBy = _createdBy;
            stageOfConstruction.LastModifiedOn = DateTime.Now;
            Update(stageOfConstruction);
            string result = $"StageOfConstruction details of {stageOfConstruction.Id} is updated successfully!";
            return result;
        }
    }
}