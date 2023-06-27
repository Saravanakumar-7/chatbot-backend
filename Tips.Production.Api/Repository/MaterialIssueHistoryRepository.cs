using System.Linq.Expressions;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Repository
{
    public class MaterialIssueHistoryRepository : RepositoryBase<MaterialIssueHistory>, IMaterialIssueHistoryRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;
        public MaterialIssueHistoryRepository(TipsProductionDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
        }

        public async Task<int> CreateMaterialIssueHistory(MaterialIssueHistory materialIssueHistory)
        {
            materialIssueHistory.CreatedBy = "Admin";
            materialIssueHistory.CreatedOn = DateTime.Now;
            materialIssueHistory.Unit = "Bangalore";
            var result = await Create(materialIssueHistory);
            return result.Id;
        }
    }
}
