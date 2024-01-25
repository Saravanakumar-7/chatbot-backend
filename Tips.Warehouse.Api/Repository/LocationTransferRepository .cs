using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
 

namespace Tips.Warehouse.Api.Repository
{
    public class LocationTransferRepository : RepositoryBase<LocationTransfer>, ILocationTransferRepository
    {
        
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public LocationTransferRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateLocationTransfer(LocationTransfer locationTransfer)
        {
            locationTransfer.CreatedBy = _createdBy;
            locationTransfer.CreatedOn = DateTime.Now;
            locationTransfer.Unit = _unitname;
            var result = await Create(locationTransfer);
            return result.Id;
        }

        public async Task<string> DeleteLocationTransfer(LocationTransfer locationTransfer)
        {
            Delete(locationTransfer);
            string result = $"locationTransfer details of {locationTransfer.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<LocationTransfer>> GetAllLocationTransfer([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var locationTransfers = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.FromPartNumber.Contains(searchParammes.SearchValue)
              || inv.FromLocation.Contains(searchParammes.SearchValue)
              || inv.ToLocation.Contains(searchParammes.SearchValue))));

            return PagedList<LocationTransfer>.ToPagedList(locationTransfers, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<IEnumerable<LocationTransferIdNameList>> GetAllLocationTransferIdNameList()
        {
            IEnumerable<LocationTransferIdNameList> locationTransferIdNameList = await _tipsWarehouseDbContext.locationTransfers
                                .Select(x => new LocationTransferIdNameList()
                                {
                                    Id = x.Id,

                                    FromPartNumber = x.FromPartNumber,
                                    ToPartNumber = x.ToPartNumber

                                })
                                .OrderByDescending(x => x.Id)
                              .ToListAsync();

            return locationTransferIdNameList;
        }

        public async Task<LocationTransfer> GetLocationTransferById(int id)
        {
            var LocationTransferbyId = await _tipsWarehouseDbContext.locationTransfers.Where(x => x.Id == id).FirstOrDefaultAsync();
            return LocationTransferbyId;
        }

        public async Task<string> UpdateLocationTransfer(LocationTransfer locationTransfer)
        {
            locationTransfer.LastModifiedBy = _createdBy;
            locationTransfer.LastModifiedOn = DateTime.Now;
            Update(locationTransfer);
            string result = $"locationTransfer of Detail {locationTransfer.Id} is updated successfully!";
            return result;
        }

        public async Task<IEnumerable<LocationTransfer>> SearchLocationTransfer([FromQuery] SearchParammes searchParammes)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.locationTransfers.AsQueryable();
                if (!string.IsNullOrEmpty(searchParammes.SearchValue))
                {
                    query = query.Where(po => po.FromPartNumber.Contains(searchParammes.SearchValue)
                    || po.ToPartNumber.Contains(searchParammes.SearchValue)
                    || po.FromLocation.Contains(searchParammes.SearchValue)
                    || po.ToLocation.Contains(searchParammes.SearchValue)
                    || po.FromUOM.Contains(searchParammes.SearchValue)
                    || po.ToUOM.Contains(searchParammes.SearchValue));

                }
                return query.ToList();
            }
        }

        public async Task<PagedList<LocationTransferSPReport>> LocationTransferSPReport(PagingParameter pagingParameter)
        {
            var results = _tipsWarehouseDbContext.Set<LocationTransferSPReport>()
                        .FromSqlInterpolated($"CALL Location_Transfer_Report")
                        .ToList();

            return PagedList<LocationTransferSPReport>.ToPagedList(results.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<IEnumerable<LocationTransfer>> SearchLocationTransferDate([FromQuery] SearchDateParam searchDatesParams)
        {
            var locationTransferDetails = _tipsWarehouseDbContext.locationTransfers
            .Where(inv => ((inv.CreatedOn >= searchDatesParams.SearchFromDate &&
            inv.CreatedOn <= searchDatesParams.SearchToDate
            )))
            .ToList();
            return locationTransferDetails;
        }

        //public async Task<LocationTransferFromDto> GetProjectLocWareFromInventoryByItemNo(string itemNumber)
        //{
        //    var inventoryDetail = await _tipsWarehouseDbContext.Inventory.Where(x => x.PartNumber == itemNumber
        //    && x.IsStockAvailable == true)
        //                  .ToListAsync();
        //    return inventoryDetail;
        //}
        public async Task<List<LocationTransferFromDto>> GetProjectLocWareFromInventoryByItemNo(string itemNumber)
        {
            var inventoryDetails = await _tipsWarehouseDbContext.Inventories
                .Where(x => x.PartNumber == itemNumber && x.IsStockAvailable == true)
                .GroupBy(x => new { x.ProjectNumber, x.Location, x.Warehouse })
                .Select(group => new LocationTransferFromDto
                {
                    FromProject = group.Key.ProjectNumber,
                    FromLocation = group.Key.Location,
                    FromWarehouse = group.Key.Warehouse,
                    AvailableStock = group.Sum(x => x.Balance_Quantity)
                })
                .ToListAsync();

            return inventoryDetails;
        }


        //public async Task<List<LocationTransferFromDto>> GetProjectLocWareFromInventoryByItemNo(string itemNumber)
        //{
        //    var inventoryDetails = await _tipsWarehouseDbContext.Inventory
        //        .Where(x => x.PartNumber == itemNumber && x.IsStockAvailable)
        //        .Select(x => new LocationTransferFromDto
        //        {
        //            FromProject = x.ProjectNumber ,
        //            FromLocation = x.Location ,
        //            FromWarehouse = x.Warehouse
        //        })
        //        .ToListAsync();

        //    // Assuming you only want to return the first item from the list, if there are multiple matching entries
        //    return inventoryDetails;
        //}
        public async Task<IEnumerable<LocationTransferSPReport>> LocationTransferSPReportWithParam(string FromPartNumber, string FromPartType, string FromWarehouse, string FromLocation, string FromProjectNumber, string ToPartnumber, string ToPartType, string ToWarehouse, string ToLocation, string ToProjectNumber)
        {
            {
                if (string.IsNullOrWhiteSpace(FromPartNumber)
           || string.IsNullOrWhiteSpace(FromPartType)
           || string.IsNullOrWhiteSpace(FromWarehouse)
           || string.IsNullOrWhiteSpace(FromLocation)
           || string.IsNullOrWhiteSpace(FromProjectNumber)
           || string.IsNullOrWhiteSpace(ToPartnumber)
           || string.IsNullOrWhiteSpace(ToPartType)
           || string.IsNullOrWhiteSpace(ToWarehouse)
           || string.IsNullOrWhiteSpace(ToLocation)
           || string.IsNullOrWhiteSpace(ToProjectNumber));
                var result = _tipsWarehouseDbContext
                .Set<LocationTransferSPReport>()
                .FromSqlInterpolated($"CALL Location_Transfer_Report_withparameter({FromPartNumber},{FromPartType},{FromWarehouse},{FromLocation},{FromProjectNumber},{ToPartnumber},{ToPartType},{ToWarehouse},{ToLocation},{ToProjectNumber})")
                .ToList();
                return result;
            }

        }
        public async Task<IEnumerable<LocationTransferSPReport>> LocationTransferSPReportDates(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsWarehouseDbContext.Set<LocationTransferSPReport>()
                         .FromSqlInterpolated($"CALL Location_Transfer_Report_withparameter_withdate({FromDate},{ToDate})")
                         .ToList();

            return results;
        }
        public async Task<IEnumerable<LocationTransfer>> GetAllLocationTransferWithItems(LocationTransferSearchDto locationTransferSearchDto)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.locationTransfers.AsQueryable();
                if (locationTransferSearchDto != null || (locationTransferSearchDto.FromPartNumber.Any())
               && locationTransferSearchDto.ToPartNumber.Any() && locationTransferSearchDto.FromUOM.Any()
               && locationTransferSearchDto.ToUOM.Any() && locationTransferSearchDto.FromLocation.Any()
               && locationTransferSearchDto.ToLocation.Any())
                {
                    query = query.Where
                    (po => (locationTransferSearchDto.FromPartNumber.Any() ? locationTransferSearchDto.FromPartNumber.Contains(po.FromPartNumber) : true)
                   && (locationTransferSearchDto.ToPartNumber.Any() ? locationTransferSearchDto.ToPartNumber.Contains(po.ToPartNumber) : true)
                   && (locationTransferSearchDto.FromUOM.Any() ? locationTransferSearchDto.FromUOM.Contains(po.FromUOM) : true)
                    && (locationTransferSearchDto.ToUOM.Any() ? locationTransferSearchDto.ToUOM.Contains(po.ToUOM) : true)
                     && (locationTransferSearchDto.FromLocation.Any() ? locationTransferSearchDto.FromLocation.Contains(po.FromLocation) : true)
                      && (locationTransferSearchDto.ToLocation.Any() ? locationTransferSearchDto.ToLocation.Contains(po.ToLocation) : true));
                }
                return await query.ToListAsync();
            }
        }
    }
}