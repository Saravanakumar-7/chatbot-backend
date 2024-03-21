using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Repository
{
    public class OQCBinningRepository : RepositoryBase<OQCBinning>, IOQCBinningRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public OQCBinningRepository(TipsProductionDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int> CreateOQCBinning(OQCBinning oQCBinning)
        {
            oQCBinning.CreatedBy = _createdBy;
            oQCBinning.CreatedOn = DateTime.Now;
            oQCBinning.Unit = _unitname;
            var result = await Create(oQCBinning);

            return result.Id;
        }
        public async Task<OQCBinning> GetOQCBinningById(int id)
        {
            var oQCDedtailsById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            List<OQCBinningLocation>? oqcbinloc = await _tipsProductionDbContext.OQCBinningLocation.Where(x => x.OQCBinningId == oQCDedtailsById.Id).ToListAsync();
            oQCDedtailsById.oQCBinningLocations = oqcbinloc;

            return oQCDedtailsById;
        }
        public async Task<PagedList<OQCBinning>> GetAllOQCBinning([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            var OQCDetails = FindAll().OrderByDescending(x => x.Id).Include(x=>x.oQCBinningLocations)
                           .Where(inv => ((string.IsNullOrWhiteSpace(searchParamess.SearchValue) || inv.ShopOrderNumber.Contains(searchParamess.SearchValue) ||
                              inv.ItemNumber.Contains(searchParamess.SearchValue))));

            return PagedList<OQCBinning>.ToPagedList(OQCDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<List<OQCStock>?> GetOqcBinningShopOrderQty(string Itemnumber)
        {
            var ShopOrderQty = await _tipsProductionDbContext.OQCBinning.Where(x => x.ItemNumber == Itemnumber).GroupBy(x => new { x.ItemNumber, x.ShopOrderNumber })
            .Select(g => new OQCStock
            {
                ItemNumber = g.Key.ItemNumber,
                ShopOrderNumber = g.Key.ShopOrderNumber,
                TotalAcceptedQty = g.Sum(x => x.ShopOrderQty)
            }).ToListAsync();
            return ShopOrderQty;
        }
    }
    
}
