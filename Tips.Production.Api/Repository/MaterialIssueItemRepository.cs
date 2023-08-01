using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Repository
{
    public class MaterialIssueItemRepository : RepositoryBase<MaterialIssueItem>, IMaterialIssueItemRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;
        public MaterialIssueItemRepository(TipsProductionDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
        }

        public async Task<IEnumerable<MaterialIssueItem>> GetMaterialIssueItemById(int id)
        {

            var materialIssueItemDetail = await _tipsProductionDbContext.MaterialIssueItems
                                    .Where(x => x.MaterialIssueId == id)
                      
                                    .ToListAsync();

            return materialIssueItemDetail;
        }
        public async Task<List<string>> GetMaterialIssueItemProjectNumbersById(int id)
        {
            var materialIssueItemProjectNumbers = await _tipsProductionDbContext.MaterialIssueItems
                                                .Where(x => x.MaterialIssueId == id)
                                                .Select(m => m.ProjectNumber)
                                                .Distinct()
                                                .ToListAsync();

            return materialIssueItemProjectNumbers;
        }

        public async Task<string> UpdateMaterialIssueItem(MaterialIssueItem materialIssueItem)
        {
            materialIssueItem.LastModifiedBy = "Admin";
            materialIssueItem.LastModifiedOn = DateTime.Now;
            Update(materialIssueItem);
            string result = $"MaterialIssueItem of Detail {materialIssueItem.Id} is updated successfully!";
            return result;
        }
    }
}