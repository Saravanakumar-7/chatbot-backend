using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Repository
{
    public class LocationTransferPartNoRepository : RepositoryBase<LocationTransferPartNo>, ILocationTransferPartNoRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public LocationTransferPartNoRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateLocationTransferPartNo(LocationTransferPartNo locationTransferPartNo)
        {
            locationTransferPartNo.CreatedBy = _createdBy;
            locationTransferPartNo.CreatedOn = DateTime.Now;
            locationTransferPartNo.Unit = _unitname;
            var result = await Create(locationTransferPartNo);
            return result.Id;
        }

        public async Task<PagedList<LocationTransferPartNo>> GetAllLocationTransferPartNo(PagingParameter pagingParameter, SearchParammes searchParammes)
        {
            var locationTransferPartNo = FindAll().OrderByDescending(x => x.Id)
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.FromPartNumber.Contains(searchParammes.SearchValue)
             || inv.FromLocation.Contains(searchParammes.SearchValue)
             || inv.ToLocation.Contains(searchParammes.SearchValue))));

            return PagedList<LocationTransferPartNo>.ToPagedList(locationTransferPartNo, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<LocationTransferPartNo> GetLocationTransferPartNoById(int id)
        {
            var LocationTransferPartNo = await _tipsWarehouseDbContext.LocationTransferPartNos.Where(x => x.Id == id).FirstOrDefaultAsync();
            return LocationTransferPartNo;
        }
        public async Task<int> GetLatestLocationTransferPartNoId()
        {
            var latestLocationTransferId = await _tipsWarehouseDbContext.locationTransfers
                                                 .OrderByDescending(x => x.Id)
                                                 .Select(x => x.Id)
                                                 .FirstOrDefaultAsync();
            return latestLocationTransferId;
        }
    }
}
