using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

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
            locationTransfer.LastModifiedBy = "Admin";
            locationTransfer.LastModifiedOn = DateTime.Now;
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

        public async Task<PagedList<LocationTransfer>> GetAllLocationTransfer(PagingParameter pagingParameter)
        {
            var AllLocationTransDetails = PagedList<LocationTransfer>.ToPagedList(FindAll()
                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return AllLocationTransDetails;
        }

        public async Task<LocationTransfer> GetLocationTransferById(int id)
        {
            var LocationTransDetailsId = await _tipsSalesServiceDbContext.locationTransfers.Where(x => x.Id == id).FirstOrDefaultAsync();
            return LocationTransDetailsId;
        }

        public async Task<string> UpdateLocationTransfer(LocationTransfer locationTransfer)
        {
            locationTransfer.LastModifiedBy = "Admin";
            locationTransfer.LastModifiedOn = DateTime.Now;
            Update(locationTransfer);
            string result = $"locationTransfer of Detail {locationTransfer.Id} is updated successfully!";
            return result;
        }
    }
}