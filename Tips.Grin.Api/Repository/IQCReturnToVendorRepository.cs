using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;

namespace Tips.Grin.Api.Repository
{
    public class IQCReturnToVendorRepository : RepositoryBase<IQCReturnToVendor>, IIQCReturnToVendorRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public IQCReturnToVendorRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateIQCReturnToVendor(IQCReturnToVendor iQCReturnToVendor)
        {
            iQCReturnToVendor.CreatedBy = _createdBy;
            iQCReturnToVendor.CreatedOn = DateTime.Now;
            iQCReturnToVendor.Unit = _unitname;
            var result = await Create(iQCReturnToVendor);
            return result.Id;
        }
        public async Task<PagedList<IQCReturnToVendor>> GetAllIQCReturnToVendor([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            var getAllIQCReturnToVendor = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.GrinNumber.Contains(searchParams.SearchValue) ||
              inv.IQCNumber.Contains(searchParams.SearchValue) || inv.VendorName.Contains(searchParams.SearchValue) || inv.VendorId.Contains(searchParams.SearchValue) 
              || inv.VendorNumber.Contains(searchParams.SearchValue))));

            return PagedList<IQCReturnToVendor>.ToPagedList(getAllIQCReturnToVendor, pagingParameter.PageNumber, pagingParameter.PageSize);

        }
        public async Task<IQCReturnToVendor> GetIQCReturnToVendorById(int id)
        {
            var IQCReturnToVendorDetailsbyId = await _tipsGrinDbContext.IQCReturnToVendor.Where(x => x.Id == id)
              .Include(t => t.iQCReturnToVendorItems).FirstOrDefaultAsync();

            return IQCReturnToVendorDetailsbyId;
        }
    }
}
