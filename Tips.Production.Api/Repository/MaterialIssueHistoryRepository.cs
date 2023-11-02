using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using System.Security.Claims;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Repository
{
    public class MaterialIssueHistoryRepository : RepositoryBase<MaterialIssueHistory>, IMaterialIssueHistoryRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public MaterialIssueHistoryRepository(TipsProductionDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int> CreateMaterialIssueHistory(MaterialIssueHistory materialIssueHistory)
        {
            materialIssueHistory.CreatedBy = _createdBy;
            materialIssueHistory.CreatedOn = DateTime.Now;
            materialIssueHistory.Unit = _unitname;
            var result = await Create(materialIssueHistory);
            return result.Id;
        }
    }
}
