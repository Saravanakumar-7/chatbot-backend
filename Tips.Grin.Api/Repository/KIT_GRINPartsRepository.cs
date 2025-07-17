using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Repository
{
    public class KIT_GRINPartsRepository : RepositoryBase<KIT_GRINParts>, IKIT_GRINPartsRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public KIT_GRINPartsRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<KIT_GRINParts> GetKIT_GRINPartsById(int id)
        {
            var grinPartsDetailsbyId = await _tipsGrinDbContext.KIT_GRINParts.Where(x => x.Id == id)
               .Include(d => d.KIT_GRIN_ProjectNumbers).ThenInclude(a=>a.KIT_GRIN_KITComponents).FirstOrDefaultAsync();
            return grinPartsDetailsbyId;
        }
        public async Task<string> UpdateKIT_GRINPartsQty(KIT_GRINParts grinParts)
        {
            Update(grinParts);
            string result = $"KIT_GRINParts Detail {grinParts.Id} is updated successfully!";
            return result;
        }
    }
}
