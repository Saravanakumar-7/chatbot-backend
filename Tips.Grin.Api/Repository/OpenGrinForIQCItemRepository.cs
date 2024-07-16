using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Repository
{
    public class OpenGrinForIQCItemRepository : RepositoryBase<OpenGrinForIQCItems>, IOpenGrinForIQCItemRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public OpenGrinForIQCItemRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateOpenGrinForIQCItems(OpenGrinForIQCItems openGrinForIQC)
        {
            var result = await Create(openGrinForIQC);
            return result.Id;
        }
        public async Task<int?> GetOpenGrinForIQCItemsCount(int openGrinForIQCId)
        {
            var openGrinForIQCItemsCount = _tipsGrinDbContext.OpenGrinForIQCItems.Where(x => x.OpenGrinForIQCId == openGrinForIQCId).Count();

            return openGrinForIQCItemsCount;
        }
        public async Task<OpenGrinForIQCItems> GetOpenGrinForIQCItemsDetailsbyOpenGrinForGrinItemId(int openGrinForGrinItemId)
        {
            var openGrinForGrinItemsDetails = await _tipsGrinDbContext.OpenGrinForIQCItems.Where(x => x.OpenGrinForGrinItemId == openGrinForGrinItemId)
                .FirstOrDefaultAsync();

            return openGrinForGrinItemsDetails;
        }
        public async Task<string> UpdateOpenGrinForIQCItems(OpenGrinForIQCItems openGrinForIQCItems)
        {
            Update(openGrinForIQCItems);
            string result = $"OpenGrinForIQCItems details of {openGrinForIQCItems.Id} is updated successfully!";
            return result;
        }
    }
}
