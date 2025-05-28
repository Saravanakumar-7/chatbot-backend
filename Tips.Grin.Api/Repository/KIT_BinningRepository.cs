using Entities.Helper;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Repository
{
    public class KIT_BinningRepository : RepositoryBase<KIT_Binning>, IKIT_BinningRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public KIT_BinningRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int?> CreateKIT_Binning(KIT_Binning kIT_Binning)
        {
            kIT_Binning.CreatedBy = _createdBy;
            kIT_Binning.CreatedOn = DateTime.Now;
            kIT_Binning.KIT_BinningItems.Where(x => x.CreatedBy == null && x.CreatedOn == null).ToList().ForEach(x => { x.CreatedBy = _createdBy; x.CreatedOn = DateTime.Now; });
            kIT_Binning.Unit = _unitname;
            var result = await Create(kIT_Binning);
            return result.Id;
        }
        public async Task<KIT_Binning?> GetKIT_BinningbyKIT_GrinNumber(string kIT_GrinNumber)
        {
            return await _tipsGrinDbContext.KIT_Binning.Where(x => x.KIT_GrinNumber == kIT_GrinNumber).Include(a => a.KIT_BinningItems).FirstOrDefaultAsync();
        }
        public async Task<string> UpdateKIT_Binning(KIT_Binning kIT_Binning)
        {
            kIT_Binning.LastModifiedBy = _createdBy;
            kIT_Binning.LastModifiedOn = DateTime.Now;
            kIT_Binning.KIT_BinningItems.Where(x => x.CreatedBy == null && x.CreatedOn == null).ToList().ForEach(x => { x.CreatedBy = _createdBy; x.CreatedOn = DateTime.Now; });
            kIT_Binning.Unit = _unitname;
            Update(kIT_Binning);
            return $"KIT_Binning of Id:{kIT_Binning.Id} is updated successfully";
        }
        public async Task<PagedList<KIT_Binning>> GetAllKIT_Binning(PagingParameter pagingParameter, SearchParams searchParams)
        {
            var getallIQCList = FindAll()
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue)
                || (inv.KIT_GrinNumber != null && inv.KIT_GrinNumber.Contains(searchParams.SearchValue))
                || (inv.Id != null && inv.Id.ToString().Contains(searchParams.SearchValue))
                   || (inv.VendorName != null && inv.VendorName.Contains(searchParams.SearchValue))
                    || (inv.VendorNumber != null && inv.VendorNumber.Contains(searchParams.SearchValue))
                   || (inv.VendorId != null && inv.VendorId.Contains(searchParams.SearchValue))))).OrderByDescending(x => x.Id);

            return PagedList<KIT_Binning>.ToPagedList(getallIQCList, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<KIT_Binning> GetKIT_BinningbyId(int Id)
        {
            return await FindByCondition(x => x.Id == Id).Include(a => a.KIT_BinningItems).FirstOrDefaultAsync();
        }
    }
}
