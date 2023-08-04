using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface ILocationTransferRepository : IRepositoryBase<LocationTransfer>
    {
        Task<int?> CreateLocationTransfer(LocationTransfer locationTransfer);
        Task<string> UpdateLocationTransfer(LocationTransfer locationTransfer);
        Task<string> DeleteLocationTransfer(LocationTransfer locationTransfer);
        Task<PagedList<LocationTransfer>> GetAllLocationTransfer(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<LocationTransfer> GetLocationTransferById(int id);
        Task<List<LocationTransferFromDto>> GetProjectLocWareFromInventoryByItemNo(string itemNumber); 
        Task<IEnumerable<LocationTransfer>> SearchLocationTransfer([FromQuery] SearchParammes searchParammes);
        Task<IEnumerable<LocationTransfer>> SearchLocationTransferDate([FromQuery] SearchDateParam searchDatesParams);
        Task<IEnumerable<LocationTransfer>> GetAllLocationTransferWithItems(LocationTransferSearchDto locationTransferSearchDto);
        Task<IEnumerable<LocationTransferIdNameList>> GetAllLocationTransferIdNameList();

    }
}