using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Contracts;

namespace Tips.Grin.Api.Repository
{
    public class GrinsForServiceItemsRepository : RepositoryBase<GrinsForServiceItems>, IGrinsForServiceItemsRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public GrinsForServiceItemsRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<PagedList<GrinsForServiceItems>> GrinsForServiceItemsForServiceItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            var getAllGrinsForServiceItemsDetails = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.GrinsForServiceItemsNumber.Contains(searchParams.SearchValue) ||
                 inv.InvoiceNumber.Contains(searchParams.SearchValue) || inv.VendorName.Contains(searchParams.SearchValue))));           

            return PagedList<GrinsForServiceItems>.ToPagedList(getAllGrinsForServiceItemsDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
    }
}
