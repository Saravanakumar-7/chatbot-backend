using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Repository
{
    public class LocationTransferRepository : RepositoryBase<LocationTransfer>, ILocationTransferRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public LocationTransferRepository(TipsSalesServiceDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsSalesServiceDbContext = repositoryContext;
        }

        public async Task<int?> CreateLocationTransfer(LocationTransfer locationTransfer)
        {
            locationTransfer.CreatedBy = "Admin";
            locationTransfer.CreatedOn = DateTime.Now;
            locationTransfer.Unit = "Bangalore";
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
            IEnumerable<LocationTransferIdNameList> locationTransferIdNameList = await _tipsSalesServiceDbContext.locationTransfers
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
            var LocationTransferbyId = await _tipsSalesServiceDbContext.locationTransfers.Where(x => x.Id == id).FirstOrDefaultAsync();
            return LocationTransferbyId;
        }

        public async Task<string> UpdateLocationTransfer(LocationTransfer locationTransfer)
        {
            locationTransfer.LastModifiedBy = "Admin";
            locationTransfer.LastModifiedOn = DateTime.Now;
            Update(locationTransfer);
            string result = $"locationTransfer of Detail {locationTransfer.Id} is updated successfully!";
            return result;
        }

        //public async Task<IEnumerable<LocationTransfer>> SearchLocationTransfer([FromQuery] SearchParammes searchParammes)
        //{
        //    using (var context = _tipsSalesServiceDbContext)
        //    {
        //        var query = _tipsSalesServiceDbContext.locationTransfers.ToList();
        //        if (!string.IsNullOrEmpty(searchParammes.SearchValue))
        //        {
        //            query = query.Where(po => po.FromPartNumber.Contains(searchParammes.SearchValue)
        //            || po.ToPartNumber.Contains(searchParammes.SearchValue)
        //            || po.FromLocation.Contains(searchParammes.SearchValue)
        //            || po.ToLocation.Contains(searchParammes.SearchValue)
        //            || po.FromUOM.Contains(searchParammes.SearchValue)
        //            || po.ToUOM.Contains(searchParammes.SearchValue));




        //        }
        //        return query.ToList();
        //    }
        //}

        public async Task<IEnumerable<LocationTransfer>> SearchLocationTransferDate([FromQuery] SearchDateParam searchDatesParams)
        {
            var locationTransferDetails = _tipsSalesServiceDbContext.locationTransfers
            .Where(inv => ((inv.CreatedOn >= searchDatesParams.SearchFromDate &&
            inv.CreatedOn <= searchDatesParams.SearchToDate
            )))
            .ToList();
            return locationTransferDetails;
        }
    }
}