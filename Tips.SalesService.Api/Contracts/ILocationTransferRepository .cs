using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface ILocationTransferRepository : IRepositoryBase<LocationTransfer>
    {
        Task<int?> CreateLocationTransfer(LocationTransfer locationTransfer);
        Task<string> UpdateLocationTransfer(LocationTransfer locationTransfer);
        Task<string> DeleteLocationTransfer(LocationTransfer locationTransfer);
        Task<PagedList<LocationTransfer>> GetAllLocationTransfer(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<LocationTransfer> GetLocationTransferById(int id);

        Task<IEnumerable<LocationTransfer>> SearchLocationTransfer([FromQuery] SearchParammes searchParammes);
        Task<IEnumerable<LocationTransfer>> SearchLocationTransferDate([FromQuery] SearchDateParam searchDatesParams);
        Task<IEnumerable<LocationTransfer>> GetAllLocationTransferWithItems(LocationTransferSearchDto locationTransferSearchDto);
        Task<IEnumerable<LocationTransferIdNameList>> GetAllLocationTransferIdNameList();

    }
}