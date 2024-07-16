using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Repository
{
    public class IQCForServiceItemsRepository : RepositoryBase<IQCForServiceItems>, IIQCForServiceItemsRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public IQCForServiceItemsRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task<IQCForServiceItems> GetIqcForServiceItemsDetailsbyGrinForServiceItemsNo(string grinsForServiceItemsNumber)
        {
            var iQCDetail = await _tipsGrinDbContext.IQCForServiceItems.Where(x => x.GrinsForServiceItemsNumber == grinsForServiceItemsNumber).Include(t => t.IQCForServiceItems_Items).FirstOrDefaultAsync();
            return iQCDetail;
        }
        public async Task<int?> CreateIQCForServiceItems(IQCForServiceItems iQCForServiceItems)
        {
            iQCForServiceItems.CreatedBy = _createdBy;
            iQCForServiceItems.CreatedOn = DateTime.Now;
            iQCForServiceItems.Unit = _unitname;
            var result = await Create(iQCForServiceItems);
            return result.Id;
        }
        public async Task<PagedList<IQCForServiceItems>> GetAllIQCForServiceItemsDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            var getallIQCList = FindAll()
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || (inv.GrinsForServiceItemsNumber != null && inv.GrinsForServiceItemsNumber.Contains(searchParams.SearchValue)) ||
                   (inv.Id != null && inv.Id.ToString().Contains(searchParams.SearchValue))))).OrderByDescending(x => x.Id);
            return PagedList<IQCForServiceItems>.ToPagedList(getallIQCList, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<IQCForServiceItems> GetIQCForServiceItemsDetailsbyGrinForServiceItemsNo(string grinsForServiceItemsNumber)
        {
            var iQCDetail = await _tipsGrinDbContext.IQCForServiceItems.Where(x => x.GrinsForServiceItemsNumber == grinsForServiceItemsNumber)
               .Include(t => t.IQCForServiceItems_Items).FirstOrDefaultAsync();
            return iQCDetail;
        }
        public async Task<string> UpdateIQCForServiceItems(IQCForServiceItems iQCForServiceItems)
        {
            iQCForServiceItems.LastModifiedBy = _createdBy;
            iQCForServiceItems.LastModifiedOn = DateTime.Now;
            Update(iQCForServiceItems);
            string result = $"IQCForServiceItems details of {iQCForServiceItems.Id} is updated successfully!";
            return result;
        }
        public async Task<IQCForServiceItems> GetIQCForServiceItemsDetailsbyId(int id)
        {
            var iQCDetailsbyId = await _tipsGrinDbContext.IQCForServiceItems.Where(x => x.Id == id)
                                .Include(x => x.IQCForServiceItems_Items)
                                .FirstOrDefaultAsync();
            return iQCDetailsbyId;
        }
    }
}
