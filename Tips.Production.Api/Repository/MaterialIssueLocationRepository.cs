using System.Security.Claims;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Repository
{
    public class MaterialIssueLocationRepository : RepositoryBase<MaterialIssueLocation>, IMaterialIssueLocationRepository
    {
        private AdvitaTipsProductionDbContext _advitaTipsProductionDbContext;
        private TipsProductionDbContext _tipsProductionDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public MaterialIssueLocationRepository(TipsProductionDbContext repositoryContext, AdvitaTipsProductionDbContext advitaTipsProductionDbContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext, advitaTipsProductionDbContext)
        {
            _advitaTipsProductionDbContext = advitaTipsProductionDbContext;
            _tipsProductionDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int> CreateMaterialIssueLocation(MaterialIssueLocation materialIssueLocation)
        {
            var result = await Create(materialIssueLocation);
            return result.Id;
        }
    }
}
