using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Repository
{
    public class MaterialIssueItemRepository : RepositoryBase<MaterialIssueItem>, IMaterialIssueItemRepository
    {
        private AdvitaTipsProductionDbContext _advitaTipsProductionDbContext;
        private TipsProductionDbContext _tipsProductionDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public MaterialIssueItemRepository(TipsProductionDbContext repositoryContext, AdvitaTipsProductionDbContext advitaTipsProductionDbContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext,advitaTipsProductionDbContext)
        {
            _advitaTipsProductionDbContext = advitaTipsProductionDbContext;
            _tipsProductionDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
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
            materialIssueItem.LastModifiedBy = _createdBy;
            materialIssueItem.LastModifiedOn = DateTime.Now;
            Update(materialIssueItem);
            string result = $"MaterialIssueItem of Detail {materialIssueItem.Id} is updated successfully!";
            return result;
        }
    }
}