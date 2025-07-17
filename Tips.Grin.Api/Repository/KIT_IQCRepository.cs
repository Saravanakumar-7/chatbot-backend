using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Repository
{
    public class KIT_IQCRepository : RepositoryBase<KIT_IQC>, IKIT_IQCRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public KIT_IQCRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int?> CreateKIT_IQC(KIT_IQC kIT_IQC)
        {
            kIT_IQC.CreatedBy = _createdBy;
            kIT_IQC.CreatedOn = DateTime.Now;
            kIT_IQC.kIT_IQCItems.Where(x => x.CreatedBy == null && x.CreatedOn == null).ToList().ForEach(x => { x.CreatedBy = _createdBy; x.CreatedOn = DateTime.Now; });
            kIT_IQC.Unit = _unitname;
            var result = await Create(kIT_IQC);
            return result.Id;
        }
        public async Task<KIT_IQC?> GetKIT_IQCbyKIT_GrinNumber(string kIT_GrinNumber)
        {
            return await _tipsGrinDbContext.KIT_IQC.Where(x => x.KIT_GrinNumber == kIT_GrinNumber).Include(a=>a.kIT_IQCItems).FirstOrDefaultAsync();
        }
        public async Task<string> UpdateKIT_IQC(KIT_IQC kIT_IQC)
        {
            kIT_IQC.LastModifiedBy = _createdBy;
            kIT_IQC.LastModifiedOn = DateTime.Now;
            kIT_IQC.kIT_IQCItems.ForEach(x => { if (x.CreatedBy == null && x.CreatedOn == null) { x.CreatedBy = _createdBy; x.CreatedOn = DateTime.Now; }
                else { x.LastModifiedBy = _createdBy; x.LastModifiedOn = DateTime.Now; }
            });
            kIT_IQC.Unit = _unitname;
            Update(kIT_IQC);
            return $"KIT_IQC of Id:{kIT_IQC.Id} is updated successfully";
        }
        public async Task<PagedList<KIT_IQC>> GetAllKIT_IQC(PagingParameter pagingParameter,SearchParams searchParams)
        {
            var getallIQCList = FindAll()
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue)
                || (inv.KIT_GrinNumber != null && inv.KIT_GrinNumber.Contains(searchParams.SearchValue))
                || (inv.Id != null && inv.Id.ToString().Contains(searchParams.SearchValue))
                   || (inv.VendorName != null && inv.VendorName.Contains(searchParams.SearchValue))
                    || (inv.VendorNumber != null && inv.VendorNumber.Contains(searchParams.SearchValue))
                   || (inv.VendorId != null && inv.VendorId.Contains(searchParams.SearchValue))))).OrderByDescending(x => x.Id);

            return PagedList<KIT_IQC>.ToPagedList(getallIQCList, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<KIT_IQC> GetKIT_IQCbyId(int Id)
        {
            return await FindByCondition(x => x.Id == Id).Include(a => a.kIT_IQCItems).FirstOrDefaultAsync();
        }
    }
}
