using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Repository
{
    public class OpenGrinForBinningItemsRepository : RepositoryBase<OpenGrinForBinningItems>, IOpenGrinForBinningItemsRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public OpenGrinForBinningItemsRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task<string> UpdateOpenGrinForBinningItems(OpenGrinForBinningItems openGrinForBinningItems)
        {
            Update(openGrinForBinningItems);
            string result = $"OpenGrinForBinningItems details of {openGrinForBinningItems.Id} is updated successfully!";
            return result;
        }
        public async Task<OpenGrinForBinningItems> CreateOpenGrinForBinningItems(OpenGrinForBinningItems openGrinForBinningItems)
        {

            var result = await Create(openGrinForBinningItems);
            return result;
        }
    }
}
