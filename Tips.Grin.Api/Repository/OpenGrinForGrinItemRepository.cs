using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Repository
{
    public class OpenGrinForGrinItemRepository : RepositoryBase<OpenGrinForGrinItems>, IOpenGrinForGrinItemRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public OpenGrinForGrinItemRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateOpenGrinForGrinItems(OpenGrinForGrinItems openGrinForGrinItems)
        {

            var result = await Create(openGrinForGrinItems);
            return result.Id;
        }

        public async Task<OpenGrinForGrinItems> GetOpenGrinForGrinItemById(int id)
        {
            var openGrinForGrinDetailsbyId = await _tipsGrinDbContext.OpenGrinForGrinItems.Where(x => x.Id == id)

                             .FirstOrDefaultAsync();

            return openGrinForGrinDetailsbyId;
        }
        public async Task<string> UpdateOpenGrinForGrinItem(OpenGrinForGrinItems openGrinForGrinItems)
        {
            Update(openGrinForGrinItems);
            string result = $"OpenGrinForGrinItems details of {openGrinForGrinItems.Id} is updated successfully!";
            return result;
        }
        public async Task<OpenGrinForGrinItems> GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(int openGrinForGrinItemId)
        {
            var openGrinForGrinItemDetails = await _tipsGrinDbContext.OpenGrinForGrinItems.Where(x => x.Id == openGrinForGrinItemId)
                .FirstOrDefaultAsync();

            return openGrinForGrinItemDetails;
        }
        public async Task<int?> GetOpenGrinForGrinItemsIqcStatusCount(int grinId)
        {
            var openGrinForGrinItemsIqcStatusCount = _tipsGrinDbContext.OpenGrinForGrinItems.Where(x => x.OpenGrinForGrinId == grinId && x.IsOpenGrinForIqcCompleted == false).Count();

            return openGrinForGrinItemsIqcStatusCount;
        }
        public async Task<OpenGrinForGrinItems> UpdateOpenGrinForGrinItemsQty(int openGrinForGrinPartId, string AcceptedQty, string RejectedQty)
        {
            var data = await _tipsGrinDbContext.OpenGrinForGrinItems.Where(x => x.Id == openGrinForGrinPartId).FirstOrDefaultAsync();
            data.AcceptedQty = Convert.ToDecimal(AcceptedQty);
            data.RejectedQty = Convert.ToDecimal(RejectedQty);
            return data;
        }
        public async Task<int?> GetOpenGrinForGrinItemsCount(int openGrinForGrinId)
        {
            var openGrinForGrinItemsCount = _tipsGrinDbContext.OpenGrinForGrinItems.Where(x => x.OpenGrinForGrinId == openGrinForGrinId).Count();

            return openGrinForGrinItemsCount;
        }
        public async Task<int?> GetOpenGrinForGrinItemsBinningStatusCount(int openGrinForGrinId)
        {
            var openGrinForGrinItemsBinningStatusCount = _tipsGrinDbContext.OpenGrinForGrinItems.Where(x => x.OpenGrinForGrinId == openGrinForGrinId && x.IsOpenGrinForBinningCompleted == false).Count();

            return openGrinForGrinItemsBinningStatusCount;
        }
    }
}
