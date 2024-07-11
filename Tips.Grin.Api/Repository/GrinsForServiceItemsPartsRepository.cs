using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Repository
{
    public class GrinsForServiceItemsPartsRepository : RepositoryBase<GrinsForServiceItemsParts>, IGrinsForServiceItemsPartsRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public GrinsForServiceItemsPartsRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<GrinsForServiceItemsParts> GetGrinsForServiceItemsPartsById(int id)
        {
            var grinPartsDetailsbyId = await _tipsGrinDbContext.GrinsForServiceItemsParts.Where(x => x.Id == id).Include(d => d.GrinsForServiceItemsProjectNumbers).FirstOrDefaultAsync();

            return grinPartsDetailsbyId;
        }

        public async Task<string> UpdateGrinsForServiceItemsParts(GrinsForServiceItemsParts grinsForServiceItemsParts)
        {
            grinsForServiceItemsParts.LastModifiedBy = _createdBy;
            grinsForServiceItemsParts.LastModifiedOn = DateTime.Now;
            Update(grinsForServiceItemsParts);
            string result = $"GrinsForServiceItemsParts Detail {grinsForServiceItemsParts.Id} is updated successfully!";
            return result;
        }
        public async Task<GrinsForServiceItemsParts> GetGrinsForServiceItemsPartsDetailsbyGrinsForServiceItemsPartsId(int GrinsForServiceItemsPartsId)
        {
            var grinPartsDetails = await _tipsGrinDbContext.GrinsForServiceItemsParts.Where(x => x.Id == GrinsForServiceItemsPartsId)
                .Include(x => x.GrinsForServiceItemsProjectNumbers).FirstOrDefaultAsync();
            return grinPartsDetails;
        }
        public async Task<int?> GetGrinsForServiceItemsPartsCount(int grinForServiceItemsId)
        {
            var grinPartsBinningStatusCount = _tipsGrinDbContext.GrinsForServiceItemsParts.Where(x => x.GrinsForServiceItemsId == grinForServiceItemsId).Count();

            return grinPartsBinningStatusCount;
        }
        public async Task<GrinsForServiceItemsParts> UpdateGrinsForServiceItemsPartsQty(int GrinsForServiceItemsPartsId, string AcceptedQty, string RejectedQty)
        {
            var data = await _tipsGrinDbContext.GrinsForServiceItemsParts.Where(x => x.Id == GrinsForServiceItemsPartsId).FirstOrDefaultAsync();
            data.AcceptedQty = Convert.ToDecimal(AcceptedQty);
            data.RejectedQty = Convert.ToDecimal(RejectedQty);
            return data;
        }
    }
}
