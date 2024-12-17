using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

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
    }
}
